using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;
using IDLE.Models;

namespace IDLE.Views
{
    public sealed partial class SensoryLoadPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public SensoryLoadPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            QuietModeToggle.IsOn = s.QuietModeActive;
            ReduceBannersCheck.IsChecked = s.ReduceNotificationBanners;
            SuppressFocusCheck.IsChecked = s.SuppressNotificationsDuringFocus;
            MuteSoundsCheck.IsChecked = s.MuteIdleAlertSounds;
            VisualAlertCheck.IsChecked = s.PreferVisualAlertOverSound;
            DistractionFreeCheck.IsChecked = s.DistractionFreeWorkspaceMode;

            FocusDurationSlider.Value = s.FocusSessionDurationMinutes;
            FocusDurationLabel.Text = $"{s.FocusSessionDurationMinutes} min";

            CheckInStatusText.Text = $"Current status: {s.LastUserCheckInStatus}";
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.QuietModeActive = QuietModeToggle.IsOn;
            s.ReduceNotificationBanners = ReduceBannersCheck.IsChecked == true;
            s.SuppressNotificationsDuringFocus = SuppressFocusCheck.IsChecked == true;
            s.MuteIdleAlertSounds = MuteSoundsCheck.IsChecked == true;
            s.PreferVisualAlertOverSound = VisualAlertCheck.IsChecked == true;
            s.DistractionFreeWorkspaceMode = DistractionFreeCheck.IsChecked == true;

            s.FocusSessionDurationMinutes = (int)FocusDurationSlider.Value;
            FocusDurationLabel.Text = $"{s.FocusSessionDurationMinutes} min";

            _state.SaveSettings();
        }

        private void OnStartFocusClicked(object sender, RoutedEventArgs e)
        {
            QuietModeToggle.IsOn = true;
            _state.AddHistoryEvent(new ActivityEventItem
            {
                Category = "Sensory Load",
                Title = "Focus Session Activated",
                Explanation = $"Activated {FocusDurationSlider.Value:0}-minute calm focus profile. Non-essential notifications suppressed.",
                Confidence = 1.0,
                RuleFired = "User Focus Request",
                ActionTaken = "Quiet mode initiated",
                SourceApplication = "IDLE Environment"
            });
        }

        private void OnCheckInButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content != null)
            {
                string status = btn.Content.ToString() ?? "Comfortable";
                _state.Settings.LastUserCheckInStatus = status;
                CheckInStatusText.Text = $"Current status: {status}";
                _state.SaveSettings();

                _state.AddHistoryEvent(new ActivityEventItem
                {
                    Category = "Sensory Check-In",
                    Title = $"User reported: {status}",
                    Explanation = "Private local self-report logged to correlate with ambient comfort settings.",
                    Confidence = 1.0,
                    RuleFired = "Self Check-In",
                    ActionTaken = "Logged locally",
                    SourceApplication = "User Profile"
                });
            }
        }
    }
}
