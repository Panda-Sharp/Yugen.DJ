namespace Yugen.DJ.Audio.WaveForm.Models
{
    public class WaveFormRendererSettings
    {
        public int Width { get; set; } = 500;
        public int TopHeight { get; set; } = 50;
        public int BottomHeight { get; set; } = 50;
        public int PixelsPerPeak { get; set; } = 1;
        public int SpacerPixels { get; set; } = 0;
        public bool DecibelScale { get; set; }
    }
}