using System;
using Windows.UI;

namespace Yugen.Toolkit.Uwp.Audio.Helpers
{
    public static class ColorHelper
    {
        public static Color GradientColor(float mu)
        {
            var c = (byte)((Math.Sin(mu * Math.PI * 2) + 1) * 127.5);

            return Color.FromArgb(255, (byte)(255 - c), c, 220);
        }
    }
}