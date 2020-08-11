using NAudio.Wave;
using Yugen.DJ.Audio.WaveForm.Models;

namespace Yugen.DJ.Audio.WaveForm.Interfaces
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);

        PeakInfo GetNextPeak();
    }
}