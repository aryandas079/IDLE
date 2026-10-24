using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;
using IDLE.Models;

namespace IDLE.Views
{
    public sealed partial class MotionVestibularPage : Page
    {
        private bool _isInitializing = true;
        private readonly IdleStateService _state = IdleStateService.Instance;

        public MotionVestibularPage()
        {
            InitializeComponent();
            LoadFromState();
            _isInitializing = false;
        }

        private void LoadFromState()
        {
            var s = _state.Settings;
            MotionAnalysisToggle.IsOn = s.MotionAnalysisEnabled;
            MotionSensitivitySlider.Value = s.MotionSensitivityPercent;
            MotionSensitivityLabel.Text = $"{s.MotionSensitivityPercent:0}%";

            RapidMovementCheck.IsChecked = s.RapidMovementDetection;
            RepetitiveMotionCheck.IsChecked = s.RepetitiveMotionDetection;
            OpticalFlowCheck.IsChecked = s.OpticalFlowAnalysis;

            InstantScrollingCheck.IsChecked = s.InstantScrollingPreferred;
            ReduceScrollAccelerationCheck.IsChecked = s.ReduceScrollAcceleration;
            PauseContinuousScrollCheck.IsChecked = s.PauseOnContinuousScrolling;

            RespectWindowsMotionCheck.IsChecked = s.RespectWindowsReducedMotion;
            DisableDecorativeMotionCheck.IsChecked = s.DisableDecorativeMotion;
            ReduceAnimatedTransitionsCheck.IsChecked = s.ReduceAnimatedTransitions;
        }

        private void OnSettingChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;

            var s = _state.Settings;
            s.MotionAnalysisEnabled = MotionAnalysisToggle.IsOn;
            s.MotionSensitivityPercent = MotionSensitivitySlider.Value;
            MotionSensitivityLabel.Text = $"{s.MotionSensitivityPercent:0}%";

            s.RapidMovementDetection = RapidMovementCheck.IsChecked == true;
            s.RepetitiveMotionDetection = RepetitiveMotionCheck.IsChecked == true;
            s.OpticalFlowAnalysis = OpticalFlowCheck.IsChecked == true;

            s.InstantScrollingPreferred = InstantScrollingCheck.IsChecked == true;
            s.ReduceScrollAcceleration = ReduceScrollAccelerationCheck.IsChecked == true;
            s.PauseOnContinuousScrolling = PauseContinuousScrollCheck.IsChecked == true;

            s.RespectWindowsReducedMotion = RespectWindowsMotionCheck.IsChecked == true;
            s.DisableDecorativeMotion = DisableDecorativeMotionCheck.IsChecked == true;
            s.ReduceAnimatedTransitions = ReduceAnimatedTransitionsCheck.IsChecked == true;

            _state.SaveSettings();
        }

        private void OnTestMotionEventClicked(object sender, RoutedEventArgs e)
        {
            _state.AddHistoryEvent(new ActivityEventItem
            {
                Category = "Motion & Vestibular",
                Title = "High-velocity parallax scrolling identified",
                Explanation = "Visual optical flow vectors indicated rapid multi-layer parallax offset exceeding 45% screen area.",
                MeasuredValue = MotionSensitivitySlider.Value,
                MeasuredValueUnit = "% motion",
                Confidence = 0.92,
                RuleFired = "Optical Flow Parallax Rule",
                ActionTaken = "Instant scroll damping recommended",
                SourceApplication = "Active Browser Window"
            });
        }
    }
}
