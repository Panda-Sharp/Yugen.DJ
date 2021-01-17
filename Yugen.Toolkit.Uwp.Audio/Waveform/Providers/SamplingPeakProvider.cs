using System;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Providers
{
    public class SamplingPeakProvider : PeakProvider
    {
        private readonly int sampleInterval;

        public SamplingPeakProvider(int sampleInterval)
        {
            this.sampleInterval = sampleInterval;
        }

        public override PeakInfo GetNextPeak()
        {
            var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);
            var max = 0.0f;
            var min = 0.0f;
            for (var x = 0; x < samplesRead; x += sampleInterval)
            {
                max = Math.Max(max, ReadBuffer[x]);
                min = Math.Min(min, ReadBuffer[x]);
            }

            return new PeakInfo(min, max);
        }
    }
}