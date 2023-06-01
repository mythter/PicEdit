using System.Windows;
using PicEdit.Services;
using PicEdit.ViewModels;

namespace PicEdit.Views.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            WindowService windowService = new WindowService();

            DataContext = new MainWindowViewModel(windowService);

            Top = (SystemParameters.WorkArea.Height - Height) / 2;
            Left = (SystemParameters.WorkArea.Width - Width) / 2;
        }
    }
}
