using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Net.Http;

namespace wallhaven
{
    public partial class ViewWallpaper : PhoneApplicationPage
    {
        const double MaxScale = 10;

        double _scale = 1.0;
        double _minScale;
        double _coercedScale;
        double _originalScale;

        bool _pinching;
        Point _screenMidpoint;
        Point _relativeMidpoint;

        BitmapImage _bitmap;

        // These are the variables that I defined to help deal with the limitations of the 
        // CacheMode property.
        BitmapImage temp;
        double width, height;

        public ViewWallpaper()
        {
            InitializeComponent();

            // Loads the image so that I can get the width and height.
            temp = new BitmapImage(App.wallHavenModel.SelectedWallpaper.OriginalImageUri);
            temp.CreateOptions = BitmapCreateOptions.None;
            temp.ImageOpened += temp_ImageOpened;
        }

        private void temp_ImageOpened(object sender, RoutedEventArgs e)
        {
            // It's only possible to get the height and width if the image has been opened.
            BitmapImage image = sender as BitmapImage;
            width = image.PixelWidth;
            height = image.PixelHeight;

            // 3840 x 2160
            // If I reuse the BitmapImage variable from the Mainpage() the zoom does not work. 
            // It's because when reusing the variable the ImageOpened event of the SelectedImageViewer is not fired. 
            // ... which is why I'm loading the image again from another variable.
            //
            // One possible workaround is to create a function that contains the code from the ImageOpened event.
            // But then, the DecodePixelWidth is not working - the image is partially cut off again. 
            BitmapImage bitmapImage = new BitmapImage(App.wallHavenModel.SelectedWallpaper.OriginalImageUri);
            // Setting the DecodePixel value depending on the dimension of the image.
            // >= is used to account for images that have the same height and width.
            if (width > 2048 && width >= height)
            {
                bitmapImage.DecodePixelWidth = 1500;
            }   
            else if (height > 2048 && height > width)
            {
                bitmapImage.DecodePixelHeight = 1500;
            }
            SelectedImageViewer.Source = bitmapImage;
        }

        private void SelectedImageViewer_ImageOpened(object sender, RoutedEventArgs e)
        {
            _bitmap = (BitmapImage)SelectedImageViewer.Source;

            // Set scale to the minimum, and then save it. 
            _scale = 0;
            CoerceScale(true);
            _scale = _coercedScale;
            ResizeImage(true);

            // Hiding the loading bar after the image has loaded and properly set.
            GettingOriginalImageOverlay.Visibility = Visibility.Collapsed;
        }


        private void OnManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _pinching = false;
            _originalScale = _scale;
        }

        private void OnManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!_pinching)
                {
                    _pinching = true;
                    Point center = e.PinchManipulation.Original.Center;
                    _relativeMidpoint = new Point(center.X / SelectedImageViewer.ActualWidth, center.Y / SelectedImageViewer.ActualHeight);


                    var xform = SelectedImageViewer.TransformToVisual(Viewport);
                    _screenMidpoint = xform.Transform(center);
                }

                _scale = _originalScale * e.PinchManipulation.CumulativeScale;
                CoerceScale(false);
                ResizeImage(false);

            }
            else if (_pinching)
            {
                _pinching = false;
                _originalScale = _scale = _coercedScale;
            }
        }

        private void OnManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            _pinching = false;
            _scale = _coercedScale;
        }

        void ResizeImage(bool center)
        {
            if (_coercedScale != 0 && _bitmap != null)
            {
                double newWidth = Canvas.Width = Math.Round(_bitmap.PixelWidth * _coercedScale);
                double newHeight = Canvas.Height = Math.Round(_bitmap.PixelHeight * _coercedScale);

                xform.ScaleX = xform.ScaleY = _coercedScale;

                Viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

                if (center)
                {
                    Viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - Viewport.ActualWidth) / 2),
                            Math.Round((newHeight - Viewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    Point newImgMid = new Point(newWidth * _relativeMidpoint.X, newHeight * _relativeMidpoint.Y);
                    Point origin = new Point(newImgMid.X - _screenMidpoint.X, newImgMid.Y - _screenMidpoint.Y);
                    Viewport.SetViewportOrigin(origin);
                }
            }
        }

        void CoerceScale(bool recompute)
        {
            if (recompute && _bitmap != null && Viewport != null)
            {
                // Calculate the minimum scale to fit the viewport 
                double minX = Viewport.ActualWidth / _bitmap.PixelWidth;
                double minY = Viewport.ActualHeight / _bitmap.PixelHeight;

                _minScale = Math.Min(minX, minY);
            }
            _coercedScale = Math.Min(MaxScale, Math.Max(_scale, _minScale));
        }

    }
}