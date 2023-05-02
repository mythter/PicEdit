using PicEdit.Infrastructure.Commands;
using PicEdit.ViewModels.Base;
using System;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.DataFormats;

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

        Stream imageStream;

        #region MainImage
        private BitmapImage _image;

        /// <summary>
        /// Main image.
        /// </summary>
        public BitmapImage Image
        {
            get => _image;
            set
            {
                IsSaveEnabled = true;
                Set(ref _image, value);
            }
        }
        #endregion

        #region MainImageFormat
        /// <summary>
        /// Format of a main image.
        /// </summary>
        private ImageFormat _format;
        #endregion

        #region MainImageSaveFormat
        /// <summary>
        /// Format to save a main image.
        /// </summary>
        private string _saveFormat;
        #endregion

        #region MainImagePath
        /// <summary>
        /// Path of a main image.
        /// </summary>
        private string _path = "";
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

        #region IsSaveEnabled
        private bool _isSaveEnabled = false;

        /// <summary>
        /// Property of "Save" and "Save as" buttons in menu.
        /// </summary>
        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set => Set(ref _isSaveEnabled, value);
        }
        #endregion

        #region Commands

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool OnCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            imageStream?.Close();
            Application.Current.Shutdown();
        }
        #endregion

        #region OpenImageCommand
        public ICommand OpenImageCommand { get; }

        private bool OnOpenImageCommandExecute(object p) => true;

        private void OnOpenImageCommandExecuted(object p)
        {
            Forms.OpenFileDialog open = new Forms.OpenFileDialog();
            open.Title = "Open an image";
            open.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG;*.TIFF;*.ICO)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG;*.TIFF;*.ICO";
            if (open.ShowDialog() == Forms.DialogResult.OK)
            {
                imageStream = new System.IO.MemoryStream(File.ReadAllBytes(open.FileName));
                //Image = (BitmapImage)(BitmapSource)BitmapDecoder.Create(imageStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames[0];
                //Image img = System.Drawing.Image.FromStream(File.Open(open.FileName, FileMode.Open));
                _saveFormat = open.FileName.Substring(open.FileName.LastIndexOf('.') + 1);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = imageStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                Image = bitmap;

                switch (_saveFormat)
                {
                    case "png":
                        _format = ImageFormat.Png;
                        break;
                    case "jpeg":
                    case "jpg":
                        _format = ImageFormat.Jpeg;
                        break;
                    case "gif":
                        _format = ImageFormat.Gif;
                        break;
                    case "bmp":
                        _format = ImageFormat.Bmp;
                        break;
                    case "tiff":
                        _format = ImageFormat.Tiff;
                        break;
                    case "ico":
                        _format = ImageFormat.Icon;
                        break;
                    default:
                        _format = ImageFormat.Png;
                        break;
                }

                //img.Save(imageStream, _format);
                //imageStream.Position = 0;
                //Image = new BitmapImage();
                //Image.BeginInit();
                //Image.StreamSource = imageStream;
                //Image.CacheOption = BitmapCacheOption.OnLoad;
                //Image.EndInit();
                //Image = new BitmapImage(new Uri(open.FileName));
                _path = open.FileName;
                OnPropertyChanged(nameof(Image));
            }
        }
        #endregion

        #region ChangeSaveImageFormatCommand
        public ICommand ChangeSaveImageFormatCommand { get; }

        private bool OnChangeSaveImageFormatCommandExecute(object p) => true;

        private void OnChangeSaveImageFormatCommandExecuted(object p)
        {
            _saveFormat = (string)p;
        }
        #endregion

        #region ConvertImageCommand
        public ICommand ConvertImageCommand { get; }

        private bool OnConvertImageCommandExecute(object p) => true;

        private void OnConvertImageCommandExecuted(object p)
        {
            Forms.SaveFileDialog save = new Forms.SaveFileDialog();
            save.Title = "Save image as ...";
            save.Filter = "PNG File(*.png)|*.png|" +
                "JPG File(*.jpg)|*.jpg|" +
                "GIF File(*.gif)|*.gif|" +
                "BMP File(*.bmp)|*.bmp|" +
                "TIFF File(*.tiff)|*.tiff|" +
                "ICO File(*.ico)|*.ico";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            save.FileName = "Untitled1";
            ImageFormat saveFormat;

            switch (_saveFormat)
            {
                case "png":
                    save.FilterIndex = 1;
                    saveFormat = ImageFormat.Png;
                    break;
                case "jpeg":
                case "jpg":
                    save.FilterIndex = 2;
                    saveFormat = ImageFormat.Jpeg;
                    break;
                case "gif":
                    save.FilterIndex = 3;
                    saveFormat = ImageFormat.Gif;
                    break;
                case "bmp":
                    save.FilterIndex = 4;
                    saveFormat = ImageFormat.Bmp;
                    break;
                case "tiff":
                    save.FilterIndex = 5;
                    saveFormat = ImageFormat.Tiff;
                    break;
                case "ico":
                    save.FilterIndex = 6;
                    saveFormat = ImageFormat.Icon;
                    break;
                default:
                    save.FilterIndex = 1;
                    saveFormat = ImageFormat.Png;
                    break;
            }

            if (save.ShowDialog() == Forms.DialogResult.OK)
            {
                string fileName = save.FileName;

                Bitmap bmp;
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(Image));
                    enc.Save(outStream);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                    bmp = new Bitmap(bitmap);
                }

                bmp.Save(fileName, saveFormat);
            }
        }
        #endregion

        #region SaveAsCommand
        public ICommand SaveAsCommand { get; }

        private bool OnSaveAsCommandExecute(object p) => true;

        private void OnSaveAsCommandExecuted(object p)
        {
            Forms.SaveFileDialog save = new Forms.SaveFileDialog();
            save.Title = "Save image as ...";
            save.Filter = "PNG File(*.png)|*.png|" +
                "JPG File(*.jpg)|*.jpg|" +
                "GIF File(*.gif)|*.gif|" +
                "BMP File(*.bmp)|*.bmp|" +
                "TIFF File(*.tiff)|*.tiff|" +
                "ICO File(*.ico)|*.ico";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            save.FileName = "Untitled1";

            if (save.ShowDialog() == Forms.DialogResult.OK)
            {
                string fileName = save.FileName;

                Bitmap bmp;
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(Image));
                    enc.Save(outStream);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                    bmp = new Bitmap(bitmap);
                }

                bmp.Save(fileName);
            }
        }
        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }

        private bool OnSaveCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            ImageFormat saveFormat = null;
            switch (_saveFormat)
            {
                case "png":
                    saveFormat = ImageFormat.Png;
                    break;
                case "jpeg":
                case "jpg":
                    saveFormat = ImageFormat.Jpeg;
                    break;
                case "gif":
                    saveFormat = ImageFormat.Gif;
                    break;
                case "bmp":
                    saveFormat = ImageFormat.Bmp;
                    break;
                case "tiff":
                    saveFormat = ImageFormat.Tiff;
                    break;
                case "ico":
                    saveFormat = ImageFormat.Icon;
                    break;
                default:
                    saveFormat = ImageFormat.Png;
                    break;
            }

            var img = System.Drawing.Image.FromStream(imageStream);
            img.Save(_path);

            //Bitmap bmp;
            //using (MemoryStream outStream = new MemoryStream())
            //{
            //    BitmapEncoder enc = new BmpBitmapEncoder();
            //    enc.Frames.Add(BitmapFrame.Create(Image));
            //    enc.Save(outStream);
            //    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
            //    bmp = new Bitmap(bitmap);
            //}

            //using (MemoryStream memory = new MemoryStream())
            //{
            //    using (FileStream fs = new FileStream(_path, FileMode.Create, FileAccess.ReadWrite))
            //    {
            //        bmp.Save(memory, ImageFormat.Jpeg);
            //        byte[] bytes = memory.ToArray();
            //        fs.Write(bytes, 0, bytes.Length);
            //    }
            //}

            //bmp.Save(_path);
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
            double temp = Math.Round(ZoomValue * 10);
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

            ChangeSaveImageFormatCommand = new LambdaCommand(OnChangeSaveImageFormatCommandExecuted, OnChangeSaveImageFormatCommandExecute);

            ConvertImageCommand = new LambdaCommand(OnConvertImageCommandExecuted, OnConvertImageCommandExecute);

            SaveAsCommand = new LambdaCommand(OnSaveAsCommandExecuted, OnSaveAsCommandExecute);

            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, OnSaveCommandExecute);

            #endregion
        }
    }
}
