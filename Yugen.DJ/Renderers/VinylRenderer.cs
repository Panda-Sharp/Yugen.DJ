using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Yugen.DJ.Helpers;

namespace Yugen.DJ.Renderers
{
    public class VinylRenderer
    {
        private CanvasBitmap _vinylBitmap;

        private float _width = 1000;
        private float _height = 1000;
        private float _angle = 0;
        private bool _isTouched;

        public async Task CreateResourcesAsync(CanvasAnimatedControl sender) =>
                    _vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Images/Vinyl.png");

        public void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args, bool isPaused, TimeSpan position, double pitch)
        {
            var ds = args.DrawingSession;
            ds.Transform = RendererHelper.CalculateLayout(sender.Size, _width, _height);

            //if (!isPaused)
            //{
            //    _angle += 0.1f;
            //}
            if (_isTouched)
            {
                //Draw(sender, ds, _angle);
            }
            else
            {
                _angle = TimeToAngle(position, pitch);
            }

            try
            {
                var originalImageRect = _vinylBitmap.GetBounds(ds);
                var endpoint = new Vector2((float)originalImageRect.Width, (float)originalImageRect.Height) / 2;

                ds.Clear(Colors.Transparent);

                ICanvasImage image = new Transform2DEffect
                {
                    Source = _vinylBitmap,
                    TransformMatrix = Matrix3x2.CreateRotation(_angle, endpoint)
                };

                var offset = sender.Size.ToVector2() * ds.Transform.M11 / 2;
                ds.DrawImage(image, offset);
            }
            catch { }
        }

        private float TimeToAngle(TimeSpan timingInformation, double pitch)
        {
            var fractionSecond = (double)timingInformation.Milliseconds / 1000;
            var fractionSecondAngle = (float)(2 * Math.PI * fractionSecond);
            var angle = (float)(fractionSecondAngle % (2 * Math.PI));

            var pitchRatio = ((float)pitch + 51) / 10;

            angle += pitchRatio;

            return angle;
        }

        public void PointerPressed(object sender, PointerRoutedEventArgs e) => 
            _isTouched = true;

        public void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasAnimatedControl &&
                _isTouched)
            {
                PointerPoint currentLocation = e.GetCurrentPoint(canvasAnimatedControl);

                var dialCenter = new Point(canvasAnimatedControl.ActualHeight / 2, canvasAnimatedControl.ActualWidth / 2);

                // Calculate an angle
                var radians = Math.Atan((currentLocation.Position.Y - dialCenter.Y) /
                                           (currentLocation.Position.X - dialCenter.X));

                // in order to get these figures to work, I actually had to *add* 90 degrees to it,
                // and *subtract* 180 from it if the X coord is negative.
                var x = radians * 180 / Math.PI + 90;
                if (currentLocation.Position.X - dialCenter.X < 0)
                {
                    x -= 180;
                }

                _angle = (float)x / 100;
            }
        }

        public void PointerReleased(object sender, PointerRoutedEventArgs e) => 
            _isTouched = false;


        //public void Draw(ICanvasAnimatedControl sender, CanvasTimingInformation timingInformation, CanvasDrawingSession ds)
        //{
        //    var updatesPerSecond = 1000.0 / sender.TargetElapsedTime.TotalMilliseconds;
        //    var seconds = (int)(timingInformation.UpdateCount / updatesPerSecond % 10);
        //    var updates = (double)timingInformation.UpdateCount;
        //    var fractionSecond = updates / updatesPerSecond % 1.0;
        //    var Angle = (float)Math.PI * (seconds / 10.0f) * 2.0f;
        //    Rotate(ds, fractionSecond);
        //}

        //public void Rotate(CanvasDrawingSession ds, double fractionSecond)
        //{
        //    var fractionSecondAngle = (float)(2 * Math.PI * fractionSecond);
        //    var angle = (float)(fractionSecondAngle % (2 * Math.PI));

        //    try
        //    {
        //        var originalImageRect = _vinylBitmap.GetBounds(ds);
        //        var endpoint = new Vector2((float)originalImageRect.Width / 2, (float)originalImageRect.Height / 2);

        //        ds.Clear(Colors.Transparent);

        //        ICanvasImage image = new Transform2DEffect
        //        {
        //            Source = _vinylBitmap,
        //            TransformMatrix = Matrix3x2.CreateRotation(angle, endpoint)
        //        };

        //        var sourceRect = image.GetBounds(ds);
        //        ds.DrawImage(image);
        //    }
        //    catch
        //    {
        //    }
        //}
    }
}