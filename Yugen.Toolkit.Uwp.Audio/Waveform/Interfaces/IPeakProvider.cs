using NAudio.Wave;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);

        PeakInfo GetNextPeak();
    }
}