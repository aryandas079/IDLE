using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;
using IDLE.Models;

namespace IDLE.Views
{
    public sealed partial class FlashBrightnessPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public FlashBrightnessPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            FlashDetectionToggle.IsOn = s.FlashDetectionEnabled;
            BrightToDarkCheck.IsChecked = s.BrightToDarkDetection;
            DarkToBrightCheck.IsChecked = s.DarkToBrightDetection;
            RepeatedFlashCheck.IsChecked = s.RepeatedFlashDetection;
            AlternatingPatternCheck.IsChecked = s.AlternatingPatternDetection;
            LocalizedFlashCheck.IsChecked = s.LocalizedFlashDetection;
            FullScreenFlashCheck.IsChecked = s.FullScreenFlashDetection;

            FrequencySlider.Value = s.FlashFrequencyThresholdHz;
            FrequencyLabel.Text = $"{s.FlashFrequencyThresholdHz:0.0} Hz";

            IntensitySlider.Value = s.FlashIntensityThreshold;
            IntensityLabel.Text = $"{s.FlashIntensityThreshold:0}%";

            DurationSlider.Value = s.FlashDurationFilterMs;
            DurationLabel.Text = $"{s.FlashDurationFilterMs:0} ms";

            CooldownSlider.Value = s.DetectionCooldownSeconds;
            CooldownLabel.Text = $"{s.DetectionCooldownSeconds:0} s";

            BrightnessSlider.Value = s.PreferredBrightnessPercent;
            BrightnessLabel.Text = $"{s.PreferredBrightnessPercent:0}%";

            GradualBrightnessCheck.IsChecked = s.GradualBrightnessTransitions;
            RestoreBrightnessCheck.IsChecked = s.RestoreBrightnessAfterEvent;
            PauseGamingCheck.IsChecked = s.PauseAdjustmentsGamingOrPresenting;

            DimmingOverlayToggle.IsOn = s.DimmingOverlayEnabled;
            OverlayOpacitySlider.Value = s.DimmingOverlayOpacity;
            OverlayOpacityLabel.Text = $"{s.DimmingOverlayOpacity:0}%";

            HighContrastPatternCheck.IsChecked = s.HighContrastPatternDetection;

            // Set Combo
            if (s.ResponseInterventionMode.Contains("Reversible")) ResponseModeCombo.SelectedIndex = 2;
            else if (s.ResponseInterventionMode.Contains("Notification Only")) ResponseModeCombo.SelectedIndex = 1;
            else if (s.ResponseInterventionMode.Contains("History Only")) ResponseModeCombo.SelectedIndex = 3;
            else ResponseModeCombo.SelectedIndex = 0;
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.FlashDetectionEnabled = FlashDetectionToggle.IsOn;
            s.BrightToDarkDetection = BrightToDarkCheck.IsChecked == true;
            s.DarkToBrightDetection = DarkToBrightCheck.IsChecked == true;
            s.RepeatedFlashDetection = RepeatedFlashCheck.IsChecked == true;
            s.AlternatingPatternDetection = AlternatingPatternCheck.IsChecked == true;
            s.LocalizedFlashDetection = LocalizedFlashCheck.IsChecked == true;
            s.FullScreenFlashDetection = FullScreenFlashCheck.IsChecked == true;

            s.FlashFrequencyThresholdHz = FrequencySlider.Value;
            FrequencyLabel.Text = $"{s.FlashFrequencyThresholdHz:0.0} Hz";

            s.FlashIntensityThreshold = IntensitySlider.Value;
            IntensityLabel.Text = $"{s.FlashIntensityThreshold:0}%";

            s.FlashDurationFilterMs = DurationSlider.Value;
            DurationLabel.Text = $"{s.FlashDurationFilterMs:0} ms";

            s.DetectionCooldownSeconds = CooldownSlider.Value;
            CooldownLabel.Text = $"{s.DetectionCooldownSeconds:0} s";

            s.PreferredBrightnessPercent = BrightnessSlider.Value;
            BrightnessLabel.Text = $"{s.PreferredBrightnessPercent:0}%";

            s.GradualBrightnessTransitions = GradualBrightnessCheck.IsChecked == true;
            s.RestoreBrightnessAfterEvent = RestoreBrightnessCheck.IsChecked == true;
            s.PauseAdjustmentsGamingOrPresenting = PauseGamingCheck.IsChecked == true;

            s.DimmingOverlayEnabled = DimmingOverlayToggle.IsOn;
            s.DimmingOverlayOpacity = OverlayOpacitySlider.Value;
            OverlayOpacityLabel.Text = $"{s.DimmingOverlayOpacity:0}%";

            s.HighContrastPatternDetection = HighContrastPatternCheck.IsChecked == true;

            _state.SaveSettings();
        }

        private void OnResponseModeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;
            if (ResponseModeCombo.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                _state.Settings.ResponseInterventionMode = item.Content.ToString() ?? "";
                _state.SaveSettings();
            }
        }

        private void OnSimulateFlashClicked(object sender, RoutedEventArgs e)
        {
            _state.AddHistoryEvent(new ActivityEventItem
            {
                Category = "Flash Detection",
                Title = "Simulated Flash Event Test",
                Explanation = $"Evaluated test frame sequence: {FrequencySlider.Value:0.0} Hz oscillation over {IntensitySlider.Value:0}% magnitude threshold.",
                MeasuredValue = FrequencySlider.Value,
                MeasuredValueUnit = "Hz",
                Confidence = 0.99,
                RuleFired = "Engineering Flash Diagnostic Rule",
                ActionTaken = "Recorded to local activity history",
                SourceApplication = "Test Pattern Suite"
            });
        }

        private void OnResetDefaultsClicked(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;
            _state.Settings.FlashFrequencyThresholdHz = 3.0;
            _state.Settings.FlashIntensityThreshold = 35.0;
            _state.Settings.FlashDurationFilterMs = 150.0;
            _state.Settings.DetectionCooldownSeconds = 5.0;
            _state.Settings.PreferredBrightnessPercent = 70.0;
            _state.SaveSettings();
            LoadFromState();
            _isInitializing = false;
        }
    }
}
