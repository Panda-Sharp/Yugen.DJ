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

        private Stream _fileStream;

        public WaveformViewModel(IWaveformRendererService waveformRendererService)
        {
            _waveformRendererService = waveformRendererService;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public event EventHandler WaveformGenerated;

        public ICommand OpenCommand { get; }

        public void WaveformRendererServiceDrawLine(CanvasControl sender, CanvasDrawingSession drawingSession)
        {
            _waveformRendererService.DrawLine(sender, drawingSession);
        }

        public async Task GenerateAudioData()
        {
            ISampleProvider isp;
            long samples;

            await Task.Run(() =>
            {
                using (var reader = new StreamMediaFoundationReader(_fileStream))
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

        private async Task GetFileStream()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                     new List<string> { ".mp3" },
                     PickerLocationId.MusicLibrary
                 );

            if (audioFile != null)
            {
                var ras = await audioFile.OpenReadAsync();
                _fileStream = ras.AsStreamForRead();
            }
        }

        private async Task OpenCommandBehavior()
        {
            await GetFileStream();

            await GenerateAudioData();
        }
    }
}