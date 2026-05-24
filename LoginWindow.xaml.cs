using System;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace ytlivechatwedus
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync(null, null, null);
                await webView.EnsureCoreWebView2Async(env);

                // Subscribe to SourceChanged to detect successful login redirects
                webView.SourceChanged += WebView_SourceChanged;

                // Set standard browser User-Agent to bypass Google login blocking in embedded views
                webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

                // Enable standard browser settings for interactive login
                webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;

                // Navigate to YouTube Login page
                string loginUrl = "https://accounts.google.com/ServiceLogin?service=youtube&hl=en&continue=https%3A%2F%2Fwww.youtube.com%2Fsignin%3Faction_handle_signin%3Dtrue%26next%3D%252F";
                webView.Source = new Uri(loginUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize Login WebView2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WebView_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (webView.Source == null) return;
            string url = webView.Source.ToString();

            // Detect redirection back to YouTube after successful sign-in completes
            if (url.Contains("youtube.com") && 
                !url.Contains("accounts.google.com") && 
                !url.Contains("signin") &&
                (webView.Source.AbsolutePath == "/" || url.Contains("action_handle_signin=true")))
            {
                // Sign-in completed successfully, automatically close the dialog!
                DialogResult = true;
                Close();
            }
        }
    }
}
