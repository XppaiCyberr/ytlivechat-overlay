using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace ytlivechatwedus
{
    public partial class CookieWindow : Window
    {
        public string Cookies { get; private set; } = "";

        public CookieWindow(string initialCookies)
        {
            InitializeComponent();
            Cookies = initialCookies;
            txtCookies.Text = initialCookies;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear your saved YouTube cookies?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                txtCookies.Clear();
                lblCheckStatus.Text = "Status: Not Checked";
                lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(143, 144, 166)); // Gray
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Cookies = txtCookies.Text.Trim();
            DialogResult = true;
            Close();
        }

        private async void BtnCheckCookies_Click(object sender, RoutedEventArgs e)
        {
            string cookieInput = txtCookies.Text.Trim();
            if (string.IsNullOrWhiteSpace(cookieInput))
            {
                lblCheckStatus.Text = "Please paste cookies first.";
                lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Red
                return;
            }

            btnCheckCookies.IsEnabled = false;
            lblCheckStatus.Text = "Connecting to YouTube & verifying session...";
            lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(79, 172, 254)); // Theme Blue

            try
            {
                var cookiesList = ParseCookieStringToList(cookieInput);
                if (cookiesList.Count == 0)
                {
                    lblCheckStatus.Text = "Failed: No valid cookies recognized in input.";
                    lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Red
                    btnCheckCookies.IsEnabled = true;
                    return;
                }

                // Format the cookies list as a semicolon-separated HTTP Cookie header
                string cookieHeaderValue = string.Join("; ", cookiesList.Select(c => $"{c.Name}={c.Value}"));

                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.youtube.com/");
                request.Headers.Add("Cookie", cookieHeaderValue);
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();
                    
                    // Check for logged in initial data in YouTube HTML
                    bool isLoggedIn = html.Contains("\"IS_LOGGED_IN\":true") || html.Contains("\"IS_LOGGED_IN\": true");
                    
                    if (isLoggedIn)
                    {
                        lblCheckStatus.Text = "✓ SUCCESS: Valid session! Successfully logged in.";
                        lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(81, 207, 102)); // Green
                    }
                    else
                    {
                        lblCheckStatus.Text = "✗ FAILED: Cookies parsed successfully, but not logged in (expired or invalid).";
                        lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Red
                    }
                }
                else
                {
                    lblCheckStatus.Text = $"✗ Network error connecting to YouTube: {(int)response.StatusCode} {response.ReasonPhrase}";
                    lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Red
                }
            }
            catch (Exception ex)
            {
                lblCheckStatus.Text = $"✗ Checker error: {ex.Message}";
                lblCheckStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Red
            }
            finally
            {
                btnCheckCookies.IsEnabled = true;
            }
        }

        private List<(string Name, string Value)> ParseCookieStringToList(string cookieInput)
        {
            var list = new List<(string, string)>();
            if (string.IsNullOrWhiteSpace(cookieInput)) return list;

            cookieInput = cookieInput.Trim();

            // 1. JSON Array format
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
                            if (element.TryGetProperty("name", out var nameProp)) name = nameProp.GetString() ?? "";
                            if (element.TryGetProperty("value", out var valProp)) value = valProp.GetString() ?? "";
                            if (element.TryGetProperty("domain", out var domProp)) domain = domProp.GetString() ?? ".youtube.com";

                            if (!string.IsNullOrEmpty(name) && (domain.Contains("youtube.com") || domain.Contains("google.com")))
                            {
                                list.Add((name, value));
                            }
                        }
                        return list;
                    }
                }
                catch { }
            }

            // 2. Netscape format
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
                        string name = parts[5];
                        string value = parts[6];

                        if (!domain.Contains("youtube.com") && !domain.Contains("google.com")) continue;

                        list.Add((name, value));
                    }
                }
                return list;
            }

            // 3. Raw standard cookie string
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
                    list.Add((name, value));
                }
            }

            return list;
        }
    }
}
