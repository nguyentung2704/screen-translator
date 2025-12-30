using System;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using ScreenTranslator.Core.Interfaces;

namespace ScreenTranslator.Infrastructure.Services
{
    public class WindowsSpeechService : ISpeechService
    {
        private SpeechRecognizer? _recognizer;
        public event EventHandler<string>? SpeechRecognized;
        private bool _isListening;

        public async Task StartContinuousRecognitionAsync(string languageCode = "ja-JP") // Default to ja-JP for meetings
        {
            if (_isListening) return;

            try 
            {
                if (_recognizer == null)
                {
                    var language = new Language(languageCode);
                    _recognizer = new SpeechRecognizer(language);
                    
                    // Compile constraints (free dictation)
                    await _recognizer.CompileConstraintsAsync();

                    _recognizer.ContinuousRecognitionSession.ResultGenerated += OnResultGenerated;
                    _recognizer.ContinuousRecognitionSession.Completed += OnCompleted;
                }

                await _recognizer.ContinuousRecognitionSession.StartAsync();
                _isListening = true;
            }
            catch (Exception)
            {
                // Handle missing language pack or microphone access error
                // In production, log or propagate
                throw;
            }
        }

        public async Task StopContinuousRecognitionAsync()
        {
            if (!_isListening || _recognizer == null) return;

            try
            {
                await _recognizer.ContinuousRecognitionSession.StopAsync();
                _isListening = false;
            }
            catch
            {
                // Ignore stop errors
            }
        }

        private void OnResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Status == SpeechRecognitionResultStatus.Success && 
                !string.IsNullOrWhiteSpace(args.Result.Text))
            {
                SpeechRecognized?.Invoke(this, args.Result.Text);
            }
        }

        private void OnCompleted(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            _isListening = false;
        }

        public void Dispose()
        {
            if (_recognizer != null)
            {
                _recognizer.ContinuousRecognitionSession.ResultGenerated -= OnResultGenerated;
                _recognizer.ContinuousRecognitionSession.Completed -= OnCompleted;
                _recognizer.Dispose();
                _recognizer = null;
            }
        }
    }
}
