using System.Windows;

namespace ScreenTranslator.UI.Views
{
    public enum SettingsMode { ApiKey, Hotkey }

    public partial class SettingsWindow : Window
    {
        public string ApiKey { get; private set; } = string.Empty;
        public System.Windows.Input.ModifierKeys ModifierKeys { get; private set; }
        public System.Windows.Input.Key Key { get; private set; }
        public bool Success { get; private set; }
        public SettingsMode CurrentMode { get; private set; }

        public class KeyItem
        {
            public System.Windows.Input.Key Value { get; set; }
            public string Display { get; set; } = string.Empty;
        }

        public SettingsWindow(SettingsMode mode, string currentApiKey, System.Windows.Input.ModifierKeys currentModifiers, System.Windows.Input.Key currentKey)
        {
            InitializeComponent();
            CurrentMode = mode;
            ApiKeyBox.Password = currentApiKey;
            
            if (mode == SettingsMode.ApiKey)
            {
                TitleBlock.Text = "Gemini AI Settings";
                HotkeySection.Visibility = Visibility.Collapsed;
                this.Height = 220;
            }
            else
            {
                TitleBlock.Text = "Hotkey Settings";
                ApiSection.Visibility = Visibility.Collapsed;
                this.Height = 180;
            }

            // Populate common keys
            var items = new System.Collections.Generic.List<KeyItem>();
            foreach (var k in System.Enum.GetValues(typeof(System.Windows.Input.Key)))
            {
                var kv = (System.Windows.Input.Key)k;
                if ((kv >= System.Windows.Input.Key.A && kv <= System.Windows.Input.Key.Z) || 
                    (kv >= System.Windows.Input.Key.F1 && kv <= System.Windows.Input.Key.F12) ||
                    (kv >= System.Windows.Input.Key.D0 && kv <= System.Windows.Input.Key.D9))
                {
                    string name = kv.ToString();
                    if (name.Length == 2 && name.StartsWith("D") && char.IsDigit(name[1]))
                        name = name.Substring(1);

                    items.Add(new KeyItem { Value = kv, Display = name });
                }
            }
            KeyCombo.ItemsSource = items;
            KeyCombo.SelectedValue = currentKey;

            CtrlCheck.IsChecked = (currentModifiers & System.Windows.Input.ModifierKeys.Control) != 0;
            ShiftCheck.IsChecked = (currentModifiers & System.Windows.Input.ModifierKeys.Shift) != 0;
            AltCheck.IsChecked = (currentModifiers & System.Windows.Input.ModifierKeys.Alt) != 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApiKey = ApiKeyBox.Password;
            
            ModifierKeys = System.Windows.Input.ModifierKeys.None;
            if (CtrlCheck.IsChecked == true) ModifierKeys |= System.Windows.Input.ModifierKeys.Control;
            if (ShiftCheck.IsChecked == true) ModifierKeys |= System.Windows.Input.ModifierKeys.Shift;
            if (AltCheck.IsChecked == true) ModifierKeys |= System.Windows.Input.ModifierKeys.Alt;
            
            if (CurrentMode == SettingsMode.Hotkey)
            {
                if (KeyCombo.SelectedValue is System.Windows.Input.Key selectedKey)
                {
                    Key = selectedKey;
                    Success = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Please select a key.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // In ApiKey mode, we don't strictly require a hotkey selection change
                Success = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://aistudio.google.com/app/apikey") { UseShellExecute = true });
            }
            catch { }
        }
    }
}
