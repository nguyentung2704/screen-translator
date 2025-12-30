using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using ScreenTranslator.Core.Interfaces;
using ScreenTranslator.Infrastructure.Services;
using ScreenTranslator.UI.ViewModels;
using ScreenTranslator.UI.Views;
using System;
using System.Drawing;
using System.Windows;

namespace ScreenTranslator.UI
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private TaskbarIcon? _notifyIcon;
        private SelectionWindow? _selectionWindow;
        private ResultWindow? _resultWindow;
        private static System.Threading.Mutex? _mutex;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Core Services
            services.AddSingleton<IOcrService, WindowsOcrService>();
            services.AddSingleton<GoogleTranslationService>();
            services.AddSingleton<GeminiTranslationService>();
            services.AddSingleton<TranslationManager>();
            services.AddSingleton<ITranslationService>(sp => sp.GetRequiredService<TranslationManager>());
            services.AddSingleton<IHotkeyService, NativeHotkeyService>();
            services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();

            // View Models
            services.AddSingleton<MainViewModel>();

            // Views
            services.AddSingleton<SelectionWindow>();
            services.AddSingleton<ResultWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Single Instance Check
            const string appName = "ScreenTranslator_SingleInstance_Mutex";
            _mutex = new System.Threading.Mutex(true, appName, out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Ứng dụng đang chạy. Bạn hãy kiểm tra ở thanh Taskbar/Tray icon.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
                return;
            }

            // Global Exception Handling
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"An unexpected error occurred: {args.Exception.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true; // Prevent app from crashing
            };

            // Initialize Tray Icon
            _notifyIcon = (TaskbarIcon)FindResource("TrayIcon");
            if (_notifyIcon != null)
            {
                // Set DataContext for commands
                var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
                _notifyIcon.DataContext = mainViewModel;
                
                // Fallback icon if none supplied in XAML (System Error icon as placeholder or Application Icon)
                _notifyIcon.Icon = SystemIcons.Information; 
            }

            // Create Windows (hidden)
            _selectionWindow = _serviceProvider.GetRequiredService<SelectionWindow>();
            _resultWindow = _serviceProvider.GetRequiredService<ResultWindow>();
            
            var vm = _serviceProvider.GetRequiredService<MainViewModel>();

            // Bind VM to Windows
            _selectionWindow.DataContext = vm;
            _resultWindow.DataContext = vm;
            
            // Wire up View logic to ViewModel properties
            // (Ideally using a Behavior or Messenger, but for simplicity we subscribe here)
            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.IsSelectionVisible))
                {
                    if (vm.IsSelectionVisible)
                    {
                        _selectionWindow.Show();
                        _selectionWindow.Activate();
                    }
                    else
                    {
                        _selectionWindow.Hide();
                    }
                }
                else if (args.PropertyName == nameof(MainViewModel.IsResultVisible))
                {
                    if (vm.IsResultVisible)
                    {
                        _resultWindow.Left = vm.ResultLeft;
                        _resultWindow.Top = vm.ResultTop;
                        _resultWindow.Show();
                        _resultWindow.Activate();
                    }
                    else
                    {
                        _resultWindow.Hide();
                    }
                }
            };

            // Wire up Selection Completion from View to ViewModel
            _selectionWindow.SelectionCompleted += (rect) =>
            {
                if (vm.ProcessSelectionCommand.CanExecute(rect))
                {
                    vm.ProcessSelectionCommand.Execute(rect);
                }
            };

            // Initialize Hotkeys
            // We need a window handle for the NativeHotkeyService. 
            // We can use the hidden SelectionWindow handle or create a message-only window.
            // Using SelectionWindow handle is easiest since it exists.
            _selectionWindow.SourceInitialized += (s, a) =>
            {
                 var handle = new System.Windows.Interop.WindowInteropHelper(_selectionWindow).Handle;
                 vm.Initialize(handle);
            };
            
            // Force handle creation
            _selectionWindow.Show(); 
            _selectionWindow.Hide();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            _mutex?.ReleaseMutex();
            _mutex?.Close();
        }
    }
}
