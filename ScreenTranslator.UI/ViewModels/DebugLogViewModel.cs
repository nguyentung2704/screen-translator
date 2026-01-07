using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace ScreenTranslator.UI.ViewModels
{
    public partial class DebugLogViewModel : ObservableObject
    {
        public record LogEntry(DateTime Timestamp, string Type, string Content);

        public ObservableCollection<LogEntry> Logs { get; } = new ObservableCollection<LogEntry>();

        [ObservableProperty]
        private bool _autoScroll = true;

        [ObservableProperty]
        private bool _isPaused = false;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private int _logCount = 0;

        public IRelayCommand ClearCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand TogglePauseCommand { get; }

        public DebugLogViewModel()
        {
            ClearCommand = new RelayCommand(ClearLogs);
            SaveCommand = new RelayCommand(SaveLogs);
            TogglePauseCommand = new RelayCommand(() => IsPaused = !IsPaused);
        }

        public void AddLog(string type, string content)
        {
            if (IsPaused) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Add(new LogEntry(DateTime.Now, type, content));
                LogCount = Logs.Count;
                StatusText = $"Last update: {DateTime.Now:HH:mm:ss}";
            });
        }

        private void ClearLogs()
        {
            Logs.Clear();
            LogCount = 0;
            StatusText = "Logs cleared";
        }

        private void SaveLogs()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"DebugLog_{DateTime.Now:yyyyMMdd_HHmmss}",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            if (dlg.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                foreach (var log in Logs)
                {
                    sb.AppendLine($"[{log.Timestamp:HH:mm:ss}] [{log.Type}] {log.Content}");
                }
                
                try 
                {
                    System.IO.File.WriteAllText(dlg.FileName, sb.ToString());
                    StatusText = "Saved to file";
                }
                catch (Exception ex)
                {
                    StatusText = $"Error saving: {ex.Message}";
                }
            }
        }
    }
}
