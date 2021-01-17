using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NAudio.Wave;
using System;
using Yugen.Toolkit.Uwp.Audio.Helpers;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;
using Yugen.Toolkit.Uwp.Audio.Waveform.Providers;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    /// <summary>
    /// https://github.com/naudio/NAudio.WaveformRenderer
    /// </summary>
    public class WaveformRendererService : IWaveformRendererService
    {
        private readonly WaveformRendererSettings _settings = new WaveformRendererSettings();

        private bool _isFinished;
        private IPeakProvider _peakProvider = new MaxPeakProvider();

        public WaveformRendererService()
        {
        }

        public WaveformRendererService(WaveformRendererSettings settings, IPeakProvider peakProvider)
        {
            _settings = settings;
            _peakProvider = peakProvider;
        }

        //public async Task Render(IStorageFile file)
        //{
        //    var stream = await file.OpenStreamForReadAsync();
        //    Render(stream);
        //}

        //public void Render(Stream stream)
        //{
        //    ISampleProvider isp;
        //    var samples = 0L;

        //    using (var reader = new StreamMediaFoundationReader(stream))
        //    {
        //        isp = reader.ToSampleProvider();
        //        var buffer = new float[reader.Length / 2];
        //        isp.Read(buffer, 0, buffer.Length);

        //        var bytesPerSample = reader.Waveformat.BitsPerSample / 8;
        //        samples = reader.Length / bytesPerSample;
        //    }

        //    Render(isp, samples);
        //}

        public void Render(ISampleProvider isp, long samples)
        {
            var samplesPerPixel = (int)(samples / _settings.Width);
            var stepSize = _settings.PixelsPerPeak + _settings.SpacerPixels;
            _peakProvider.Init(isp, samplesPerPixel * stepSize);
            _isFinished = true;
        }

        public void DrawLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            if (_isFinished)
            {
                DrawRealLine(sender, ds);
            }
            else
            {
                DrawFakeLine(sender, ds);
            }
        }

        public void DrawRealLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            // DecibelScale - if true, convert values to decibels for a logarithmic waveform
            if (_settings.DecibelScale)
                _peakProvider = new DecibelPeakProvider(_peakProvider, 48);

            var midPoint = _settings.TopHeight;

            var x = 0;
            var currentPeak = _peakProvider.GetNextPeak();

            var strokeWidth = 1;

            while (x < _settings.Width)
            {
                var mu = (float)x / _settings.Width;
                var color = ColorHelper.GradientColor(mu);

                var nextPeak = _peakProvider.GetNextPeak();

                for (var n = 0; n < _settings.PixelsPerPeak; n++)
                {
                    var lineHeight = _settings.TopHeight * currentPeak.Max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = _settings.BottomHeight * currentPeak.Min;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, 10 - strokeWidth);

                    x++;
                }

                for (var n = 0; n < _settings.SpacerPixels; n++)
                {
                    // spacer bars are always the lower of the
                    var max = Math.Min(currentPeak.Max, nextPeak.Max);
                    var min = Math.Max(currentPeak.Min, nextPeak.Min);

                    var lineHeight = _settings.TopHeight * max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = _settings.BottomHeight * min;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, 10 - strokeWidth);

                    x++;
                }

                currentPeak = nextPeak;
            }
        }

        public void DrawFakeLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            var width = (float)sender.ActualWidth;
            var height = (float)sender.ActualHeight;

            //var middle = height / 2;

            var steps = _isFinished ? 100 : 50; // Math.Min((int)(width / 10), 30);

            for (var i = 0; i < steps; ++i)
            {
                var mu = (float)i / steps;
                var a = (float)(mu * Math.PI * 2);

                var color = ColorHelper.GradientColor(mu);

                var x = width * mu;
                var rnd = new Random();
                var y = rnd.Next(1, 100); //(float)(middle + Math.Sin(a) * (middle * 0.3));

                var strokeWidth = 1; // (float)(Math.Cos(a) + 1) * 5;

                ds.DrawLine(x, 0, x, y, color, strokeWidth);
                ds.DrawLine(x, height, x, y, color, 10 - strokeWidth);
            }
        }
    }
}