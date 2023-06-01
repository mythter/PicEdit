﻿using PicEdit.Infrastructure.Commands;
using PicEdit.ViewModels.Base;
using System;
using System.Windows;
using EditingMode = System.Windows.Controls.InkCanvasEditingMode;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Ink;
using PicEdit.Services;
using System.Drawing;

namespace PicEdit.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Fields

        /// <summary>
        /// Variable for interaction of windows.
        /// </summary>
        IWindowService _windowService;

        /// <summary>
        /// Stream of main image.
        /// </summary>
        Stream? _imageStream;

        /// <summary>
        /// Collection to store changes of main image.
        /// </summary>
        ObservableCollection<BitmapSource> _obCollection;
        int position = -1;

        /// <summary>
        /// Collection to store strokes while painting.
        /// </summary>
        ObservableCollection<StrokeCollection> _obStrokeCollection;
        int strPos = -1;

        /// <summary>
        /// Path of the main image.
        /// </summary>
        private string _path = "";

        /// <summary>
        /// Format to save the main image.
        /// </summary>
        private string _saveFormat = "png";

        /// <summary>
        /// Format of the main image.
        /// </summary>
        private ImageFormat? _format;

        #endregion

        #region Properties

        #region Window title
        private string _title = "PicEdit";

        /// <summary>
        /// Window title.
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion

        #region MainImage
        private BitmapSource? _image;

        /// <summary>
        /// Main image.
        /// </summary>
        public BitmapSource? Image
        {
            get => _image;
            set
            {
                Set(ref _image, value);
                IsSaveEnabled = true;
            }
        }
        #endregion

        #region Is Scale Relation Checked
        private bool _isScaleChecked = false;

        /// <summary>
        /// Is scaleX and scaleY do synchronously.
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
        /// Is rotation tool checked.
        /// </summary>
        public bool IsRotationToolChecked
        {
            get => _isRotationToolChecked;
            set
            {
                Set(ref _isRotationToolChecked, value);

                if (IsRotationToolChecked && IsSaveEnabled)
                    IsRotationEnabled = true;
                else
                    IsRotationEnabled = false;

                if (!IsRotationToolChecked && Image != null && AngleValue != 0)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(new TransformedBitmap(Image, new RotateTransform(AngleValue)));
                    Image = _obCollection[++position];
                    AngleValue = 0;
                }
            }
        }
        #endregion

        #region Is Scale Tool Checked
        private bool _isScaleToolChecked;

        /// <summary>
        /// Is scale tool checked.
        /// </summary>
        public bool IsScaleToolChecked
        {
            get => _isScaleToolChecked;
            set
            {
                Set(ref _isScaleToolChecked, value);

                if (IsScaleToolChecked && IsSaveEnabled)
                    IsScaleEnabled = true;
                else
                    IsScaleEnabled = false;

                if ((SliderXValue != 100 || SliderYValue != 100) && Image != null)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue)));
                    Image = _obCollection[++position];
                    SliderXValue = 100;
                    SliderYValue = 100;
                }
            }
        }
        #endregion

        #region MainImage Scale X Value
        private double _scaleXValue = 1;

        /// <summary>
        /// ScaleX value of main image.
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
        /// ScaleY value of main image.
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
        /// Angle value of main image.
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
        /// Zoom value for scaling the image.
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
        /// Is "Save" and "Save as" buttons enabled menu.
        /// </summary>
        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set
            {
                Set(ref _isSaveEnabled, value);
            }
        }
        #endregion

        #region Is Rotation Enabled
        private bool _isRotationEnabled = false;

        /// <summary>
        /// Is Rotation Slider Enabled.
        /// </summary>
        public bool IsRotationEnabled
        {
            get => _isRotationEnabled;
            set
            {
                Set(ref _isRotationEnabled, value);
            }
        }
        #endregion

        #region Is Scale Enabled
        private bool _isScaleEnabled = false;

        /// <summary>
        /// Is Scale Slider Enabled.
        /// </summary>
        public bool IsScaleEnabled
        {
            get => _isScaleEnabled;
            set
            {
                Set(ref _isScaleEnabled, value);
            }
        }
        #endregion

        #region Slider X Value
        private int _sliderXValue = 100;

        /// <summary>
        /// Slider X value.
        /// </summary>
        public int SliderXValue
        {
            get => _sliderXValue;
            set
            {
                Set(ref _sliderXValue, value);
                ScaleXValue = SliderXValue / 100f;
                if (IsSaveEnabled && IsScaleChecked && SliderYValue != SliderXValue)
                    SliderYValue = SliderXValue;
            }
        }
        #endregion

        #region Slider Y Value
        private int _sliderYValue = 100;

        /// <summary>
        /// Slider Y value.
        /// </summary>
        public int SliderYValue
        {
            get => _sliderYValue;
            set
            {
                Set(ref _sliderYValue, value);
                ScaleYValue = SliderYValue / 100f;
                if (IsSaveEnabled && IsScaleChecked && SliderXValue != SliderYValue)
                    SliderXValue = SliderYValue;
            }
        }
        #endregion

        #region Editing mode of InkCanvas
        private InkCanvasEditingMode _inkCanvasEditingMode = EditingMode.None;

        /// <summary>
        /// Editing mode of InkCanvas.
        /// </summary>
        public InkCanvasEditingMode InkCanvasEditingMode
        {
            get => _inkCanvasEditingMode;
            set => Set(ref _inkCanvasEditingMode, value);
        }
        #endregion

        #region InkCanvas Default Drawing Attributes
        private DrawingAttributes _inkCanvasDefaultDrawingAttributes = new()
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };

        /// <summary>
        /// InkCanvas default drawing attributes.
        /// </summary>
        public DrawingAttributes InkCanvasDefaultDrawingAttributes
        {
            get => _inkCanvasDefaultDrawingAttributes;
            set => Set(ref _inkCanvasDefaultDrawingAttributes, value);
        }
        #endregion

        #region Is Select Tool Checked
        private bool _isSelectToolChecked;

        /// <summary>
        /// Is select tool checked.
        /// </summary>
        public bool IsSelectToolChecked
        {
            get => _isSelectToolChecked;
            set
            {
                Set(ref _isSelectToolChecked, value);
                if (IsSelectToolChecked)
                    InkCanvasEditingMode = EditingMode.Select;
            }
        }
        #endregion

        #region Is Pen Tool Checked
        private bool _isPenToolChecked;

        /// <summary>
        /// Is pen tool checked.
        /// </summary>
        public bool IsPenToolChecked
        {
            get => _isPenToolChecked;
            set
            {
                Set(ref _isPenToolChecked, value);
                if (IsPenToolChecked)
                    InkCanvasEditingMode = EditingMode.Ink;
            }
        }
        #endregion

        #region Is Eraser Tool Checked
        private bool _isEraserToolChecked;

        /// <summary>
        /// Is eraser tool checked.
        /// </summary>
        public bool IsEraserToolChecked
        {
            get => _isEraserToolChecked;
            set
            {
                Set(ref _isEraserToolChecked, value);
                if (IsEraserToolChecked)
                    InkCanvasEditingMode = EditingMode.EraseByStroke;
            }
        }
        #endregion

        #region Color Picker Selected Color
        private System.Windows.Media.Color _colorPickerSelectedColor = Colors.Black;

        /// <summary>
        /// Color picker selected color.
        /// </summary>
        public System.Windows.Media.Color ColorPickerSelectedColor
        {
            get => _colorPickerSelectedColor;
            set
            {
                Set(ref _colorPickerSelectedColor, value);
                InkCanvasDefaultDrawingAttributes.Color = ColorPickerSelectedColor;
            }
        }
        #endregion

        #region Is Paint Enabled
        private bool _isPaintEnabled = false;

        /// <summary>
        /// Is Paint Enabled.
        /// </summary>
        public bool IsPaintEnabled
        {
            get => _isPaintEnabled;
            set
            {
                Set(ref _isPaintEnabled, value);
            }
        }
        #endregion

        #region Is Brush Tool Checked
        private bool _isBrushToolChecked;

        /// <summary>
        /// Is brush tool checked
        /// </summary>
        public bool IsBrushToolChecked
        {
            get => _isBrushToolChecked;
            set
            {
                Set(ref _isBrushToolChecked, value);
                if (IsBrushToolChecked && IsSaveEnabled)
                {
                    IsPaintEnabled = true;
                    IsInkCanvasVisible = true;
                }
                else
                    IsPaintEnabled = false;
            }
        }
        #endregion

        #region Pen Tool Thickness Value
        private int _thicknessValue = 1;

        /// <summary>
        /// Pen Tool Thickness Value.
        /// </summary>
        public int ThicknessValue
        {
            get => _thicknessValue;
            set
            {
                Set(ref _thicknessValue, value);
                InkCanvasDefaultDrawingAttributes.Width = ThicknessValue;
                InkCanvasDefaultDrawingAttributes.Height = ThicknessValue;
            }
        }
        #endregion

        #region InkCanvas Strokes
        private StrokeCollection _inkCanvasStrokes;

        /// <summary>
        /// InkCanvas Stroke Collection.
        /// </summary>
        public StrokeCollection InkCanvasStrokes
        {
            get => _inkCanvasStrokes;
            set => Set(ref _inkCanvasStrokes, value);
        }
        #endregion

        #region Is InkCanvas Visible
        private bool _isInkCanvasVisible;

        /// <summary>
        /// Is InkCanvas Visible.
        /// </summary>
        public bool IsInkCanvasVisible
        {
            get => _isInkCanvasVisible;
            set => Set(ref _isInkCanvasVisible, value);
        }
        #endregion

        #region Is Crop Enabled
        private bool _isCropEnabled = false;

        /// <summary>
        /// Is Crop Enabled.
        /// </summary>
        public bool IsCropEnabled
        {
            get => _isCropEnabled;
            set => Set(ref _isCropEnabled, value);
        }
        #endregion

        #region Top Crop Value
        private int _topCropValue = 0;

        /// <summary>
        /// Top Crop Value.
        /// </summary>
        public int TopCropValue
        {
            get => _topCropValue;
            set
            {
                Set(ref _topCropValue, value);
                Image = CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue);
            }
        }
        #endregion

        #region Bottom Crop Value
        private int _bottomCropValue = 0;

        /// <summary>
        /// Bottom Crop Value.
        /// </summary>
        public int BottomCropValue
        {
            get => _bottomCropValue;
            set
            {
                Set(ref _bottomCropValue, value);
                Image = CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue);
            }
        }
        #endregion

        #region Right Crop Value
        private int _rightCropValue = 0;

        /// <summary>
        /// Right Crop Value.
        /// </summary>
        public int RightCropValue
        {
            get => _rightCropValue;
            set
            {
                Set(ref _rightCropValue, value);
                Image = CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue);
            }
        }
        #endregion

        #region Left Crop Value
        private int _leftCropValue = 0;

        /// <summary>
        /// Left Crop Value.
        /// </summary>
        public int LeftCropValue
        {
            get => _leftCropValue;
            set
            {
                Set(ref _leftCropValue, value);
                Image = CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue);
            }
        }
        #endregion

        #region Is Crop Tool Checked
        private bool _isCropToolChecked;

        /// <summary>
        /// Is crop tool checked
        /// </summary>
        public bool IsCropToolChecked
        {
            get => _isCropToolChecked;
            set
            {
                Set(ref _isCropToolChecked, value);
                if (IsCropToolChecked && IsSaveEnabled)
                    IsCropEnabled = true;
                else
                    IsCropEnabled = false;

                if (!IsCropToolChecked && Image != null && (TopCropValue != 0 || BottomCropValue != 0 || LeftCropValue != 0 || RightCropValue != 0))
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue));
                    Image = _obCollection[++position];
                    TopCropValue = BottomCropValue = RightCropValue = LeftCropValue = 0;
                }
            }
        }
        #endregion 

        #endregion

        #region Commands

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool OnCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            _imageStream?.Close();
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region OpenImageCommand
        public ICommand OpenImageCommand { get; }

        private bool OnOpenImageCommandExecute(object p) => true;

        private void OnOpenImageCommandExecuted(object p)
        {
            var open = new Microsoft.Win32.OpenFileDialog();
            open.Title = "Open an image";
            open.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG;*.TIFF;*.ICO)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG;*.TIFF;*.ICO";
            if (open.ShowDialog() == true)
            {
                _imageStream = new System.IO.MemoryStream(File.ReadAllBytes(open.FileName));
                string format = open.FileName.Substring(open.FileName.LastIndexOf('.') + 1);
                _format = ToImageFormat(format);
                _path = open.FileName;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = _imageStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                Image = bitmap;

                _obCollection.Clear();
                _obStrokeCollection.Clear();
                _obStrokeCollection.Add(new StrokeCollection());
                position = -1;
                strPos = -1;

                _obCollection.Add(Image);
                ++position;

                OnPropertyChanged(nameof(Image));

                ZoomValue = 0.7;
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
            var save = new Microsoft.Win32.SaveFileDialog();
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

            if (save.ShowDialog() == true)
            {
                string fileName = save.FileName;
                string chosenFormat = fileName.Substring(fileName.LastIndexOf(".") + 1);
                _saveFormat = _saveFormat == chosenFormat ? _saveFormat : chosenFormat;
                ImageFormat saveFormat = ToImageFormat(_saveFormat);
                SaveImage(fileName, saveFormat, p);
            }
        }
        #endregion

        #region SaveAsCommand
        public ICommand SaveAsCommand { get; }

        private bool OnSaveAsCommandExecute(object p) => true;

        private void OnSaveAsCommandExecuted(object p)
        {
            var save = new Microsoft.Win32.SaveFileDialog();
            save.Title = "Save image as ...";
            save.Filter = "PNG File(*.png)|*.png|" +
                "JPG File(*.jpg)|*.jpg|" +
                "GIF File(*.gif)|*.gif|" +
                "BMP File(*.bmp)|*.bmp|" +
                "TIFF File(*.tiff)|*.tiff|" +
                "ICO File(*.ico)|*.ico";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            save.FileName = "Untitled1";

            if (save.ShowDialog() == true)
            {
                string fileName = save.FileName;
                string chosenFormat = fileName.Substring(fileName.LastIndexOf(".") + 1);
                ImageFormat saveFormat = ToImageFormat(chosenFormat);
                SaveImage(fileName, saveFormat, p);
            }
        }
        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }

        private bool OnSaveCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            SaveImage(_path, _format, p);
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

        #region UndoCommand
        public ICommand UndoCommand { get; }

        private bool OnUndoCommandExecute(object p) => true;

        private void OnUndoCommandExecuted(object p)
        {
            if (position > 0 || strPos > 0)
            {
                if (IsBrushToolChecked && _obStrokeCollection.Count > 1 && strPos > 0)
                    InkCanvasStrokes = new StrokeCollection(_obStrokeCollection[--strPos]);
                else if (position > 0 && (!IsBrushToolChecked || (IsBrushToolChecked && _obStrokeCollection.Count == 1)))
                    Image = _obCollection[--position];
            }
        }
        #endregion

        #region RedoCommand
        public ICommand RedoCommand { get; }

        private bool OnRedoCommandExecute(object p) => true;

        private void OnRedoCommandExecuted(object p)
        {
            if (position < _obCollection.Count - 1 || strPos < _obStrokeCollection.Count - 1)
            {
                if (IsBrushToolChecked && _obStrokeCollection.Count > 1 && strPos < _obStrokeCollection.Count - 1)
                    InkCanvasStrokes = new StrokeCollection(_obStrokeCollection[++strPos]);
                else if ((position < _obCollection.Count - 1) && (!IsBrushToolChecked || (IsBrushToolChecked && _obStrokeCollection.Count == 1)))
                    Image = _obCollection[++position];
            }
        }
        #endregion

        #region IsBrushToolCheckedCommand
        public ICommand IsBrushToolCheckedCommand { get; }

        private bool OnIsBrushToolCheckedCommandExecute(object p) => true;

        private void OnIsBrushToolCheckedCommandExecuted(object p)
        {
            if (Image != null && _obStrokeCollection.Count > 1)
            {
                int count = _obCollection.Count;
                if (position != count - 1)
                    for (int i = count - 1; i > position; i--)
                        _obCollection.RemoveAt(i);

                InkCanvas? ink = p as InkCanvas;
                _obCollection.Add(ConvertInkCanvasToBitmapSource(ink));
                Image = _obCollection[++position];
                InkCanvasStrokes.Clear();
                _obStrokeCollection.Clear();
                _obStrokeCollection.Add(new StrokeCollection());
                strPos = 0;
            }
            IsInkCanvasVisible = false;
        }
        #endregion

        #region StrokeChangedCommand
        public ICommand StrokeChangedCommand { get; }

        private bool OnStrokeChangedCommandExecute(object p) => true;

        private void OnStrokeChangedCommandExecuted(object p)
        {
            int count = _obStrokeCollection.Count;
            if (strPos != count - 1)
                for (int i = count - 1; i > strPos; i--)
                    _obStrokeCollection.RemoveAt(i);

            _obStrokeCollection.Add(new StrokeCollection(InkCanvasStrokes));
            ++strPos;
        }
        #endregion

        #region ShowAboutWindowCommand
        public ICommand ShowAboutWindowCommand { get; }

        private bool OnShowAboutWindowCommandExecute(object p) => true;

        private void OnShowAboutWindowCommandExecuted(object p)
        {
            _windowService.OpenWindow();
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

        private Stream StreamFromBitmapSource(BitmapSource? writeBmp, ImageFormat? format)
        {
            Stream bmp = new MemoryStream();

            BitmapEncoder enc;

            if (format == ImageFormat.Jpeg)
                enc = new JpegBitmapEncoder();
            else if (format == ImageFormat.Gif)
                enc = new GifBitmapEncoder();
            else if (format == ImageFormat.Bmp)
                enc = new BmpBitmapEncoder();
            else if (format == ImageFormat.Tiff)
                enc = new TiffBitmapEncoder();
            else
                enc = new PngBitmapEncoder();

            enc.Frames.Add(BitmapFrame.Create(writeBmp));
            enc.Save(bmp);

            return bmp;
        }

        private void SaveImage(string path, ImageFormat? format, object? p = null)
        {
            if (_imageStream != null)
            {
                if (SliderXValue != 100 || SliderYValue != 100)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue)));
                    Image = _obCollection[++position];
                    SliderXValue = 100;
                    SliderYValue = 100;
                }
                if (AngleValue != 0)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(new TransformedBitmap(Image, new RotateTransform(AngleValue)));
                    Image = _obCollection[++position];
                    AngleValue = 0;
                }
                if (TopCropValue != 0 || BottomCropValue != 0 || LeftCropValue != 0 || RightCropValue != 0)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue));
                    Image = _obCollection[++position];
                    TopCropValue = BottomCropValue = RightCropValue = LeftCropValue = 0;
                }
                if (IsInkCanvasVisible && p is InkCanvas inkCanv && _obStrokeCollection.Count > 1)
                {
                    int count = _obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            _obCollection.RemoveAt(i);

                    _obCollection.Add(ConvertInkCanvasToBitmapSource(inkCanv));
                    Image = _obCollection[++position];
                    InkCanvasStrokes.Clear();
                    _obStrokeCollection.Clear();
                    _obStrokeCollection.Add(new StrokeCollection());
                    strPos = 0;
                }
                _imageStream = StreamFromBitmapSource(Image, format);
                var img = System.Drawing.Image.FromStream(_imageStream);
                format ??= ImageFormat.Png;
                if (format == ImageFormat.Icon)
                {
                    Bitmap bmp;
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        BitmapEncoder enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(Image));
                        enc.Save(outStream);
                        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                        bmp = new Bitmap(bitmap);
                    }
                    SaveAsIcon(bmp, path);
                }
                else
                    img.Save(path, format);

                _path = path;
                _imageStream = new System.IO.MemoryStream(File.ReadAllBytes(path));

                System.Windows.MessageBox.Show("Image saved successfully", "Image saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        void SaveAsIcon(Bitmap SourceBitmap, string FilePath)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create);
            // ICO header
            FS.WriteByte(0); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);
            FS.WriteByte(1); FS.WriteByte(0);

            // Image size
            FS.WriteByte((byte)SourceBitmap.Width);
            FS.WriteByte((byte)SourceBitmap.Height);
            // Palette
            FS.WriteByte(0);
            // Reserved
            FS.WriteByte(0);
            // Number of color planes
            FS.WriteByte(0); FS.WriteByte(0);
            // Bits per pixel
            FS.WriteByte(32); FS.WriteByte(0);

            // Data size, will be written after the data
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Offset to image data, fixed at 22
            FS.WriteByte(22);
            FS.WriteByte(0);
            FS.WriteByte(0);
            FS.WriteByte(0);

            // Writing actual data
            SourceBitmap.Save(FS, ImageFormat.Png);

            // Getting data length (file length minus header)
            long Len = FS.Length - 22;

            // Write it in the correct place
            FS.Seek(14, SeekOrigin.Begin);
            FS.WriteByte((byte)Len);
            FS.WriteByte((byte)(Len >> 8));

            FS.Close();
        }

        private BitmapSource ConvertInkCanvasToBitmapSource(InkCanvas? drawCanvas)
        {
            if (drawCanvas != null)
            {
                var rtb = new RenderTargetBitmap((int)drawCanvas.Width, (int)drawCanvas.Height, 96d, 96d, PixelFormats.Default);
                rtb.Render(drawCanvas);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                //save to memory stream or file 
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                encoder.Save(ms);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            else
                return new BitmapImage();
        }

        private CroppedBitmap CropImage(int left, int top, int right, int bottom)
        {
            left = (int)(_obCollection[position].PixelWidth * left / 100f);
            right = (int)(_obCollection[position].PixelWidth * right / 100f);
            top = (int)(_obCollection[position].PixelHeight * top / 100f);
            bottom = (int)(_obCollection[position].PixelHeight * bottom / 100f);
            return new CroppedBitmap(_obCollection[position], new Int32Rect(left, top, _obCollection[position].PixelWidth - left - right, _obCollection[position].PixelHeight - bottom - top));
        }

        #endregion

        public MainWindowViewModel(IWindowService windowService)
        {
            _windowService = windowService;
            _obCollection = new ObservableCollection<BitmapSource>();
            _inkCanvasStrokes = new StrokeCollection();
            _obStrokeCollection = new ObservableCollection<StrokeCollection>
            {
                new StrokeCollection()
            };
            strPos = 0;

            #region Commands

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, OnCloseApplicationCommandExecute);

            OpenImageCommand = new LambdaCommand(OnOpenImageCommandExecuted, OnOpenImageCommandExecute);

            ZoomInImageCommand = new LambdaCommand(OnZoomInImageCommandExecuted, OnZoomInImageCommandExecute);

            ZoomOutImageCommand = new LambdaCommand(OnZoomOutImageCommandExecuted, OnZoomOutImageCommandExecute);

            ChangeSaveImageFormatCommand = new LambdaCommand(OnChangeSaveImageFormatCommandExecuted, OnChangeSaveImageFormatCommandExecute);

            ConvertImageCommand = new LambdaCommand(OnConvertImageCommandExecuted, OnConvertImageCommandExecute);

            SaveAsCommand = new LambdaCommand(OnSaveAsCommandExecuted, OnSaveAsCommandExecute);

            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, OnSaveCommandExecute);

            UndoCommand = new LambdaCommand(OnUndoCommandExecuted, OnUndoCommandExecute);

            RedoCommand = new LambdaCommand(OnRedoCommandExecuted, OnRedoCommandExecute);

            StrokeChangedCommand = new LambdaCommand(OnStrokeChangedCommandExecuted, OnStrokeChangedCommandExecute);

            IsBrushToolCheckedCommand = new LambdaCommand(OnIsBrushToolCheckedCommandExecuted, OnIsBrushToolCheckedCommandExecute);

            ShowAboutWindowCommand = new LambdaCommand(OnShowAboutWindowCommandExecuted, OnShowAboutWindowCommandExecute);

            #endregion
        }
    }
}
