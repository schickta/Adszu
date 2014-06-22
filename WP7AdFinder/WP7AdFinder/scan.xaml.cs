using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Devices;
using System.Windows.Media.Imaging;

using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode;
using com.google.zxing.oned;

namespace WP7AdFinder
{
    public partial class scan : PhoneApplicationPage
    {
        PhotoCamera camera;
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<string> _matches;
        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _qrreader;
        private Code128Reader _code128reader;
        private Code39Reader _code39reader;
        private ITFReader _itfreader;

        public scan()
        {
            InitializeComponent();

            // initialize the timer and buffer for the scanner
            _matches = new ObservableCollection<string>();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (camera == null)
            {
                //TAS: With dual cameras, mine worked with Primary.
                //if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                //    camera = new PhotoCamera(CameraType.FrontFacing);
                //else
                camera = new PhotoCamera(CameraType.Primary);
                CameraSource.SetSource(camera);

                camera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(camera_Initialized);
                CameraButtons.ShutterKeyHalfPressed += (o, arg) => camera.Focus();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            CloseCamera();
        }

        private void ApplicationBarIconButton_Click_Home (object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Tag (object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/tag.xaml", UriKind.Relative));
        }

        // These are low-level internal functions for the camera barcode scanner.

        private void CloseCamera()
        {
            if (camera != null)
            {
                camera.Dispose();
                camera = null;
            }
        }

        void camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(camera.PreviewResolution.Width);
            int height = Convert.ToInt32(camera.PreviewResolution.Height);
            _luminance = new PhotoCameraLuminanceSource(width, height);

            _qrreader = new QRCodeReader();
            _code128reader = new Code128Reader();
            _code39reader = new Code39Reader();
            _itfreader = new ITFReader();

            Dispatcher.BeginInvoke(() =>
            {
                _previewTransform.Rotation = camera.Orientation;
                _timer.Start();
            });
        }

        public class PhotoCameraLuminanceSource : LuminanceSource
        {
            public byte[] PreviewBufferY { get; private set; }
            public PhotoCameraLuminanceSource(int width, int height)
                : base(width, height)
            {
                PreviewBufferY = new byte[width * height];
            }
            public override sbyte[] Matrix
            {
                get { return (sbyte[])(Array)PreviewBufferY; }
            }
            public override sbyte[] getRow(int y, sbyte[] row)
            {
                if (row == null || row.Length < Width)
                {
                    row = new sbyte[Width];
                }

                for (int i = 0; i < Height; i++)
                    row[i] = (sbyte)PreviewBufferY[i * Width + y];
                return row;
            }
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                camera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);

                try
                {   
                    var result = _qrreader.decode(binBitmap);
                    Dispatcher.BeginInvoke(() => DisplayResult(result.Text));

                }
                catch
                {
                    try
                    {
                        var result = _itfreader.decode(binBitmap);
                        Dispatcher.BeginInvoke(() => DisplayResult("ITF" + result.Text));
                    }
                    catch
                    {
                        try
                        {
                            var result = _code128reader.decode(binBitmap);
                            Dispatcher.BeginInvoke(() => DisplayResult("CODE128" + result.Text));
                        }
                        catch
                        {
                            try
                            {
                                var result = _code39reader.decode(binBitmap);
                                Dispatcher.BeginInvoke(() => DisplayResult("CODE39" + result.Text));
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }

        private void DisplayResult(string text)
        {
            if (text.Length > 0)
            {
                CloseCamera();

                App myApp = (App)Application.Current;
                myApp.CIQTag = text;
                NavigationService.Navigate(new Uri("/tag.xaml", UriKind.Relative));
            }
        }
    }
}