using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScreenTranslator.UI.Views
{
    public partial class SelectionWindow : Window
    {
        private bool _isDragging;
        private Point _startPoint;
        public event Action<Rect>? SelectionCompleted;

        public SelectionWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _startPoint = e.GetPosition(this);
                SelectionRect.Visibility = Visibility.Visible;
                Canvas.SetLeft(SelectionRect, _startPoint.X);
                Canvas.SetTop(SelectionRect, _startPoint.Y);
                SelectionRect.Width = 0;
                SelectionRect.Height = 0;
                CaptureMouse();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(this);
                var x = Math.Min(currentPoint.X, _startPoint.X);
                var y = Math.Min(currentPoint.Y, _startPoint.Y);
                var width = Math.Abs(currentPoint.X - _startPoint.X);
                var height = Math.Abs(currentPoint.Y - _startPoint.Y);

                Canvas.SetLeft(SelectionRect, x);
                Canvas.SetTop(SelectionRect, y);
                SelectionRect.Width = width;
                SelectionRect.Height = height;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                ReleaseMouseCapture();
                SelectionRect.Visibility = Visibility.Collapsed;
                
                var width = SelectionRect.Width;
                var height = SelectionRect.Height;
                var left = Canvas.GetLeft(SelectionRect);
                var top = Canvas.GetTop(SelectionRect);

                // Convert to screen coordinates
                var topLeft = PointToScreen(new Point(left, top));
                
                // On High DPI setups, PointToScreen might need adjustment if logic was pure WPF coords, 
                // but PointToScreen returns physical pixels usually suitable for CopyFromScreen unless Per-Monitor DPI aware.
                // For simplicity, we assume standard scaling or that the logical/physical mapping works out.
                // Ideally we should use the scaling factor.
                
                // Let's pass the rect back.
                SelectionCompleted?.Invoke(new Rect(topLeft.X, topLeft.Y, width, height)); // Using raw physical capture later might need correction
                // Actually, PointToScreen returns pixels. CopyFromScreen uses pixels. Width/Height in WPF are logical units.
                // We need to convert Width/Height to pixels too.
                
                var source = PresentationSource.FromVisual(this);
                double dpiX = 1.0, dpiY = 1.0;
                if (source?.CompositionTarget != null)
                {
                    dpiX = source.CompositionTarget.TransformToDevice.M11;
                    dpiY = source.CompositionTarget.TransformToDevice.M22;
                }
                
                SelectionCompleted?.Invoke(new Rect(topLeft.X, topLeft.Y, width * dpiX, height * dpiY));
                
                Hide();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _isDragging = false;
                ReleaseMouseCapture();
                SelectionCompleted?.Invoke(new Rect(0, 0, 0, 0));
                Hide();
            }
        }
    }
}
