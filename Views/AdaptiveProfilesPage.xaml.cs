using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;
using IDLE.Models;

namespace IDLE.Views
{
    public sealed partial class AdaptiveProfilesPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public AdaptiveProfilesPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            AutoSwitchFullScreenCheck.IsChecked = s.AutoSwitchOnFullScreen;
            AskBeforeSwitchCheck.IsChecked = s.AskBeforeProfileSwitch;

            switch (s.ActiveProfileName)
            {
                case "Low Visual Stimulation": ProfileRadioGroup.SelectedIndex = 1; break;
                case "Flash Awareness": ProfileRadioGroup.SelectedIndex = 2; break;
                case "Vestibular Comfort": ProfileRadioGroup.SelectedIndex = 3; break;
                case "Focus": ProfileRadioGroup.SelectedIndex = 4; break;
                case "Reading": ProfileRadioGroup.SelectedIndex = 5; break;
                default: ProfileRadioGroup.SelectedIndex = 0; break;
            }
        }

        private void OnProfileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;

            string name = ProfileRadioGroup.SelectedIndex switch
            {
                1 => "Low Visual Stimulation",
                2 => "Flash Awareness",
                3 => "Vestibular Comfort",
                4 => "Focus",
                5 => "Reading",
                _ => "Standard"
            };

            _state.Settings.ActiveProfileName = name;
            _state.SaveSettings();

            _state.AddHistoryEvent(new ActivityEventItem
            {
                Category = "Adaptive Profiles",
                Title = $"Profile activated: {name}",
                Explanation = $"Applied configuration preset '{name}' to active protection filters.",
                Confidence = 1.0,
                RuleFired = "Profile Switch",
                ActionTaken = "Filters updated",
                SourceApplication = "System"
            });
        }

        private void OnPolicyCheckChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            _state.Settings.AutoSwitchOnFullScreen = AutoSwitchFullScreenCheck.IsChecked == true;
            _state.Settings.AskBeforeProfileSwitch = AskBeforeSwitchCheck.IsChecked == true;
            _state.SaveSettings();
        }
    }
}
