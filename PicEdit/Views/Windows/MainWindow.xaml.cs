using PicEdit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PicEdit.ViewModels;

namespace PicEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowService windowService = new WindowService();

            DataContext = new MainWindowViewModel(windowService);
        }

        #region ToolBar color settings
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as Grid;
            if (overflowGrid != null)
            {
                overflowGrid.Background = new SolidColorBrush(Color.FromArgb(255, (byte)66, (byte)66, (byte)66));
            }

            var overflowButton = toolBar.Template.FindName("OverflowButton", toolBar) as ToggleButton;
            if (overflowButton != null)
            {
                overflowButton.Background = new SolidColorBrush(Color.FromArgb(255, (byte)66, (byte)66, (byte)66));
            }

            var overflowPanel = toolBar.Template.FindName("PART_ToolBarOverflowPanel", toolBar) as ToolBarOverflowPanel;
            if (overflowPanel != null)
            {
                overflowPanel.Background = new SolidColorBrush(Color.FromArgb(255, (byte)66, (byte)66, (byte)66));
            }
        } 
        #endregion
    }
}
