using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using IDLE.Models;

namespace IDLE.Services
{
    public class IdleStateService
    {
        private static readonly Lazy<IdleStateService> _instance = new(() => new IdleStateService());
        public static IdleStateService Instance => _instance.Value;

        private readonly string _settingsFilePath;
        private readonly string _historyFilePath;

        public AppSettingsState Settings { get; private set; } = new AppSettingsState();
        public ObservableCollection<ActivityEventItem> HistoryEvents { get; } = new ObservableCollection<ActivityEventItem>();

        public event EventHandler? SettingsChanged;

        private IdleStateService()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IDLE");
            Directory.CreateDirectory(appData);
            _settingsFilePath = Path.Combine(appData, "idle_settings.json");
            _historyFilePath = Path.Combine(appData, "idle_history.json");

            LoadSettings();
            LoadHistory();
            EnsureSampleHistoryIfEmpty();
        }

        public void SaveSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<AppSettingsState>(json);
                    if (loaded != null) Settings = loaded;
                }
            }
            catch { }
        }

        public void AddHistoryEvent(ActivityEventItem item)
        {
            HistoryEvents.Insert(0, item);
            SaveHistory();
        }

        public void ClearHistory()
        {
            HistoryEvents.Clear();
            SaveHistory();
        }

        private void SaveHistory()
        {
            try
            {
                string json = JsonSerializer.Serialize(HistoryEvents, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_historyFilePath, json);
            }
            catch { }
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyFilePath))
                {
                    string json = File.ReadAllText(_historyFilePath);
                    var list = JsonSerializer.Deserialize<ObservableCollection<ActivityEventItem>>(json);
                    if (list != null)
                    {
                        HistoryEvents.Clear();
                        foreach (var item in list) HistoryEvents.Add(item);
                    }
                }
            }
            catch { }
        }

        private void EnsureSampleHistoryIfEmpty()
        {
            if (HistoryEvents.Count == 0)
            {
                HistoryEvents.Add(new ActivityEventItem
                {
                    Timestamp = DateTime.Now.AddMinutes(-14),
                    Category = "Flash Detection",
                    Title = "Repeated flash sequence detected",
                    Explanation = "3.4 Hz luminance oscillation exceeding 38% magnitude threshold across screen quadrant 2.",
                    MeasuredValue = 3.4,
                    MeasuredValueUnit = "Hz",
                    Confidence = 0.94,
                    RuleFired = "Repeated flash frequency rule (3-30 Hz range)",
                    ActionTaken = "Notification displayed; temporary dimming offered",
                    SourceApplication = "Active Browser Window"
                });

                HistoryEvents.Add(new ActivityEventItem
                {
                    Timestamp = DateTime.Now.AddHours(-1).AddMinutes(-22),
                    Category = "Motion & Vestibular",
                    Title = "High-acceleration rapid scroll detected",
                    Explanation = "Continuous vertical viewport translation rate of 2,400 px/sec exceeded smooth threshold.",
                    MeasuredValue = 2400,
                    MeasuredValueUnit = "px/s",
                    Confidence = 0.91,
                    RuleFired = "Rapid scroll acceleration limit",
                    ActionTaken = "Instant scroll damping recommended",
                    SourceApplication = "Feed Reader"
                });

                HistoryEvents.Add(new ActivityEventItem
                {
                    Timestamp = DateTime.Now.AddHours(-3),
                    Category = "Display & Visual Comfort",
                    Title = "Abrupt white-screen transition",
                    Explanation = "Large bright area increased average screen luminance from 14% to 89% in under 80ms.",
                    MeasuredValue = 75,
                    MeasuredValueUnit = "% delta",
                    Confidence = 0.98,
                    RuleFired = "Dark-to-bright abrupt transition",
                    ActionTaken = "Gentle transition filter applied",
                    SourceApplication = "System"
                });

                SaveHistory();
            }
        }
    }
}
