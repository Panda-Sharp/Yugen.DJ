﻿using NAudio.Wave;
using System;
using System.Linq;
using Yugen.DJ.Audio.WaveForm.Models;

namespace Yugen.DJ.Audio.WaveForm.Providers
{
    public class AveragePeakProvider : PeakProvider
    {
        private readonly float scale;

        public AveragePeakProvider(float scale)
        {
            this.scale = scale;
        }

        public override PeakInfo GetNextPeak()
        {
            var samplesRead = Provider.Read(ReadBuffer, 0, ReadBuffer.Length);
            var sum = samplesRead == 0 ? 0 : ReadBuffer.Take(samplesRead).Select(s => Math.Abs(s)).Sum();
            var average = sum / samplesRead;

            return new PeakInfo(average * (0 - scale), average * scale);
        }
    }
}