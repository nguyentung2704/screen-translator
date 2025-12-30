using System.Windows;

namespace ScreenTranslator.UI.Views
{
    public partial class StatusWindow : Window
    {
        public StatusWindow()
        {
            InitializeComponent();
            // Position Top-Right
            this.Left = SystemParameters.PrimaryScreenWidth - 320;
            this.Top = 10;
        }
    }
}
