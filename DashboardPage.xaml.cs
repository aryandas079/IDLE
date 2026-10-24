using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IDLE.Services;

namespace IDLE
{
    public sealed partial class DashboardPage : Page
    {
        private readonly IdleStateService _state = IdleStateService.Instance;
        private int _filterIndex = 0;
        private readonly string[] _filters = new[] { "Real-time", "Daily", "Weekly" };
        private readonly (double lumLav, double lumMint, double motLav, double motMint, string frames)[] _telemetryPresets = new[]
        {
            (100.0, 50.0, 140.0, 95.0, "2,480"),
            (120.0, 70.0, 110.0, 60.0, "18,920"),
            (85.0, 40.0, 150.0, 110.0, "124,350")
        };

        private double _comfortIndexScore = 98.5;

        public DashboardPage()
        {
            InitializeComponent();
            LoadState();
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

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Dynamic responsive layout breakdown:
            // When width is tight (< 1000px), stack cards in a single column so nothing is compressed.
            // When width is >= 1000px, maintain the sleek 2-column dashboard layout from the design mockup.
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

                // Ensure 4 rows exist
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

            // Animate / adjust responsive telemetry bars
            var preset = _telemetryPresets[_filterIndex];
            LuminanceBarLavender.Height = preset.lumLav;
            LuminanceBarMint.Height = preset.lumMint;
            MotionBarLavender.Height = preset.motLav;
            MotionBarMint.Height = preset.motMint;
            FramesAnalyzedCountText.Text = preset.frames;
        }

        private void OnComfortPrevClicked(object sender, RoutedEventArgs e)
        {
            _comfortIndexScore = Math.Max(88.0, _comfortIndexScore - 1.5);
            OptimalComfortText.Text = $"{_comfortIndexScore:0.0}%";
            StabilityScoreText.Text = $"{_comfortIndexScore - 4.5:0}%";
        }

        private void OnComfortNextClicked(object sender, RoutedEventArgs e)
        {
            _comfortIndexScore = Math.Min(99.8, _comfortIndexScore + 1.2);
            OptimalComfortText.Text = $"{_comfortIndexScore:0.0}%";
            StabilityScoreText.Text = $"{_comfortIndexScore - 4.5:0}%";
        }

        private void OnViewFullHistoryClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ActivityHistoryPage));
        }
    }
}
