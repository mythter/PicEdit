using Microsoft.Win32;
using PicEdit.Infrastructure.Commands;
using PicEdit.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Markup;

namespace PicEdit.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Window title
        private string _title = "PicEdit";

        /// <summary>
        /// Window title.
        /// </summary>
        public string Title
        {
            get => _title;
            //set
            //{
            //	if (Equals(_title, value)) return;
            //	_title = value;
            //	OnPropertyChanged();

            //	Set(ref _title, value);
            //}
            set => Set(ref _title, value);
        }
        #endregion

        #region MainImage
        private BitmapImage _image;

        /// <summary>
        /// Main image.
        /// </summary>
        public BitmapImage Image
        {
            get => _image;
            set => Set(ref _image, value);
        }
        #endregion

        #region ZoomValue
        private double _zoomValue = 1;

        /// <summary>
        /// Zoom value for scaling the image
        /// </summary>
        public double ZoomValue
        {
            get => _zoomValue;
            set { if (_zoomValue >= 0) Set(ref _zoomValue, value); }
        } 
        #endregion

        #region Commands

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool OnCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region OpenImageCommand
        public ICommand OpenImageCommand { get; }

        private bool OnOpenImageCommandExecute(object p) => true;

        private void OnOpenImageCommandExecuted(object p)
        {
            Forms.OpenFileDialog open = new Forms.OpenFileDialog();
            open.Title = "Select a picture";
            open.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (open.ShowDialog() == Forms.DialogResult.OK)
            {
                Image = new BitmapImage(new Uri(open.FileName));
                OnPropertyChanged(nameof(Image));
            }
        }
        #endregion

        #region ZoomInImageCommand
        public ICommand ZoomInImageCommand { get; }

        private bool OnZoomInImageCommandExecute(object p) => true;

        private void OnZoomInImageCommandExecuted(object p)
        {
            ZoomValue += 0.1;
        }
        #endregion

        #region ZoomOutImageCommand
        public ICommand ZoomOutImageCommand { get; }

        private bool OnZoomOutImageCommandExecute(object p) => true;

        private void OnZoomOutImageCommandExecuted(object p)
        {
            double temp = ZoomValue * 10;
            temp = Math.Round(temp);
            if (temp <= 1)
            {
                return;
            }
            ZoomValue -= 0.1;
        }
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Commands

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, OnCloseApplicationCommandExecute);

            OpenImageCommand = new LambdaCommand(OnOpenImageCommandExecuted, OnOpenImageCommandExecute);

            ZoomInImageCommand = new LambdaCommand(OnZoomInImageCommandExecuted, OnZoomInImageCommandExecute);

            ZoomOutImageCommand = new LambdaCommand(OnZoomOutImageCommandExecuted, OnZoomOutImageCommandExecute);

            #endregion
        }
    }
}
