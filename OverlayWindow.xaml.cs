using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;

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
        private string? _pendingUrl = null;
        public string? Cookies { get; set; } = "";

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
                webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
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

                // If there is a pending URL, load it now!
                if (!string.IsNullOrEmpty(_pendingUrl))
                {
                    LoadChat(_pendingUrl);
                    _pendingUrl = null;
                }
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
            if (!_isInitialized)
            {
                _pendingUrl = videoIdOrUrl;
                return;
            }

            string? videoId = ExtractVideoId(videoIdOrUrl);
            if (string.IsNullOrEmpty(videoId))
            {
                MessageBox.Show("Invalid YouTube URL or Video ID.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Inject YouTube cookies if configured
            ApplyCookies();

            string chatUrl = $"https://www.youtube.com/live_chat?v={videoId}";
            
            // Bypass WPF DependencyProperty change check when reloading the identical Uri
            var targetUri = new Uri(chatUrl);
            if (webView.Source == targetUri)
            {
                webView.CoreWebView2?.Navigate(chatUrl);
            }
            else
            {
                webView.Source = targetUri;
            }
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

            // Append deleted chat styling so it applies across all themes seamlessly
            cleanedCss += @"
                .overlay-deleted-chat {
                    background-color: rgba(220, 53, 69, 0.18) !important;
                    border-left: 4px solid #ff4b4b !important;
                    opacity: 0.9 !important;
                    transition: background-color 0.3s ease;
                }
                .overlay-deleted-chat #message::before {
                    content: '[DELETED] ' !important;
                    color: #ff4b4b !important;
                    font-weight: 800 !important;
                    font-size: 0.9em !important;
                    text-transform: uppercase;
                }
            ";

            string js = $@"
                (function() {{
                    // 1. Inject Styles
                    let style = document.getElementById('custom-overlay-style');
                    if (!style) {{
                        style = document.createElement('style');
                        style.id = 'custom-overlay-style';
                        document.head.appendChild(style);
                    }}
                    style.textContent = `{cleanedCss}`;

                    // 2. Start Deleted Message Observer (if not already running)
                    if (!window.hasDeletedMessageObserver) {{
                        window.hasDeletedMessageObserver = true;
                        const messageCache = new Map();
                        
                        const observer = new MutationObserver((mutations) => {{
                            for (const mutation of mutations) {{
                                // Handle node removals (moderator deletion)
                                if (mutation.removedNodes.length > 0) {{
                                    for (const node of mutation.removedNodes) {{
                                        if (node.nodeType === Node.ELEMENT_NODE && 
                                            (node.tagName === 'YT-LIVE-CHAT-TEXT-MESSAGE-RENDERER' || 
                                             node.tagName === 'YT-LIVE-CHAT-PAID-MESSAGE-RENDERER')) {{
                                            
                                            const id = node.getAttribute('id');
                                            if (id && messageCache.has(id)) {{
                                                const restoredNode = node.cloneNode(true);
                                                restoredNode.classList.add('overlay-deleted-chat');
                                                restoredNode.setAttribute('data-deleted', 'true');
                                                
                                                const parent = mutation.target;
                                                const nextSibling = mutation.nextSibling;
                                                
                                                if (nextSibling && nextSibling.parentNode === parent) {{
                                                    parent.insertBefore(restoredNode, nextSibling);
                                                }} else {{
                                                    parent.appendChild(restoredNode);
                                                }}
                                            }}
                                        }}
                                    }}
                                }}
                                
                                // Cache new nodes as they are added
                                if (mutation.addedNodes.length > 0) {{
                                    for (const node of mutation.addedNodes) {{
                                        if (node.nodeType === Node.ELEMENT_NODE && 
                                            (node.tagName === 'YT-LIVE-CHAT-TEXT-MESSAGE-RENDERER' || 
                                             node.tagName === 'YT-LIVE-CHAT-PAID-MESSAGE-RENDERER')) {{
                                            
                                            const id = node.getAttribute('id');
                                            if (id) {{
                                                messageCache.set(id, node.innerHTML);
                                            }}
                                        }}
                                    }}
                                }}
                                
                                // Handle inline text replacements (YouTube self-editing/deletion)
                                if (mutation.type === 'characterData' || mutation.type === 'childList') {{
                                    const target = mutation.target;
                                    if (target.nodeType === Node.TEXT_NODE) {{
                                        const parent = target.parentElement;
                                        if (parent && (parent.id === 'message' || parent.classList.contains('message'))) {{
                                            const renderer = parent.closest('yt-live-chat-text-message-renderer, yt-live-chat-paid-message-renderer');
                                            if (renderer) {{
                                                const text = target.textContent || '';
                                                if (text.toLowerCase().includes('message deleted') || text.toLowerCase().includes('deleted')) {{
                                                    const id = renderer.getAttribute('id');
                                                    if (id && messageCache.has(id)) {{
                                                        renderer.classList.add('overlay-deleted-chat');
                                                        renderer.setAttribute('data-deleted', 'true');
                                                        renderer.innerHTML = messageCache.get(id);
                                                    }}
                                                }}
                                            }}
                                        }}
                                    }}
                                }}
                            }}
                        }});
                        
                        const startObserving = () => {{
                            const chatItems = document.querySelector('yt-live-chat-item-list-renderer #items');
                            if (chatItems) {{
                                observer.observe(chatItems, {{ 
                                    childList: true, 
                                    subtree: true,
                                    characterData: true
                                }});
                                
                                // Cache pre-existing messages
                                const existing = chatItems.querySelectorAll('yt-live-chat-text-message-renderer, yt-live-chat-paid-message-renderer');
                                for (const node of existing) {{
                                    const id = node.getAttribute('id');
                                    if (id) {{
                                        messageCache.set(id, node.innerHTML);
                                    }}
                                }}
                            }} else {{
                                setTimeout(startObserving, 500);
                            }}
                        }};
                        
                        startObserving();
                    }}
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

        private void ApplyCookies()
        {
            if (webView.CoreWebView2 == null || string.IsNullOrWhiteSpace(Cookies)) return;

            var cookieManager = webView.CoreWebView2.CookieManager;
            cookieManager.DeleteAllCookies();

            string cookieInput = Cookies.Trim();

            // 1. JSON Array format (e.g. from standard extensions)
            if (cookieInput.StartsWith("[") && cookieInput.EndsWith("]"))
            {
                try
                {
                    using var doc = JsonDocument.Parse(cookieInput);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in doc.RootElement.EnumerateArray())
                        {
                            string name = "";
                            string value = "";
                            string domain = ".youtube.com";
                            string path = "/";
                            bool isSecure = false;

                            if (element.TryGetProperty("name", out var nameProp)) name = nameProp.GetString() ?? "";
                            if (element.TryGetProperty("value", out var valProp)) value = valProp.GetString() ?? "";
                            if (element.TryGetProperty("domain", out var domProp)) domain = domProp.GetString() ?? ".youtube.com";
                            if (element.TryGetProperty("path", out var pathProp)) path = pathProp.GetString() ?? "/";
                            if (element.TryGetProperty("secure", out var secProp)) isSecure = secProp.GetBoolean();

                            if (string.IsNullOrEmpty(name)) continue;

                            // Secure/limit domain to YouTube and Google
                            if (!domain.Contains("youtube.com") && !domain.Contains("google.com")) continue;

                            var cookie = cookieManager.CreateCookie(name, value, domain, path);
                            cookie.IsSecure = isSecure;
                            cookieManager.AddOrUpdateCookie(cookie);
                        }
                        return;
                    }
                }
                catch
                {
                    // Fall back if JSON parsing fails
                }
            }

            // 2. Netscape format (tab-separated cookies.txt)
            var lines = cookieInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            bool isNetscape = false;
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("#") || line.Contains("\t"))
                {
                    isNetscape = true;
                    break;
                }
            }

            if (isNetscape)
            {
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("#") || string.IsNullOrEmpty(trimmed)) continue;

                    var parts = trimmed.Split('\t');
                    if (parts.Length >= 7)
                    {
                        string domain = parts[0];
                        string path = parts[2];
                        bool isSecure = parts[3].Equals("TRUE", StringComparison.OrdinalIgnoreCase);
                        string name = parts[5];
                        string value = parts[6];

                        if (!domain.Contains("youtube.com") && !domain.Contains("google.com")) continue;

                        var cookie = cookieManager.CreateCookie(name, value, domain, path);
                        cookie.IsSecure = isSecure;
                        cookieManager.AddOrUpdateCookie(cookie);
                    }
                }
                return;
            }

            // 3. Raw standard cookie string (from document.cookie)
            var cookiePairs = cookieInput.Split(';');
            foreach (var pair in cookiePairs)
            {
                var trimmedPair = pair.Trim();
                if (string.IsNullOrEmpty(trimmedPair)) continue;

                int eqIndex = trimmedPair.IndexOf('=');
                if (eqIndex > 0)
                {
                    string name = trimmedPair.Substring(0, eqIndex).Trim();
                    string value = trimmedPair.Substring(eqIndex + 1).Trim();

                    var cookie = cookieManager.CreateCookie(name, value, ".youtube.com", "/");
                    if (name.StartsWith("__Secure-"))
                    {
                        cookie.IsSecure = true;
                    }
                    cookieManager.AddOrUpdateCookie(cookie);
                }
            }
        }

        public void SetOverlayMode(bool isLocked)
        {
            _isClickThrough = isLocked;
            
            if (isLocked)
            {
                TitleBar.Visibility = Visibility.Collapsed;
                BottomGrip.Visibility = Visibility.Collapsed;
                WindowBorder.BorderBrush = System.Windows.Media.Brushes.Transparent;
                WindowBorder.Background = System.Windows.Media.Brushes.Transparent;
                SetClickThrough(true);
            }
            else
            {
                TitleBar.Visibility = Visibility.Visible;
                BottomGrip.Visibility = Visibility.Visible;
                WindowBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 79, 172, 254)); // #4facfe
                WindowBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 20, 20, 25)); // Semi-transparent dark
                SetClickThrough(false);
            }
        }
    }
}
