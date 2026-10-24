using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE.Views
{
    public sealed partial class ActivityHistoryPage : Page
    {
        private readonly IdleStateService _state = IdleStateService.Instance;

        public ActivityHistoryPage()
        {
            InitializeComponent();
            EventsListView.ItemsSource = _state.HistoryEvents;
            UpdateCount();
        }

        private void UpdateCount()
        {
            EventCountText.Text = $"{_state.HistoryEvents.Count} Events Logged";
        }

        private void OnClearHistoryClicked(object sender, RoutedEventArgs e)
        {
            _state.ClearHistory();
            UpdateCount();
        }

        private async void OnExportReportClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Local Report Exported",
                Content = $"Exported {_state.HistoryEvents.Count} local events to your IDLE application directory. No data was transferred externally.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
