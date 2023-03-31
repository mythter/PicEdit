using System;
using System.Collections.Generic;
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

        private void SV_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0) Zoom.Value += 0.1;
            else Zoom.Value -= 0.1;
        }

        //private double _startX;
        //private double _startY;

        //private void Samplebutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    _startX = Mouse.GetPosition(MainImage).X;
        //    _startY = Mouse.GetPosition(MainImage).Y;
        //}

        //private void Samplebutton_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //   if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Point imgP = e.GetPosition(MainImage);
        //        Point rectP = e.GetPosition(HRect);

        //        if(rectP.X > imgP.X)
        //        {
        //            rectP.X = imgP.X;
        //            return;
        //        }
        //        else if(rectP.Y > imgP.Y)
        //        {
        //            rectP.Y = imgP.Y;
        //            return;
        //        }

        //        SV.ScrollToHorizontalOffset((imgP.X * Zoom.Value) - HRect.Width / 2);
        //        SV.ScrollToVerticalOffset((imgP.Y * Zoom.Value) - HRect.Height / 2);

        //        TranslateTransform transform = new TranslateTransform();
        //        transform.X = Mouse.GetPosition(MainImage).X - _startX;
        //        transform.Y = Mouse.GetPosition(MainImage).Y - _startY;
        //        this.HRect.RenderTransform = transform;
        //    }
        //}

        //private void SV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    HRect.Width = SV.ViewportWidth / Zoom.Value;
        //    HRect.Height = SV.ViewportHeight / Zoom.Value;
        //    HRect.SetValue(Canvas.LeftProperty, SV.ContentHorizontalOffset / Zoom.Value);
        //    HRect.SetValue(Canvas.TopProperty, SV.ContentVerticalOffset / Zoom.Value);
        //}

        //private void Image_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Point P = e.GetPosition(Canv);
        //        SV.ScrollToHorizontalOffset((P.X * Zoom.Value) - HRect.Width / 2);
        //        SV.ScrollToVerticalOffset((P.Y * Zoom.Value) - HRect.Height / 2);
        //    }
        //}
    }
}
