using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace ytlivechatwedus
{
    public partial class MainWindow : Window
    {
        // Win32 Global Hotkey APIs
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_L = 0x004C; // 'L' key
        private const int HOTKEY_ID = 9000;
        private const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource? _hwndSource;
        
        private OverlayWindow? _overlayWindow;
        private readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            _isLoaded = true;
            ApplyChangesToOverlay();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // Register Global Hotkey (Ctrl + Shift + L)
            _windowHandle = new WindowInteropHelper(this).Handle;
            _hwndSource = HwndSource.FromHwnd(_windowHandle);
            _hwndSource.AddHook(HwndHook);
            
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_L);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                chkClickThrough.IsChecked = !chkClickThrough.IsChecked;
                handled = true;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unregister hotkey
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(HwndHook);
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
            }

            // Close overlay if it's still open
            if (_overlayWindow != null)
            {
                _overlayWindow.Close();
            }

            SaveSettings();
            base.OnClosed(e);
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Please enter a valid YouTube Stream URL or Video ID.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow();
                _overlayWindow.Closed += (s, ev) => 
                {
                    _overlayWindow = null;
                    lblStatus.Text = "Status: Disconnected";
                    btnConnect.Content = "LAUNCH OVERLAY";
                };

                _overlayWindow.Show();
                ApplyChangesToOverlay();
                _overlayWindow.LoadChat(txtUrl.Text);

                lblStatus.Text = "Status: Active";
                btnConnect.Content = "RELOAD CHAT";
            }
            else
            {
                // Reload chat on existing overlay
                _overlayWindow.LoadChat(txtUrl.Text);
            }
        }

        private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lblOpacity != null)
            {
                lblOpacity.Text = $"{(int)sliderOpacity.Value}%";
            }

            if (_overlayWindow != null)
            {
                _overlayWindow.Opacity = sliderOpacity.Value / 100.0;
            }
        }

        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lblZoom != null)
            {
                lblZoom.Text = $"{(int)sliderZoom.Value}%";
            }

            if (_overlayWindow != null)
            {
                _overlayWindow.SetZoom(sliderZoom.Value / 100.0);
            }
        }

        private void ChkClickThrough_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkClickThrough.IsChecked == true;
            if (_overlayWindow != null)
            {
                _overlayWindow.SetClickThrough(isChecked);
            }
        }

        private void ChkAlwaysOnTop_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkAlwaysOnTop.IsChecked == true;
            if (_overlayWindow != null)
            {
                _overlayWindow.Topmost = isChecked;
            }
        }

        private void ComboPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded) return;

            bool isCustom = comboPresets.SelectedIndex == 4;
            
            // Toggle custom CSS textbox visibility
            if (lblCustomCss != null && txtCustomCss != null)
            {
                var vis = isCustom ? Visibility.Visible : Visibility.Collapsed;
                lblCustomCss.Visibility = vis;
                txtCustomCss.Visibility = vis;
            }

            ApplyThemePreset();
        }

        private void TxtCustomCss_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || comboPresets.SelectedIndex != 4) return;
            ApplyThemePreset();
        }

        private void ApplyThemePreset()
        {
            if (_overlayWindow == null) return;

            string selectedCss = ThemePresets.MinimalistBubbles;

            switch (comboPresets.SelectedIndex)
            {
                case 0:
                    selectedCss = ThemePresets.MinimalistBubbles;
                    break;
                case 1:
                    selectedCss = ThemePresets.NeonCyberpunk;
                    break;
                case 2:
                    selectedCss = ThemePresets.ClearMonospace;
                    break;
                case 3:
                    selectedCss = ThemePresets.GlassmorphismLight;
                    break;
                case 4:
                    selectedCss = txtCustomCss.Text;
                    break;
            }

            _overlayWindow.UpdateTheme(selectedCss);
        }

        private void ApplyChangesToOverlay()
        {
            if (_overlayWindow == null) return;

            _overlayWindow.Opacity = sliderOpacity.Value / 100.0;
            _overlayWindow.SetZoom(sliderZoom.Value / 100.0);
            _overlayWindow.SetClickThrough(chkClickThrough.IsChecked == true);
            _overlayWindow.Topmost = chkAlwaysOnTop.IsChecked == true;
            ApplyThemePreset();
        }

        // --- Settings Persistence ---
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<OverlaySettings>(json);
                    
                    if (settings != null)
                    {
                        txtUrl.Text = settings.Url;
                        sliderOpacity.Value = settings.Opacity;
                        sliderZoom.Value = settings.Zoom;
                        comboPresets.SelectedIndex = settings.PresetIndex;
                        chkClickThrough.IsChecked = settings.ClickThrough;
                        chkAlwaysOnTop.IsChecked = settings.AlwaysOnTop;
                        txtCustomCss.Text = settings.CustomCss;
                        return;
                    }
                }
            }
            catch
            {
                // Fallback to defaults
            }

            // Defaults
            txtUrl.Text = "https://www.youtube.com/watch?v=5qap5aO4i9A";
            sliderOpacity.Value = 80;
            sliderZoom.Value = 100;
            comboPresets.SelectedIndex = 0;
            chkClickThrough.IsChecked = false;
            chkAlwaysOnTop.IsChecked = true;
            txtCustomCss.Text = ThemePresets.BaseCSS;
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new OverlaySettings
                {
                    Url = txtUrl.Text,
                    Opacity = sliderOpacity.Value,
                    Zoom = sliderZoom.Value,
                    PresetIndex = comboPresets.SelectedIndex,
                    ClickThrough = chkClickThrough.IsChecked == true,
                    AlwaysOnTop = chkAlwaysOnTop.IsChecked == true,
                    CustomCss = txtCustomCss.Text
                };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
                // Ignore settings write errors
            }
        }
    }

    public class OverlaySettings
    {
        public string Url { get; set; } = "";
        public double Opacity { get; set; }
        public double Zoom { get; set; }
        public int PresetIndex { get; set; }
        public bool ClickThrough { get; set; }
        public bool AlwaysOnTop { get; set; }
        public string CustomCss { get; set; } = "";
    }
}