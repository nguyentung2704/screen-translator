using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenTranslator.Core.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ScreenTranslator.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IHotkeyService _hotkeyService;
        private readonly IScreenCaptureService _captureService;
        private readonly IOcrService _ocrService;
        private readonly ITranslationService _translationService;
        private readonly ScreenTranslator.Infrastructure.Services.TranslationManager _translationManager;
        private readonly ScreenTranslator.Infrastructure.Services.SettingsService _settingsService;

        [ObservableProperty]
        private string _translatedText = string.Empty;

        [ObservableProperty]
        private bool _isSelectionVisible;

        [ObservableProperty]
        private bool _isResultVisible;

        [ObservableProperty]
        private bool _isProcessing;

        [ObservableProperty]
        private string _statusText = "Ready (Ctrl+Shift+D: Capture)";

        [ObservableProperty]
        private string _sourceLanguage = "ja";

        [ObservableProperty]
        private string _targetLanguage = "vi";

        [ObservableProperty]
        private string _geminiApiKey = string.Empty;

        [ObservableProperty] private bool _isJaViSelected = true;
        [ObservableProperty] private bool _isEnViSelected;
        [ObservableProperty] private bool _isStandardTranslatorSelected = true;
        [ObservableProperty] private bool _isAiTranslatorSelected;

        [ObservableProperty] private ModifierKeys _hotkeyModifiers = ModifierKeys.Control | ModifierKeys.Shift;
        [ObservableProperty] private Key _hotkeyKey = Key.D;
        [ObservableProperty] private string _hotkeyDisplay = "Ctrl+Shift+D";

        public IRelayCommand<string> SetLanguageCommand { get; }
        public IRelayCommand<string> SetTranslatorCommand { get; }
        public IRelayCommand SetApiKeyCommand { get; }
        public IRelayCommand SetHotkeyCommand { get; }

        public string AppVersion => "v1.0.3";

        // Helper to show momentary status updates
        private async void UpdateStatusTemporary(string message, int durationMs = 3000)
        {
            StatusText = message;
            await Task.Delay(durationMs);
            if (StatusText == message)
            {
                StatusText = "Mode: Ready";
            }
        }

        // Selection coordinates
        public double SelectionLeft { get; set; }
        public double SelectionTop { get; set; }
        public double SelectionWidth { get; set; }
        public double SelectionHeight { get; set; }

        // Result Window coordinates
        [ObservableProperty]
        private double _resultLeft;
        
        [ObservableProperty]
        private double _resultTop;

        public IRelayCommand StartSelectionCommand { get; }
        public IRelayCommand<Rect> ProcessSelectionCommand { get; }
        public IRelayCommand ExitCommand { get; }

        public MainViewModel(
            IHotkeyService hotkeyService,
            IScreenCaptureService captureService,
            IOcrService ocrService,
            ITranslationService translationService)
        {
            _hotkeyService = hotkeyService;
            _captureService = captureService;
            _ocrService = ocrService;
            _translationService = translationService;
            _translationManager = (ScreenTranslator.Infrastructure.Services.TranslationManager)translationService;
            _settingsService = new ScreenTranslator.Infrastructure.Services.SettingsService();

            StartSelectionCommand = new RelayCommand(StartSelection);
            ProcessSelectionCommand = new AsyncRelayCommand<Rect>(ProcessSelectionAsync);
            SetLanguageCommand = new RelayCommand<string>(SetLanguages);
            SetTranslatorCommand = new RelayCommand<string>(SetTranslator);
            SetApiKeyCommand = new RelayCommand(OpenApiKeySettings);
            SetHotkeyCommand = new RelayCommand(OpenHotkeySettings);
            ExitCommand = new RelayCommand(ExitApp);

            LoadSettings();
        }
        private void SetTranslator(string? type)
        {
            if (type == "AI")
            {
                _translationManager.CurrentProvider = ScreenTranslator.Infrastructure.Services.TranslatorProvider.AI;
                IsStandardTranslatorSelected = false;
                IsAiTranslatorSelected = true;
                UpdateStatusTemporary("Translator: AI (Gemini)");
            }
            else
            {
                _translationManager.CurrentProvider = ScreenTranslator.Infrastructure.Services.TranslatorProvider.Standard;
                IsStandardTranslatorSelected = true;
                IsAiTranslatorSelected = false;
                UpdateStatusTemporary("Translator: Standard");
            }
            SaveSettings();
        }

        private void OpenApiKeySettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var settingsWin = new ScreenTranslator.UI.Views.SettingsWindow(ScreenTranslator.UI.Views.SettingsMode.ApiKey, _translationManager.GetApiKey(), HotkeyModifiers, HotkeyKey);
                settingsWin.ShowDialog();
                
                if (settingsWin.Success)
                {
                    _translationManager.SetGeminiApiKey(settingsWin.ApiKey);
                    SaveSettings();
                    UpdateStatusTemporary("API Key Updated");
                }
            });
        }

        private void OpenHotkeySettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var settingsWin = new ScreenTranslator.UI.Views.SettingsWindow(ScreenTranslator.UI.Views.SettingsMode.Hotkey, _translationManager.GetApiKey(), HotkeyModifiers, HotkeyKey);
                settingsWin.ShowDialog();
                
                if (settingsWin.Success)
                {
                    if (settingsWin.ModifierKeys != HotkeyModifiers || settingsWin.Key != HotkeyKey)
                    {
                        HotkeyModifiers = settingsWin.ModifierKeys;
                        HotkeyKey = settingsWin.Key;
                        UpdateHotkeyDisplay();
                        ReRegisterHotkey();
                        SaveSettings();
                    }
                    UpdateStatusTemporary("Hotkey Updated");
                }
            });
        }

        private void SetLanguages(string? langCode)
        {
            if (langCode == null) return;
            var parts = langCode.Split('-');
            if (parts.Length == 2)
            {
                SourceLanguage = parts[0];
                TargetLanguage = parts[1];

                IsJaViSelected = langCode == "ja-vi";
                IsEnViSelected = langCode == "en-vi";
                
                UpdateStatusTemporary($"Language: {SourceLanguage} -> {TargetLanguage}");
                SaveSettings();
            }
        }

        public void Initialize(IntPtr windowHandle)
        {
            // Register Ctrl + Shift + D
            try
            {
                if (_hotkeyService is ScreenTranslator.Infrastructure.Services.NativeHotkeyService nativeService)
                {
                    nativeService.Initialize(windowHandle);
                }

                _hotkeyService.Register(HotkeyModifiers, HotkeyKey, StartSelection);
                UpdateHotkeyDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register hotkey: {ex.Message}");
            }
        }

        private void ReRegisterHotkey()
        {
            try
            {
                _hotkeyService.UnregisterAll();
                _hotkeyService.Register(HotkeyModifiers, HotkeyKey, StartSelection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to re-register hotkey: {ex.Message}");
            }
        }

        private void UpdateHotkeyDisplay()
        {
            var mods = string.Empty;
            if ((HotkeyModifiers & ModifierKeys.Control) != 0) mods += "Ctrl+";
            if ((HotkeyModifiers & ModifierKeys.Shift) != 0) mods += "Shift+";
            if ((HotkeyModifiers & ModifierKeys.Alt) != 0) mods += "Alt+";
            if ((HotkeyModifiers & ModifierKeys.Windows) != 0) mods += "Win+";
            
            string keyStr = HotkeyKey.ToString();
            // Standardize numeric keys (D1 -> 1, etc.)
            if (keyStr.Length == 2 && keyStr.StartsWith("D") && char.IsDigit(keyStr[1]))
            {
                keyStr = keyStr.Substring(1);
            }

            HotkeyDisplay = $"{mods}{keyStr}";
            StatusText = $"Ready ({HotkeyDisplay}: Capture)";
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();
            HotkeyModifiers = settings.HotkeyModifiers;
            HotkeyKey = settings.HotkeyKey;
            SourceLanguage = settings.SourceLanguage;
            TargetLanguage = settings.TargetLanguage;
            _translationManager.SetGeminiApiKey(settings.GeminiApiKey);
            
            IsJaViSelected = SourceLanguage == "ja" && TargetLanguage == "vi";
            IsEnViSelected = SourceLanguage == "en" && TargetLanguage == "vi";
            
            if (settings.TranslatorProvider == "AI")
            {
                _translationManager.CurrentProvider = ScreenTranslator.Infrastructure.Services.TranslatorProvider.AI;
                IsStandardTranslatorSelected = false;
                IsAiTranslatorSelected = true;
            }
            else
            {
                _translationManager.CurrentProvider = ScreenTranslator.Infrastructure.Services.TranslatorProvider.Standard;
                IsStandardTranslatorSelected = true;
                IsAiTranslatorSelected = false;
            }
        }

        private void SaveSettings()
        {
            var settings = new ScreenTranslator.Core.Models.AppSettings
            {
                GeminiApiKey = _translationManager.GetApiKey(),
                HotkeyModifiers = HotkeyModifiers,
                HotkeyKey = HotkeyKey,
                SourceLanguage = SourceLanguage,
                TargetLanguage = TargetLanguage,
                TranslatorProvider = IsAiTranslatorSelected ? "AI" : "Standard"
            };
            _settingsService.SaveSettings(settings);
        }

        private void StartSelection()
        {
            try 
            {
                IsSelectionVisible = true;
                IsResultVisible = false;
                StatusText = "Mode: Image Capture (Drag to Select)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting selection: " + ex.Message);
            }
        }

        private async Task ProcessSelectionAsync(Rect selection)
        {
            IsSelectionVisible = false;
            
            if (selection.Width <= 0 || selection.Height <= 0) 
            {
                UpdateStatusTemporary("Selection canceled");
                return;
            }

            IsProcessing = true;
            StatusText = "Processing Image...";

            // Position result window near selection
            ResultLeft = selection.Right + 10;
            ResultTop = selection.Top;

            // Adjust if off screen (simplified)
            if (ResultLeft > SystemParameters.PrimaryScreenWidth - 300)
                ResultLeft = selection.Left - 310;
            
            IsResultVisible = true;
            TranslatedText = "Processing...";

            try
            {
                // Add padding to improve OCR accuracy for small text/single lines
                int padding = 10;
                using var stream = await _captureService.CaptureRegionAsync(
                    (int)selection.X - padding, 
                    (int)selection.Y - padding, 
                    (int)selection.Width + (padding * 2), 
                    (int)selection.Height + (padding * 2));
                
                if (stream == null || stream.Length == 0)
                {
                    TranslatedText = "Failed to capture screen.";
                    UpdateStatusTemporary("Error: Capture Failed");
                    return;
                }

                var recognizedText = await _ocrService.RecognizeTextAsync(stream, SourceLanguage);
                
                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    TranslatedText = "No text detected.";
                    UpdateStatusTemporary("OCR Result: No text detected");
                    return;
                }

                StatusText = "Translating...";
                var translated = await _translationService.TranslateAsync(recognizedText, SourceLanguage, TargetLanguage);
                
                if (string.IsNullOrWhiteSpace(translated))
                {
                    TranslatedText = "[Error: Translation returned no result]";
                    UpdateStatusTemporary("Translation Failed");
                }
                else
                {
                    TranslatedText = translated;
                    UpdateStatusTemporary("Translation Complete");
                }
            }
            catch (Exception ex)
            {
                TranslatedText = $"[Error: {ex.Message}]";
                UpdateStatusTemporary($"Error: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void ExitApp()
        {
            _hotkeyService.UnregisterAll();
            Application.Current.Shutdown();
        }
    }
}
