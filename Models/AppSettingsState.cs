using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace IDLE.Models
{
    public class ActivityEventItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Category { get; set; } = "Flash"; // Flash, Motion, Display, Sensory
        public string Title { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public double MeasuredValue { get; set; }
        public string MeasuredValueUnit { get; set; } = "";
        public double Confidence { get; set; } = 0.95;
        public string RuleFired { get; set; } = string.Empty;
        public string ActionTaken { get; set; } = "Notified";
        public string SourceApplication { get; set; } = "Active Display";
    }

    public class AppSettingsState
    {
        // 2. Flash & Brightness
        public bool FlashDetectionEnabled { get; set; } = true;
        public bool BrightToDarkDetection { get; set; } = true;
        public bool DarkToBrightDetection { get; set; } = true;
        public bool RepeatedFlashDetection { get; set; } = true;
        public double FlashFrequencyThresholdHz { get; set; } = 3.0; // 3 Hz default
        public double FlashDurationFilterMs { get; set; } = 150.0;
        public double FlashIntensityThreshold { get; set; } = 35.0; // %
        public bool AlternatingPatternDetection { get; set; } = true;
        public bool LocalizedFlashDetection { get; set; } = true;
        public bool FullScreenFlashDetection { get; set; } = true;
        public double FlashConfidenceThreshold { get; set; } = 80.0;
        public double DetectionCooldownSeconds { get; set; } = 5.0;

        // Brightness transition & adjustment
        public bool MonitorAverageLuminance { get; set; } = true;
        public bool GradualBrightnessTransitions { get; set; } = true;
        public double PreferredBrightnessPercent { get; set; } = 70.0;
        public bool RestoreBrightnessAfterEvent { get; set; } = true;
        public bool PauseAdjustmentsGamingOrPresenting { get; set; } = true;
        public bool DimmingOverlayEnabled { get; set; } = false;
        public double DimmingOverlayOpacity { get; set; } = 25.0;
        public bool HighContrastPatternDetection { get; set; } = true;
        public string ResponseInterventionMode { get; set; } = "Detect & Explain (Ask Before Changing)";

        // 3. Motion & Vestibular
        public bool MotionAnalysisEnabled { get; set; } = true;
        public double MotionSensitivityPercent { get; set; } = 65.0;
        public bool RapidMovementDetection { get; set; } = true;
        public bool RepetitiveMotionDetection { get; set; } = true;
        public bool OpticalFlowAnalysis { get; set; } = true;
        public bool InstantScrollingPreferred { get; set; } = true;
        public bool ReduceScrollAcceleration { get; set; } = true;
        public bool PauseOnContinuousScrolling { get; set; } = false;
        public bool RespectWindowsReducedMotion { get; set; } = true;
        public bool DisableDecorativeMotion { get; set; } = true;
        public bool ReduceAnimatedTransitions { get; set; } = true;
        public bool PauseAnimatedContentRequested { get; set; } = false;

        // 4. Display & Visual Comfort
        public string InterfaceThemePreference { get; set; } = "System Default";
        public double PreferredContrastLevelPercent { get; set; } = 75.0;
        public bool ReduceExtremeBlackWhiteContrast { get; set; } = true;
        public bool WarmColorFilterGuidance { get; set; } = true;
        public double PreferredColorTemperatureK { get; set; } = 5500.0;
        public double PreferredReadingFontSizePt { get; set; } = 15.0;
        public bool ScreenDistanceReminderEnabled { get; set; } = true;
        public double BreakReminderIntervalMinutes { get; set; } = 20.0;
        public bool DarkRoomBrightnessReminder { get; set; } = true;

        // 5. Sensory Load & Overload Support
        public bool NotificationControlsEnabled { get; set; } = true;
        public bool QuietModeActive { get; set; } = false;
        public bool ReduceNotificationBanners { get; set; } = true;
        public bool SuppressNotificationsDuringFocus { get; set; } = true;
        public bool MuteIdleAlertSounds { get; set; } = true;
        public bool PreferVisualAlertOverSound { get; set; } = true;
        public bool DistractionFreeWorkspaceMode { get; set; } = false;
        public int FocusSessionDurationMinutes { get; set; } = 25;
        public string LastUserCheckInStatus { get; set; } = "Comfortable";

        // 6. Adaptive Protection & Profiles
        public string ActiveProfileName { get; set; } = "Standard";
        public bool AutoSwitchOnFullScreen { get; set; } = true;
        public bool AskBeforeProfileSwitch { get; set; } = true;

        // 7. Detection, Activity & History
        public int FrameSamplingRateFps { get; set; } = 30;
        public string FrameAnalysisResolution { get; set; } = "1280x720 (Balanced)";
        public int HistoryRetentionDays { get; set; } = 30;

        // 8. Accessibility & Interaction
        public bool ReducedAnimationsInIdle { get; set; } = false;
        public bool ConfirmBeforeMajorChanges { get; set; } = true;
        public bool KeepAlertsDismissible { get; set; } = true;

        // 9. Privacy, Security & System
        public bool LocalProcessingOnly { get; set; } = true;
        public bool NeverUploadScreenshots { get; set; } = true;
        public bool ExcludePasswordManagers { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToSystemTray { get; set; } = true;
        public double MaxCpuUtilizationTargetPercent { get; set; } = 15.0;
    }
}
