using ScreenTranslator.UI.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenTranslator.UI.Views
{
    public partial class DebugLogWindow : Window
    {
        private readonly DebugLogViewModel _viewModel;
        private ScrollViewer? _scrollViewer;

        public DebugLogWindow(DebugLogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.Logs.CollectionChanged += Logs_CollectionChanged;
        }

        private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel.AutoScroll && e.Action == NotifyCollectionChangedAction.Add)
            {
                if (_scrollViewer == null)
                {
                    _scrollViewer = FindVisualChild<ScrollViewer>(this);
                }
                _scrollViewer?.ScrollToBottom();
            }
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild) return typedChild;
                
                var nested = FindVisualChild<T>(child);
                if (nested != null) return nested;
            }
            return null;
        }
    }
}
