using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.DJ.Audio.WaveForm;

namespace Yugen.DJ.Audio.BPM
{
    /// <summary>
    /// https://github.com/matixmatix/bpmdetector/blob/master/BPMDetector.cs
    /// </summary>
    public class BPMDetector
    {
        public double BPM { get; private set; }

        public async Task<double> Detect(IStorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            return Detect(stream);
        }

        public double Detect(Stream stream)
        {
            var buffer = new float[0];
            var sampleRate = 0;
            var totalMinutes = 0d;

            using (var reader = new StreamMediaFoundationReader(stream))
            {
                ISampleProvider isp = reader.ToSampleProvider();
                buffer = new float[reader.Length / 2];
                isp.Read(buffer, 0, buffer.Length);

                sampleRate = isp.WaveFormat.SampleRate;
                totalMinutes = reader.TotalTime.TotalMinutes;
            }

            return Detect(buffer, sampleRate, totalMinutes);
        }

        //var chan1 = new float[buffer.Length / 2];
        //var chan2 = new float[buffer.Length / 2];
        //for (int i = 0; i < buffer.Length; i += 2)
        //{
        //    chan1[i] = buffer[i];
        //    chan2[i] = buffer[i + 1];
        //}
        //trackLength = (float)leftChn.Length / isp.WaveFormat.SampleRate;
        // 0.1s window ... 0.1*44100 = 4410 samples (lets adjust this to 3600)
        //var sampleStep = 3600;

        public double Detect(float[] buffer, int sampleRate, double totalMinutes)
        {
            // 0.1s window EG: 0.1*44100 = 4410 samples
            var sampleStep = (int)(0.1 * sampleRate);

            // calculate energy over windows of size sampleSetep
            var energies = new List<double>();
            for (var i = 0; i < buffer.Length - sampleStep - 1; i += sampleStep)
            {
                energies.Add(RangeQuadSum(buffer, i, i + sampleStep));
            }

            var beats = 0;
            double average = 0;
            double sumOfSquaresOfDifferences = 0;
            double variance = 0;
            double newC = 0;
            var variances = new List<double>();

            // how many energies before and after index for local energy average
            var offset = 10;

            for (var i = offset; i <= energies.Count - offset - 1; i++)
            {
                // calculate local energy average
                var currentEnergy = energies[i];
                var qwe = RangeSum(energies.ToArray(), i - offset, i - 1)
                    + currentEnergy
                    + RangeSum(energies.ToArray(), i + 1, i + offset);
                qwe /= offset * 2 + 1;

                // calculate energy variance of nearby energies
                var nearbyEnergies = energies.Skip(i - 5).Take(5)
                    .Concat(energies.Skip(i + 1).Take(5)).ToList();
                average = nearbyEnergies.Average();
                sumOfSquaresOfDifferences = nearbyEnergies
                    .Select(val => (val - average) * (val - average)).Sum();
                variance = sumOfSquaresOfDifferences / nearbyEnergies.Count / Math.Pow(10, 22);

                // experimental linear regression - constant calculated according to local energy variance
                newC = variance * 0.009 + 1.385;
                if (currentEnergy > newC * qwe)
                    beats++;
            }

            BPM = beats / totalMinutes / 2;
            return BPM;
        }

        private static double RangeQuadSum(float[] samples, int start, int stop)
        {
            double tmp = 0;
            for (var i = start; i <= stop; i++)
            {
                tmp += Math.Pow(samples[i], 2);
            }
            return tmp;
        }

        private static double RangeSum(double[] data, int start, int stop)
        {
            double tmp = 0;
            for (var i = start; i <= stop; i++)
            {
                tmp += data[i];
            }
            return tmp;
        }
    }
}