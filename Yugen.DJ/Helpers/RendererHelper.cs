using System.Numerics;
using Windows.Foundation;

namespace Yugen.DJ.Helpers
{
    public static class RendererHelper
    {
        public static Matrix3x2 CalculateLayout(Size size, float width, float height)
        {
            var targetWidth = (float)size.Width;
            var targetHeight = (float)size.Height;
            var scaleFactor = targetWidth / width;

            if (height * scaleFactor > targetHeight)
            {
                scaleFactor = targetHeight / height;
            }

            return Matrix3x2.CreateScale(scaleFactor, scaleFactor);
        }
    }
}