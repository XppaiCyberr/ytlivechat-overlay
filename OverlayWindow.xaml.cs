using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Web.WebView2.Core;

namespace ytlivechatwedus
{
    public partial class OverlayWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }

        private bool _isClickThrough = false;
        private string _currentCss = ThemePresets.MinimalistBubbles;
        private double _currentZoom = 1.0;
        private bool _isInitialized = false;

        public OverlayWindow()
        {
            InitializeComponent();
            Loaded += OverlayWindow_Loaded;
            InitializeWebView();
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial click-through state
            SetOverlayMode(_isClickThrough);
        }

        private async void InitializeWebView()
        {
            try
            {
                // Ensure Environment is initialized with transparency support
                var env = await CoreWebView2Environment.CreateAsync(null, null, null);
                await webView.EnsureCoreWebView2Async(env);

                // Configure WebView settings for a clean kiosk/widget feel
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

                // Make background fully transparent
                webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;

                // Hook Page finished loading to apply styles automatically
                webView.NavigationCompleted += WebView_NavigationCompleted;

                _isInitialized = true;
                
                // Set initial zoom and styles
                SetZoom(_currentZoom);
                UpdateTheme(_currentCss);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize WebView2: {ex.Message}\nMake sure Microsoft Edge is installed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                ApplyStyles();
            }
        }

        public void LoadChat(string videoIdOrUrl)
        {
            if (!_isInitialized) return;

            string? videoId = ExtractVideoId(videoIdOrUrl);
            if (string.IsNullOrEmpty(videoId))
            {
                MessageBox.Show("Invalid YouTube URL or Video ID.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string chatUrl = $"https://www.youtube.com/live_chat?v={videoId}";
            webView.Source = new Uri(chatUrl);
        }

        private string? ExtractVideoId(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = input.Trim();

            // Direct 11-char Video ID
            if (input.Length == 11 && !input.Contains("/") && !input.Contains("?"))
            {
                return input;
            }

            try
            {
                // Parse standard URL formats
                if (input.Contains("youtube.com") || input.Contains("youtu.be"))
                {
                    Uri uri = new Uri(input);
                    if (input.Contains("youtu.be"))
                    {
                        return uri.AbsolutePath.TrimStart('/');
                    }

                    if (input.Contains("live_chat"))
                    {
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        return query["v"];
                    }

                    if (input.Contains("/watch"))
                    {
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        return query["v"];
                    }

                    if (input.Contains("/live/"))
                    {
                        var parts = uri.AbsolutePath.Split('/');
                        foreach (var part in parts)
                        {
                            if (part.Length == 11) return part;
                        }
                    }
                }
            }
            catch
            {
                // Fallback to substring matching if Uri fails
            }

            return null;
        }

        public void SetClickThrough(bool clickThrough)
        {
            _isClickThrough = clickThrough;
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero) return;

            IntPtr extendedStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
            if (clickThrough)
            {
                // Add transparent & layered flags
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(extendedStyle.ToInt64() | WS_EX_TRANSPARENT | WS_EX_LAYERED));
            }
            else
            {
                // Remove transparent flag
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(extendedStyle.ToInt64() & ~WS_EX_TRANSPARENT));
            }
        }

        public void SetZoom(double zoom)
        {
            _currentZoom = zoom;
            if (webView.CoreWebView2 != null)
            {
                webView.ZoomFactor = zoom;
            }
        }

        public void UpdateTheme(string css)
        {
            _currentCss = css;
            ApplyStyles();
        }

        private async void ApplyStyles()
        {
            if (webView.CoreWebView2 == null || string.IsNullOrEmpty(_currentCss)) return;

            // Escaped CSS injection
            string cleanedCss = _currentCss.Replace("`", "\\`").Replace("\n", " ").Replace("\r", " ");
            string js = $@"
                (function() {{
                    let style = document.getElementById('custom-overlay-style');
                    if (!style) {{
                        style = document.createElement('style');
                        style.id = 'custom-overlay-style';
                        document.head.appendChild(style);
                    }}
                    style.textContent = `{cleanedCss}`;
                }})();
            ";
            
            await webView.CoreWebView2.ExecuteScriptAsync(js);
        }
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void SetOverlayMode(bool isLocked)
        {
            _isClickThrough = isLocked;
            
            if (isLocked)
            {
                TitleBar.Visibility = Visibility.Collapsed;
                BottomBar.Visibility = Visibility.Collapsed;
                WindowBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
                WindowBorder.Background = System.Windows.Media.Brushes.Transparent;
                SetClickThrough(true);
            }
            else
            {
                TitleBar.Visibility = Visibility.Visible;
                BottomBar.Visibility = Visibility.Visible;
                WindowBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 79, 172, 254)); // #4facfe
                WindowBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 20, 20, 25)); // Semi-transparent dark
                SetClickThrough(false);
            }
        }
    }
}
