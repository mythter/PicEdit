using PicEdit.Infrastructure.Commands;
using PicEdit.ViewModels.Base;
using System;
using System.Windows;
using Forms = System.Windows.Forms;
using EditingMode = System.Windows.Controls.InkCanvasEditingMode;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Forms;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Collections;
using System.Windows.Media.Media3D;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Application = System.Windows.Application;
using PicEdit.Services;

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

        private IWindowService _windowService;

        Stream? imageStream;
        ObservableCollection<BitmapSource> obCollection;
        int position = -1;
        ObservableCollection<StrokeCollection> obStrokeCollection;
        int strPos = -1;

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

        #region MainImage Format
        /// <summary>
        /// Format of the main image.
        /// </summary>
        private ImageFormat? _format;
        #endregion

        #region MainImage Save Format
        /// <summary>
        /// Format to save the main image.
        /// </summary>
        private string _saveFormat = "png";
        #endregion

        #region MainImage Path
        /// <summary>
        /// Path of the main image.
        /// </summary>
        private string _path = "";
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
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(new TransformedBitmap(Image, new RotateTransform(AngleValue)));
                    Image = obCollection[++position];
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
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue)));
                    Image = obCollection[++position];
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
                //Image = CropImage(LeftCropValue, 0, 0, 0);
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
                //Image = CropImage(LeftCropValue, 0, 0, 0);
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
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue));
                    Image = obCollection[++position];
                    TopCropValue = BottomCropValue = RightCropValue = LeftCropValue = 0;
                }
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
            System.Windows.Application.Current.Shutdown();
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
                _path = open.FileName;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = imageStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                Image = bitmap;

                obCollection.Add(Image);
                ++position;

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
                if (IsBrushToolChecked && obStrokeCollection.Count > 1 && strPos > 0)
                    InkCanvasStrokes = new StrokeCollection(obStrokeCollection[--strPos]);
                else if (position > 0 && (!IsBrushToolChecked || (IsBrushToolChecked && obStrokeCollection.Count == 1)))
                    Image = obCollection[--position];
            }
        }
        #endregion

        #region RedoCommand
        public ICommand RedoCommand { get; }

        private bool OnRedoCommandExecute(object p) => true;

        private void OnRedoCommandExecuted(object p)
        {
            if (position < obCollection.Count - 1 || strPos < obStrokeCollection.Count - 1)
            {
                if (IsBrushToolChecked && obStrokeCollection.Count > 1 && strPos < obStrokeCollection.Count - 1)
                    InkCanvasStrokes = new StrokeCollection(obStrokeCollection[++strPos]);
                else if ((position < obCollection.Count - 1) && (!IsBrushToolChecked || (IsBrushToolChecked && obStrokeCollection.Count == 1)))
                    Image = obCollection[++position];
            }
        }
        #endregion

        #region IsBrushToolCheckedCommand
        public ICommand IsBrushToolCheckedCommand { get; }

        private bool OnIsBrushToolCheckedCommandExecute(object p) => true;

        private void OnIsBrushToolCheckedCommandExecuted(object p)
        {
            if (Image != null && obStrokeCollection.Count > 1)
            {
                int count = obCollection.Count;
                if (position != count - 1)
                    for (int i = count - 1; i > position; i--)
                        obCollection.RemoveAt(i);

                InkCanvas? ink = p as InkCanvas;
                obCollection.Add(ConvertInkCanvasToBitmapSource(ink));
                Image = obCollection[++position];
                InkCanvasStrokes.Clear();
                obStrokeCollection.Clear();
                obStrokeCollection.Add(new StrokeCollection());
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
            int count = obStrokeCollection.Count;
            if (strPos != count - 1)
                for (int i = count - 1; i > strPos; i--)
                    obStrokeCollection.RemoveAt(i);

            obStrokeCollection.Add(new StrokeCollection(InkCanvasStrokes));
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

        private Stream StreamFromBitmapSource(BitmapSource? writeBmp)
        {
            Stream bmp = new MemoryStream();

            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(writeBmp));
            enc.Save(bmp);

            return bmp;
        }

        private void SaveImage(string path, ImageFormat? format, object? p = null)
        {
            if (imageStream != null)
            {
                if (SliderXValue != 100 || SliderYValue != 100)
                {
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(new TransformedBitmap(Image, new ScaleTransform(ScaleXValue, ScaleYValue)));
                    Image = obCollection[++position];
                    SliderXValue = 100;
                    SliderYValue = 100;
                }
                if (AngleValue != 0)
                {
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(new TransformedBitmap(Image, new RotateTransform(AngleValue)));
                    Image = obCollection[++position];
                    AngleValue = 0;
                }
                if (TopCropValue != 0 || BottomCropValue != 0 || LeftCropValue != 0 || RightCropValue != 0)
                {
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(CropImage(LeftCropValue, TopCropValue, RightCropValue, BottomCropValue));
                    Image = obCollection[++position];
                    TopCropValue = BottomCropValue = RightCropValue = LeftCropValue = 0;
                }
                if (IsInkCanvasVisible && p is InkCanvas inkCanv && obStrokeCollection.Count > 1)
                {
                    int count = obCollection.Count;
                    if (position != count - 1)
                        for (int i = count - 1; i > position; i--)
                            obCollection.RemoveAt(i);

                    obCollection.Add(ConvertInkCanvasToBitmapSource(inkCanv));
                    Image = obCollection[++position];
                    InkCanvasStrokes.Clear();
                    obStrokeCollection.Clear();
                    obStrokeCollection.Add(new StrokeCollection());
                    strPos = 0;
                }
                imageStream = StreamFromBitmapSource(Image);
                var img = System.Drawing.Image.FromStream(imageStream);
                format ??= ImageFormat.Png;
                img.Save(path, format);
                System.Windows.MessageBox.Show("Image saved successfully", "Image saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private BitmapSource ConvertInkCanvasToBitmapSource(InkCanvas? drawCanvas)
        {
            //string newImagePath = "./strokes.png";
            //InkCanvas inkCanvas = drawCanvas;
            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = obCollection[obCollection.Count - 1];
            //drawCanvas.Background = imageBrush;

            //using (FileStream fs = new FileStream(newImagePath, FileMode.Create))
            //{
            //    var rtb = new RenderTargetBitmap((int)drawCanvas.Width, (int)drawCanvas.Height, 96d, 96d, PixelFormats.Default);
            //    rtb.Render(drawCanvas);
            //    PngBitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create(rtb));
            //    encoder.Save(fs);
            //}
            //return new BitmapImage();
            //creating temporary InkCanvas
            //InkCanvas inkCanvas = drawCanvas;
            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = obCollection[obCollection.Count - 1];
            //drawCanvas.Background = imageBrush;
            //inkCanvas.Width = Image.Width;
            //inkCanvas.Height = Image.Height;

            //render bitmap
            //RenderTargetBitmap rtb = new RenderTargetBitmap((int)drawCanvas.Width, (int)drawCanvas.Height, 96, 96, PixelFormats.Default);
            //rtb.Render(drawCanvas);
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(rtb));
            //rtb.Render(drawCanvas);

            if(drawCanvas != null)
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

            //create bitmap with memory stream or file
            //Bitmap bitmap = new Bitmap(ms);
            //return ConvertBitmapToBitmapSource(bitmap);
        }

        public static byte[]? ConvertWriteableBitmapToByteArray(WriteableBitmap wb)
        {
            if (wb == null || wb.PixelHeight == 0 || wb.PixelWidth == 0)
                return null;

            int stride = wb.PixelWidth * wb.Format.BitsPerPixel / 8;
            int size = stride * wb.PixelHeight;

            byte[] buffer = new byte[size];

            wb.CopyPixels(buffer, stride, 0);

            return buffer;
        }

        private CroppedBitmap CropImage(int left, int top, int right, int bottom)
        {
            left = (int)(obCollection[position].PixelWidth * left / 100f);
            right = (int)(obCollection[position].PixelWidth * right / 100f);
            top = (int)(obCollection[position].PixelHeight * top / 100f);
            bottom = (int)(obCollection[position].PixelHeight * bottom / 100f);
            return new CroppedBitmap(obCollection[position], new Int32Rect(left, top, obCollection[position].PixelWidth - left - right, obCollection[position].PixelHeight - bottom - top));
        }

        //private BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        //{
        //    BitmapImage bmImage = new BitmapImage();
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        PngBitmapEncoder encoder = new PngBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(wbm));
        //        encoder.Save(stream);
        //        bmImage.BeginInit();
        //        bmImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bmImage.StreamSource = stream;
        //        bmImage.EndInit();
        //        bmImage.Freeze();
        //    }
        //    return bmImage;
        //}

        //private byte[] ConvertBitmapSourceToByteArray(BitmapSource bitmapSource)
        //{
        //    int width = bitmapSource.PixelWidth;
        //    int height = bitmapSource.PixelHeight;
        //    int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);

        //    byte[] bitmapData = new byte[height * stride];

        //    bitmapSource.CopyPixels(bitmapData, stride, 0);
        //    return bitmapData;
        //}

        //private WriteableBitmap ChangeBrightness(WriteableBitmap source, byte change_value)
        //{
        //    WriteableBitmap dest = new WriteableBitmap(source);

        //    byte[] color = new byte[4];

        //    using (Stream s = new MemoryStream(ConvertWriteableBitmapToByteArray(source)))
        //    {
        //        using (Stream d = new MemoryStream(ConvertWriteableBitmapToByteArray(dest)))
        //        {
        //            // read the pixel color
        //            while (s.Read(color, 0, 4) > 0)
        //            {
        //                // color[0] = b
        //                // color[1] = g 
        //                // color[2] = r
        //                // color[3] = a

        //                // do the adding algo per byte (skip the alpha)
        //                for (int i = 0; i < 4; i++)
        //                {
        //                    if (color[i] + change_value > 255) color[i] = 255; else color[i] = (byte)(color[i] + change_value);
        //                }

        //                // write the new pixel color
        //                d.Write(color, 0, 4);
        //            }
        //        }
        //    }
        //    //using (FileStream stream = new FileStream("./test.png", FileMode.Create))
        //    //{
        //    //    PngBitmapEncoder encoder = new PngBitmapEncoder();
        //    //    encoder.Frames.Add(BitmapFrame.Create(dest.Clone()));
        //    //    encoder.Save(stream);
        //    //}
        //    // return the new bitmap
        //    return dest;
        //}

        //public Bitmap AdjustBrightnessContrast(System.Drawing.Image image, int contrastValue, int brightnessValue)
        //{
        //    float brightness = -(brightnessValue / 100.0f);
        //    float contrast = contrastValue / 100.0f;
        //    var bitmap = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    using (var g = Graphics.FromImage(bitmap))
        //        using (var attributes = new ImageAttributes())
        //        {
        //            float[][] matrix = {
        //                new float[] { contrast, 0, 0, 0, 0},
        //                new float[] {0, contrast, 0, 0, 0},
        //                new float[] {0, 0, contrast, 0, 0},
        //                new float[] {0, 0, 0, 1, 0},
        //                new float[] {brightness, brightness, brightness, 1, 1}
        //        };

        //        ColorMatrix colorMatrix = new ColorMatrix(matrix);
        //        attributes.SetColorMatrix(colorMatrix);
        //        g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //            0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        //        return bitmap;
        //    }
        //}

        //private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        //{
        //    System.Drawing.Bitmap bitmap;
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        PngBitmapEncoder enc = new PngBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapsource));
        //        enc.Save(outStream);
        //        bitmap = new System.Drawing.Bitmap(outStream);
        //    }
        //    return bitmap;
        //}

        //private static BitmapImage BitmapToSource(Bitmap src)
        //{
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    src.Save(ms, ImageFormat.Jpeg);

        //    BitmapImage image = new BitmapImage();
        //    image.BeginInit();
        //    ms.Seek(0, System.IO.SeekOrigin.Begin);
        //    image.StreamSource = ms;
        //    image.EndInit();
        //    return image;
        //}

        //private static BitmapSource ConvertBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        //{
        //    //var bitmapData = bitmap.LockBits(
        //    //    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //    //    System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

        //    //var bitmapSource = BitmapSource.Create(
        //    //    bitmapData.Width, bitmapData.Height,
        //    //    bitmap.HorizontalResolution, bitmap.VerticalResolution,
        //    //    PixelFormats.Bgr24, null,
        //    //    bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        //    //bitmap.UnlockBits(bitmapData);

        //    //return bitmapSource;

        //    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
        //      bitmap.GetHbitmap(),
        //      IntPtr.Zero,
        //      Int32Rect.Empty,
        //      BitmapSizeOptions.FromEmptyOptions());
        //}



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

        public MainWindowViewModel(IWindowService windowService)
        {

            _windowService = windowService;

            obCollection = new ObservableCollection<BitmapSource>();
            obStrokeCollection = new ObservableCollection<StrokeCollection>
            {
                new StrokeCollection()
            };
            strPos = 0;
            _inkCanvasStrokes = new StrokeCollection();

            //(_inkCanvasStrokes as INotifyCollectionChanged).CollectionChanged += delegate
            //{
            //    obCollection.Add(ConvertInkCanvasToBitmapSource());
            //    Image = obCollection[++position];
            //};

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
