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
        public MainWindow()
        {
            InitializeComponent();
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
            if (_isUpdatingSelection) return;

            if (CategoriesListView.SelectedIndex >= 0)
            {
                _isUpdatingSelection = true;
                SettingsListView.SelectedIndex = -1;
                _isUpdatingSelection = false;
            }
        }

        private void OnSettingsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection) return;

            if (SettingsListView.SelectedIndex >= 0)
            {
                _isUpdatingSelection = true;
                CategoriesListView.SelectedIndex = -1;
                _isUpdatingSelection = false;
            }
        }

        private void OnCategoryItemClick(object sender, ItemClickEventArgs e)
        {
            MenuSplitView.IsPaneOpen = false;
        }
    }
}
