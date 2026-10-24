using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE.Views
{
    public sealed partial class DisplayComfortPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public DisplayComfortPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            ReduceExtremeContrastCheck.IsChecked = s.ReduceExtremeBlackWhiteContrast;
            WarmColorFilterCheck.IsChecked = s.WarmColorFilterGuidance;

            ColorTempSlider.Value = s.PreferredColorTemperatureK;
            ColorTempLabel.Text = $"{s.PreferredColorTemperatureK:0} K";

            ContrastSlider.Value = s.PreferredContrastLevelPercent;
            ContrastLabel.Text = $"{s.PreferredContrastLevelPercent:0}%";

            FontSizeSlider.Value = s.PreferredReadingFontSizePt;
            FontSizeLabel.Text = $"{s.PreferredReadingFontSizePt:0} pt";

            DarkRoomReminderCheck.IsChecked = s.DarkRoomBrightnessReminder;
            ScreenDistanceCheck.IsChecked = s.ScreenDistanceReminderEnabled;

            BreakIntervalSlider.Value = s.BreakReminderIntervalMinutes;
            BreakIntervalLabel.Text = $"{s.BreakReminderIntervalMinutes:0} min";
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.ReduceExtremeBlackWhiteContrast = ReduceExtremeContrastCheck.IsChecked == true;
            s.WarmColorFilterGuidance = WarmColorFilterCheck.IsChecked == true;

            s.PreferredColorTemperatureK = ColorTempSlider.Value;
            ColorTempLabel.Text = $"{s.PreferredColorTemperatureK:0} K";

            s.PreferredContrastLevelPercent = ContrastSlider.Value;
            ContrastLabel.Text = $"{s.PreferredContrastLevelPercent:0}%";

            s.PreferredReadingFontSizePt = FontSizeSlider.Value;
            FontSizeLabel.Text = $"{s.PreferredReadingFontSizePt:0} pt";

            s.DarkRoomBrightnessReminder = DarkRoomReminderCheck.IsChecked == true;
            s.ScreenDistanceReminderEnabled = ScreenDistanceCheck.IsChecked == true;

            s.BreakReminderIntervalMinutes = BreakIntervalSlider.Value;
            BreakIntervalLabel.Text = $"{s.BreakReminderIntervalMinutes:0} min";

            _state.SaveSettings();
        }
    }
}
