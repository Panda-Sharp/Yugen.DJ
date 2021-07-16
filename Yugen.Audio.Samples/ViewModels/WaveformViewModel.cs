using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Mvvm.Input;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class WaveformViewModel : ViewModelBase
    {
        private readonly IWaveformRendererService _waveformRendererService;

        public WaveformViewModel(IWaveformRendererService waveformRendererService)
        {
            _waveformRendererService = waveformRendererService;
        }

        public event EventHandler WaveformGenerated;

        public void WaveformRendererServiceDrawLine(CanvasControl sender, CanvasDrawingSession drawingSession)
        {
            _waveformRendererService.DrawLine(sender, drawingSession);
        }

        public async Task GenerateAudioData(Stream stream)
        {
            ISampleProvider isp;
            long samples;

            await Task.Run(() =>
            {
                using (var reader = new StreamMediaFoundationReader(stream))
                {
                    isp = reader.ToSampleProvider();
                    var Buffer = new float[reader.Length / 2];
                    isp.Read(Buffer, 0, Buffer.Length);

                    var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                    samples = reader.Length / bytesPerSample;

                    var sampleRate = isp.WaveFormat.SampleRate;
                    var totalMinutes = reader.TotalTime.TotalMinutes;
                }

                _waveformRendererService.Render(isp, samples);
                WaveformGenerated?.Invoke(this, EventArgs.Empty);

            });
        }
    }
}