using NAudio.Wave;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Providers
{
    /// <summary>
    /// The peak provider decides how peaks are calculated. There are four built in options you can choose from.
    /// MaxPeakProvider simply picks out the maximum sample value in the timeblock that each bar represents.
    /// RmsPeakProvider calculates the root mean square of each sample and returns the maximum value found in a specified blcok.
    /// SamplingPeakProvider simply samples the samples, and you pass in a sample interval.
    /// AveragePeakProvider averages the sample values and takes a scale parameter to multiply the average by as it tends to produce lower values.
    /// </summary>
    public abstract class PeakProvider : IPeakProvider
    {
        protected ISampleProvider Provider { get; private set; }
        protected int SamplesPerPeak { get; private set; }
        protected float[] ReadBuffer { get; private set; }

        public void Init(ISampleProvider provider, int samplesPerPeak)
        {
            Provider = provider;
            SamplesPerPeak = samplesPerPeak;
            ReadBuffer = new float[samplesPerPeak];
        }

        public abstract PeakInfo GetNextPeak();
    }
}