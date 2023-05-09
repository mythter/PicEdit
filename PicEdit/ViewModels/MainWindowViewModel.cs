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
using System.Windows.Media;

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
        private BitmapSource _image;

        /// <summary>
        /// Main image.
        /// </summary>
        public BitmapSource Image
        {
            get => _image;
            set
            {
                IsSaveEnabled = true;
                Set(ref _image, value);
            }
        }
        #endregion

        #region MainImage Format
        /// <summary>
        /// Format of a main image.
        /// </summary>
        private ImageFormat _format;
        #endregion

        #region MainImage Save Format
        /// <summary>
        /// Format to save a main image.
        /// </summary>
        private string _saveFormat = "png";
        #endregion

        #region MainImage Path
        /// <summary>
        /// Path of a main image.
        /// </summary>
        private string _path = "";
        #endregion

        #region Is Scale Relation Checked
        private bool _isScaleChecked = false;

        /// <summary>
        /// Is scaleX and scaleY do synchronously
        /// </summary>
        public bool IsScaleChecked
        {
            get => _isScaleChecked;
            set
            {
                Set(ref _isScaleChecked, value);
            }
        }
        #endregion

        #region Is Rotation Tool Checked
        private bool _isRotationToolChecked;

        /// <summary>
        /// Is rotation tool checked
        /// </summary>
        public bool IsRotationToolChecked
        {
            get => _isRotationToolChecked;
            set
            {
                Set(ref _isRotationToolChecked, value);
                if (!IsRotationToolChecked)
                {
                    Image = new TransformedBitmap(Image, new RotateTransform(AngleValue));
                    AngleValue = 0;
                }

            }
        }
        #endregion

        #region Is Scale Tool Checked
        private bool _isScaleToolChecked;

        /// <summary>
        /// Is scale tool checked
        /// </summary>
        public bool IsScaleToolChecked
        {
            get => _isScaleToolChecked;
            set
            {
                if (SliderXValue != 100 || SliderYValue != 100)
                {
                    Image = new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue));
                    SliderXValue = 100;
                    SliderYValue = 100;
                }

            }
        }
        #endregion

        #region MainImage Scale X Value
        private double _scaleXValue = 1;

        /// <summary>
        /// ScaleX value of main image
        /// </summary>
        public double ScaleXValue
        {
            get => _scaleXValue;
            set
            {
                Set(ref _scaleXValue, value);
            }
        }
        #endregion

        #region MainImage Scale Y Value
        private double _scaleYValue = 1;

        /// <summary>
        /// ScaleY value of main image
        /// </summary>
        public double ScaleYValue
        {
            get => _scaleYValue;
            set
            {
                Set(ref _scaleYValue, value);
            }
        }
        #endregion

        #region MainImage Angle Value
        private int _angleValue = 0;

        /// <summary>
        /// Angle value of main image
        /// </summary>
        public int AngleValue
        {
            get => _angleValue;
            set
            {
                Set(ref _angleValue, value);
            }
        }
        #endregion

        #region Zoom Value
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

        #region Is Save Enabled
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

        #region Slider X Value
        private int _sliderXValue = 100;

        /// <summary>
        /// Slider X value
        /// </summary>
        public int SliderXValue
        {
            get => _sliderXValue;
            set
            {
                Set(ref _sliderXValue, value);
                ScaleXValue = SliderXValue / 100f;
                if (IsScaleChecked && SliderYValue != SliderXValue)
                    SliderYValue = SliderXValue;
            }
        }
        #endregion

        #region Slider Y Value
        private int _sliderYValue = 100;

        /// <summary>
        /// Slider Y value
        /// </summary>
        public int SliderYValue
        {
            get => _sliderYValue;
            set
            {
                Set(ref _sliderYValue, value);
                ScaleYValue = SliderYValue / 100f;
                if (IsScaleChecked && SliderXValue != SliderYValue)
                    SliderXValue = SliderYValue;
            }
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
                string format = open.FileName.Substring(open.FileName.LastIndexOf('.') + 1);
                _format = ToImageFormat(format);
 
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = imageStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                Image = bitmap;

                _path = open.FileName;
                OnPropertyChanged(nameof(Image));
            }
            ZoomValue = 0.7;
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

            switch (_saveFormat)
            {
                case "png":
                    save.FilterIndex = 1;
                    break;
                case "jpeg":
                case "jpg":
                    save.FilterIndex = 2;
                    break;
                case "gif":
                    save.FilterIndex = 3;
                    break;
                case "bmp":
                    save.FilterIndex = 4;
                    break;
                case "tiff":
                    save.FilterIndex = 5;
                    break;
                case "ico":
                    save.FilterIndex = 6;
                    break;
                default:
                    save.FilterIndex = 1;
                    break;
            }

            if (save.ShowDialog() == Forms.DialogResult.OK)
            {
                string fileName = save.FileName;
                string chosenFormat = fileName.Substring(fileName.LastIndexOf(".") + 1);
                _saveFormat = _saveFormat == chosenFormat ? _saveFormat : chosenFormat;
                ImageFormat saveFormat = ToImageFormat(_saveFormat);
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
                string chosenFormat = fileName.Substring(fileName.LastIndexOf(".") + 1);
                ImageFormat saveFormat = ToImageFormat(chosenFormat);
                //Bitmap bmp;
                //using (MemoryStream outStream = new MemoryStream())
                //{
                //    BitmapEncoder enc = new BmpBitmapEncoder();
                //    enc.Frames.Add(BitmapFrame.Create(Image));
                //    enc.Save(outStream);
                //    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                //    bmp = new Bitmap(bitmap);
                //}

                //bmp.Save(fileName, saveFormat);

                SaveImage(fileName, saveFormat);
            }
        }
        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }

        private bool OnSaveCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            SaveImage(_path, _format);
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

        #region Functions
        private ImageFormat ToImageFormat(string strFormat)
        {
            switch (strFormat)
            {
                case "png":
                    return ImageFormat.Png;
                case "jpeg":
                case "jpg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                case "tiff":
                    return ImageFormat.Tiff;
                case "ico":
                    return ImageFormat.Icon;
                default:
                    return ImageFormat.Png;
            }
        }

        private Stream StreamFromBitmapSource(BitmapSource writeBmp)
        {
            Stream bmp = new MemoryStream();

            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(writeBmp));
            enc.Save(bmp);

            return bmp;
        }

        private void SaveImage(string path,  ImageFormat format)
        {
            if (imageStream != null)
            {
                if(SliderXValue != 100 || SliderYValue != 100)
                {
                    Image = new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue));
                    SliderXValue = 100;
                    SliderYValue = 100;
                }                  
                if(AngleValue != 0)
                {
                    Image = new TransformedBitmap(Image, new RotateTransform(AngleValue));
                    AngleValue = 0;
                }
                imageStream = StreamFromBitmapSource(Image);
                var img = System.Drawing.Image.FromStream(imageStream);
                img.Save(path, format);
            }
        }

        //static byte[] GetBytesFromBitmapSource(BitmapSource bmp)
        //{
        //    int width = bmp.PixelWidth;
        //    int height = bmp.PixelHeight;
        //    int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

        //    byte[] pixels = new byte[height * stride];

        //    bmp.CopyPixels(pixels, stride, 0);

        //    return pixels;
        //}

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
