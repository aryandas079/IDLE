using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE.Views
{
    public sealed partial class SettingsPreferencesPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public SettingsPreferencesPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            ReducedAnimationsCheck.IsChecked = s.ReducedAnimationsInIdle;
            ConfirmChangesCheck.IsChecked = s.ConfirmBeforeMajorChanges;
            DismissibleAlertsCheck.IsChecked = s.KeepAlertsDismissible;

            FpsSlider.Value = s.FrameSamplingRateFps;
            FpsLabel.Text = $"{s.FrameSamplingRateFps} FPS";

            RetentionSlider.Value = s.HistoryRetentionDays;
            RetentionLabel.Text = $"{s.HistoryRetentionDays} days";
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.ReducedAnimationsInIdle = ReducedAnimationsCheck.IsChecked == true;
            s.ConfirmBeforeMajorChanges = ConfirmChangesCheck.IsChecked == true;
            s.KeepAlertsDismissible = DismissibleAlertsCheck.IsChecked == true;

            s.FrameSamplingRateFps = (int)FpsSlider.Value;
            FpsLabel.Text = $"{s.FrameSamplingRateFps} FPS";

            s.HistoryRetentionDays = (int)RetentionSlider.Value;
            RetentionLabel.Text = $"{s.HistoryRetentionDays} days";

            _state.SaveSettings();
        }
    }
}
