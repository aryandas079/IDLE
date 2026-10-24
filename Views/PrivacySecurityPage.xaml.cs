using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE.Views
{
    public sealed partial class PrivacySecurityPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public PrivacySecurityPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            LocalProcessingCheck.IsChecked = s.LocalProcessingOnly;
            NeverUploadCheck.IsChecked = s.NeverUploadScreenshots;
            ExcludePasswordManagersCheck.IsChecked = s.ExcludePasswordManagers;
            StartWithWindowsCheck.IsChecked = s.StartWithWindows;
            MinimizeTrayCheck.IsChecked = s.MinimizeToSystemTray;

            CpuLimitSlider.Value = s.MaxCpuUtilizationTargetPercent;
            CpuLimitLabel.Text = $"{s.MaxCpuUtilizationTargetPercent:0}% target";
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.LocalProcessingOnly = LocalProcessingCheck.IsChecked == true;
            s.NeverUploadScreenshots = NeverUploadCheck.IsChecked == true;
            s.ExcludePasswordManagers = ExcludePasswordManagersCheck.IsChecked == true;
            s.StartWithWindows = StartWithWindowsCheck.IsChecked == true;
            s.MinimizeToSystemTray = MinimizeTrayCheck.IsChecked == true;

            s.MaxCpuUtilizationTargetPercent = CpuLimitSlider.Value;
            CpuLimitLabel.Text = $"{s.MaxCpuUtilizationTargetPercent:0}% target";

            _state.SaveSettings();
        }
    }
}
