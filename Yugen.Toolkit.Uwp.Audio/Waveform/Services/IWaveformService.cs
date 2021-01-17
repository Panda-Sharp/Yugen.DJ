using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NAudio.Wave;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    public interface IWaveformRendererService
    {
        void DrawLine(CanvasControl sender, CanvasDrawingSession ds);
        void Render(ISampleProvider isp, long samples);
    }
}