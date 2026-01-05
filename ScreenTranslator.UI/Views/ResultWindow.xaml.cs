using System.Windows;
using System.Windows.Input;

namespace ScreenTranslator.UI.Views
{
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
             // Deprecated, kept for interface compatibility if needed but not hooked up in XAML anymore
        }
    }
}
