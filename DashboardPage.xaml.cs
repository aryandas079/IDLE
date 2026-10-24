using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE
{
    public sealed partial class DashboardPage : Page
    {
        private readonly IdleStateService _state = IdleStateService.Instance;
        private readonly SystemControlService _systemService = SystemControlService.Instance;
        private DispatcherTimer? _liveTelemetryTimer;

        private int _filterIndex = 0;
        private readonly string[] _filters = new[] { "Real-time", "Daily", "Weekly" };
        private double _comfortIndexScore = 98.2;

        public DashboardPage()
        {
            InitializeComponent();
            LoadState();
            StartLiveSystemTelemetry();
        }

        private void LoadState()
        {
            ActiveProfileHeaderBadge.Text = $"{_state.Settings.ActiveProfileName} Profile";
            SignalSensitivityText.Text = $"{_state.Settings.FlashFrequencyThresholdHz:0.0} Hz";

            if (_state.HistoryEvents.Count > 0)
            {
                var latest = _state.HistoryEvents[0];
                LatestEventSummaryText.Text = $"{latest.Title} · {latest.Confidence * 100:0}% confidence";
            }
            else
            {
                LatestEventSummaryText.Text = "Zero active detections · Baseline stable";
            }
        }

        private void StartLiveSystemTelemetry()
        {
            // Update live telemetry every 1000ms with real hardware and screen pixels
            _liveTelemetryTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _liveTelemetryTimer.Tick += (s, e) => UpdateLiveTelemetry();
            _liveTelemetryTimer.Start();
            UpdateLiveTelemetry();
        }

        private void UpdateLiveTelemetry()
        {
            var snap = _systemService.CaptureLiveSnapshot();

            // Frames Analyzed: Real stream counter
            FramesAnalyzedCountText.Text = $"{snap.TotalFramesSampled:N0}";

            // If in Real-time mode, map real screen pixel luminance & motion directly onto the histogram bars
            if (_filterIndex == 0)
            {
                // Luminance bar height: mapped to current real screen average luminance
                double lumTotal = Math.Clamp(snap.AverageScreenLuminancePercent * 1.5, 30.0, 135.0);
                double lumMint = Math.Clamp(lumTotal * 0.45, 15.0, 60.0);
                LuminanceBarLavender.Height = lumTotal;
                LuminanceBarMint.Height = lumMint;

                // Motion bar height: mapped to real inter-frame luminance variance
                double motTotal = Math.Clamp(40.0 + (snap.ScreenMotionDeltaPercent * 3.5), 40.0, 155.0);
                double motMint = Math.Clamp(motTotal * 0.65, 20.0, 95.0);
                MotionBarLavender.Height = motTotal;
                MotionBarMint.Height = motMint;
            }

            // Real-time Comfort Index
            _comfortIndexScore = snap.StabilityScorePercent;
            OptimalComfortText.Text = $"{snap.StabilityScorePercent:0.0}%";
            StabilityScoreText.Text = $"{Math.Round(snap.StabilityScorePercent - 4.2):0}%";

            // Signal Sensitivity real value
            SignalSensitivityText.Text = $"{_state.Settings.FlashFrequencyThresholdHz:0.0} Hz";
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1000)
            {
                Col0.Width = new GridLength(1, GridUnitType.Star);
                Col1.Width = new GridLength(0, GridUnitType.Pixel);

                Grid.SetRow(Card1, 0);
                Grid.SetColumn(Card1, 0);

                Grid.SetRow(Card2, 1);
                Grid.SetColumn(Card2, 0);

                Grid.SetRow(Card3, 2);
                Grid.SetColumn(Card3, 0);

                Grid.SetRow(Card4, 3);
                Grid.SetColumn(Card4, 0);

                while (DashboardMainGrid.RowDefinitions.Count < 4)
                {
                    DashboardMainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }
            }
            else
            {
                Col0.Width = new GridLength(1.15, GridUnitType.Star);
                Col1.Width = new GridLength(1, GridUnitType.Star);

                Grid.SetRow(Card1, 0);
                Grid.SetColumn(Card1, 0);

                Grid.SetRow(Card2, 0);
                Grid.SetColumn(Card2, 1);

                Grid.SetRow(Card3, 1);
                Grid.SetColumn(Card3, 0);

                Grid.SetRow(Card4, 1);
                Grid.SetColumn(Card4, 1);
            }
        }

        private void OnTelemetryFilterClicked(object sender, RoutedEventArgs e)
        {
            _filterIndex = (_filterIndex + 1) % _filters.Length;
            TelemetryFilterText.Text = _filters[_filterIndex];

            if (_filterIndex == 0)
            {
                LuminanceBarLabel.Text = "Luminance";
                MotionBarLabel.Text = "Motion";
                UpdateLiveTelemetry();
            }
            else if (_filterIndex == 1)
            {
                LuminanceBarLabel.Text = "Yesterday";
                MotionBarLabel.Text = "Today";
                LuminanceBarLavender.Height = 110;
                LuminanceBarMint.Height = 60;
                MotionBarLavender.Height = 95;
                MotionBarMint.Height = 50;
            }
            else
            {
                LuminanceBarLabel.Text = "September";
                MotionBarLabel.Text = "November";
                LuminanceBarLavender.Height = 85;
                LuminanceBarMint.Height = 42;
                MotionBarLavender.Height = 125;
                MotionBarMint.Height = 82;
            }
        }

        private void OnComfortPrevClicked(object sender, RoutedEventArgs e)
        {
            // Adjust hardware display brightness down by 5%
            double current = _systemService.GetSystemBrightness();
            int target = (int)Math.Max(10, current - 5);
            _systemService.SetSystemBrightness(target);
            _comfortIndexScore = Math.Max(85.0, _comfortIndexScore - 0.5);
            OptimalComfortText.Text = $"{_comfortIndexScore:0.0}%";
        }

        private void OnComfortNextClicked(object sender, RoutedEventArgs e)
        {
            // Adjust hardware display brightness up by 5%
            double current = _systemService.GetSystemBrightness();
            int target = (int)Math.Min(100, current + 5);
            _systemService.SetSystemBrightness(target);
            _comfortIndexScore = Math.Min(99.8, _comfortIndexScore + 0.5);
            OptimalComfortText.Text = $"{_comfortIndexScore:0.0}%";
        }

        private void OnViewFullHistoryClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ActivityHistoryPage));
        }
    }
}
