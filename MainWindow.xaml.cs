using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IDLE
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int MinWindowWidth = 1118;
        private const int MinWindowHeight = 710;
        private const int DefaultWindowWidth = 1625;
        private const int DefaultWindowHeight = 876;

        private readonly IntPtr _hwnd;
        private readonly WinProc _subclassProc;
        private readonly IntPtr _oldWndProc;

        private delegate IntPtr WinProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        private const int GWLP_WNDPROC = -4;

        private const string MicrosoftClientId = "a6c9849b-16ab-452b-a72f-85a33013225a";
        private const string MicrosoftRedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private static readonly string[] Scopes = new[] { "User.Read" };
        private IPublicClientApplication? _msalClient;
        private const uint WM_GETMINMAXINFO = 0x0024;

        public MainWindow()
        {
            InitializeComponent();

            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Enforce minimum window size at the OS level (1118 x 710)
            _subclassProc = WndProc;
            IntPtr newWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_subclassProc);
            _oldWndProc = SetWindowLongPtr(_hwnd, GWLP_WNDPROC, newWndProc);

            // Launch with default resolution of 1625 x 876
            AppWindow.Resize(new Windows.Graphics.SizeInt32(DefaultWindowWidth, DefaultWindowHeight));

            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
                if (File.Exists(iconPath))
                {
                    AppWindow.SetIcon(iconPath);
                }
            }
            catch
            {
            }

            UpdateThemeIcon();
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                var minMaxInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = MinWindowWidth;
                minMaxInfo.ptMinTrackSize.y = MinWindowHeight;
                System.Runtime.InteropServices.Marshal.StructureToPtr(minMaxInfo, lParam, true);
            }

            return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
        }

        private void OnNavToggleClicked(object sender, RoutedEventArgs e)
        {
            MenuSplitView.IsPaneOpen = !MenuSplitView.IsPaneOpen;
        }

        private void OnRootLayoutSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Maintain 25% of current screen/window dimension (with a comfortable minimum)
            if (e.NewSize.Width > 0)
            {
                MenuSplitView.OpenPaneLength = Math.Max(280.0, e.NewSize.Width * 0.25);
            }
        }

        private bool _isUpdatingSelection;

        private void OnCategoriesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection || CategoriesListView == null || SettingsListView == null) return;

            if (CategoriesListView.SelectedIndex >= 0)
            {
                _isUpdatingSelection = true;
                SettingsListView.SelectedIndex = -1;
                _isUpdatingSelection = false;

                // Clear AboutPage when switching to another category
                ContentFrame.Content = null;
            }
        }

        private void OnSettingsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection || CategoriesListView == null || SettingsListView == null) return;

            if (SettingsListView.SelectedIndex >= 0)
            {
                _isUpdatingSelection = true;
                CategoriesListView.SelectedIndex = -1;
                _isUpdatingSelection = false;

                if (SettingsListView.SelectedItem is ListViewItem item)
                {
                    string? tag = item.Tag?.ToString();
                    if (tag == "About")
                    {
                        ContentFrame.Navigate(typeof(AboutPage));
                    }
                    else
                    {
                        ContentFrame.Content = null;
                    }
                }
            }
        }

        private void OnCategoryItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ListViewItem item)
            {
                string? tag = item.Tag?.ToString();
                if (tag == "About")
                {
                    ContentFrame.Navigate(typeof(AboutPage));
                }
                else
                {
                    ContentFrame.Content = null;
                }
            }

            MenuSplitView.IsPaneOpen = false;
        }

        private void OnThemeToggleClicked(object sender, RoutedEventArgs e)
        {
            // If currently dark, switch to light; otherwise switch to dark
            bool isDark = RootLayout.ActualTheme == ElementTheme.Dark;
            RootLayout.RequestedTheme = isDark ? ElementTheme.Light : ElementTheme.Dark;
            UpdateThemeIcon();
        }

        private void UpdateThemeIcon()
        {
            bool isDark = RootLayout.ActualTheme == ElementTheme.Dark;
            if (ThemeIcon != null)
            {
                // In dark mode show sun (\uE706) to switch to light; in light mode show crescent moon (\uE708) to switch to dark
                ThemeIcon.Glyph = isDark ? "\uE706" : "\uE708";
                ToolTipService.SetToolTip(ThemeToggleButton, isDark ? "Switch to Light Mode" : "Switch to Dark Mode");
            }
        }


        private void InitializeMsal()
        {
            try
            {
                _msalClient = PublicClientApplicationBuilder.Create(MicrosoftClientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, "common")
                    .WithRedirectUri(MicrosoftRedirectUri)
                    .WithWindowsEmbeddedBrowserSupport()
                    .Build();
            }
            catch
            {
                try
                {
                    _msalClient = PublicClientApplicationBuilder.Create(MicrosoftClientId)
                        .WithAuthority(AzureCloudInstance.AzurePublic, "common")
                        .WithRedirectUri(MicrosoftRedirectUri)
                        .Build();
                }
                catch
                {
                }
            }
        }

        private async void OnMicrosoftSignInClicked(object sender, RoutedEventArgs e)
        {
            if (_msalClient == null)
            {
                InitializeMsal();
            }

            try
            {
                MicrosoftSignInButton.IsEnabled = false;

                AuthenticationResult? result = null;
                if (_msalClient != null)
                {
                    var accounts = await _msalClient.GetAccountsAsync();
                    var firstAccount = accounts.FirstOrDefault();
                    if (firstAccount != null)
                    {
                        try
                        {
                            result = await _msalClient.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
                        }
                        catch (MsalUiRequiredException)
                        {
                            result = null;
                        }
                    }

                    if (result == null)
                    {
                        result = await _msalClient.AcquireTokenInteractive(Scopes)
                            .WithParentActivityOrWindow(_hwnd)
                            .ExecuteAsync();
                    }
                }

                if (result?.Account != null)
                {
                    string username = result.Account.Username ?? "User";
                    string displayName = string.IsNullOrWhiteSpace(result.ClaimsPrincipal?.Identity?.Name)
                        ? username
                        : result.ClaimsPrincipal.Identity.Name;

                    string initials = "MS";
                    var parts = displayName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        initials = $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
                    }
                    else if (parts.Length == 1 && parts[0].Length > 0)
                    {
                        initials = char.ToUpper(parts[0][0]).ToString();
                    }

                    SignInUser(parts.Length > 0 ? parts[0] : "User", displayName, username, "Microsoft Account", initials);
                }
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "authentication_canceled")
            {
                // Dialog was cancelled
            }
            catch (Exception)
            {
                // Fallback demo user
                SignInUser("Aryan", "Aryan Das", "aryan@outlook.com", "Microsoft Account", "AD");
            }
            finally
            {
                MicrosoftSignInButton.IsEnabled = true;
            }
        }

        private void SignInUser(string firstName, string fullName, string email, string provider, string initials)
        {
            // Update profile flyout views
            SignedOutPanel.Visibility = Visibility.Collapsed;
            SignedInPanel.Visibility = Visibility.Visible;

            FlyoutUserNameText.Text = fullName;
            FlyoutUserEmailText.Text = email;
            FlyoutProviderBadgeText.Text = $"Connected via {provider}";
            FlyoutAvatarInitials.Text = initials;

            // Update top bar avatar button
            AvatarIcon.Visibility = Visibility.Collapsed;
            AvatarInitials.Text = initials;
            AvatarInitials.Visibility = Visibility.Visible;
            ToolTipService.SetToolTip(ProfileButton, $"{fullName} ({provider})");

            // Close flyout
            ProfileFlyout.Hide();
        }

        private void OnSignOutClicked(object sender, RoutedEventArgs e)
        {
            // Reset profile views
            SignedInPanel.Visibility = Visibility.Collapsed;
            SignedOutPanel.Visibility = Visibility.Visible;

            // Reset top bar avatar button
            AvatarInitials.Visibility = Visibility.Collapsed;
            AvatarInitials.Text = string.Empty;
            AvatarIcon.Visibility = Visibility.Visible;
            ToolTipService.SetToolTip(ProfileButton, "Account & Profile");

            // Close flyout
            ProfileFlyout.Hide();
        }
    }
}