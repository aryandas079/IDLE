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
        private const uint WM_GETMINMAXINFO = 0x0024;

        public MainWindow()
        {
            InitializeComponent();

            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Enforce minimum window size at the OS level
            _subclassProc = WndProc;
            IntPtr newWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_subclassProc);
            _oldWndProc = SetWindowLongPtr(_hwnd, GWLP_WNDPROC, newWndProc);

            // Resize window initially to at least minimum dimensions
            AppWindow.Resize(new Windows.Graphics.SizeInt32(MinWindowWidth, MinWindowHeight));

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

            // Default navigate to AboutPage
            ContentFrame.Navigate(typeof(AboutPage));
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
            }

            MenuSplitView.IsPaneOpen = false;
        }
    }
}
