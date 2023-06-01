using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PicEditWPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        double X;
        double Y;
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            X = e.GetPosition(container).X - translate.X;
            Y = e.GetPosition(container).Y - translate.Y;
            canvas.CaptureMouse();
        }

        Stopwatch sw = new Stopwatch();
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {

                translate.X = e.GetPosition(container).X - X;
                translate.Y = e.GetPosition(container).Y - Y;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
        }
    }
}
