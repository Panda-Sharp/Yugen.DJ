using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Services
{
    public class SongService : ISongService
    {
        public StorageFile AudioFile { get; private set; }

        public MusicProperties MusicProperties { get; private set; }

        public async Task LoadFile()
        {
            AudioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (AudioFile != null)
            {
                MusicProperties = await AudioFile.Properties.GetMusicPropertiesAsync();
            }
        }


        public event EventHandler AudioDataGenerated;

        public ISampleProvider Isp { get; private set; }

        public long Samples { get; private set; } = 0L;

        public float[] Buffer { get; private set; } = new float[0];

        public int SampleRate { get; private set; } = 0;

        public double TotalMinutes { get; private set; } = 0d;

        public double BPM { get; private set; } = 0;

        public async Task GenerateAudioData(StorageFile audioFile)
        {
            var stream = await audioFile.OpenStreamForReadAsync();

            await Task.Run(() =>
            {
                using (var reader = new StreamMediaFoundationReader(stream))
                {
                    Isp = reader.ToSampleProvider();
                    Buffer = new float[reader.Length / 2];
                    Isp.Read(Buffer, 0, Buffer.Length);

                    var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                    Samples = reader.Length / bytesPerSample;

                    SampleRate = Isp.WaveFormat.SampleRate;
                    TotalMinutes = reader.TotalTime.TotalMinutes;
                }
            });

            AudioDataGenerated?.Invoke(this, EventArgs.Empty);
        }

        //private async Task Analyze(StorageFile file)
        //{
        //    // Create analyzer for 10ms frames, 25% overlap
        //    var analyzer = new AudioAnalyzer(100000, 2, 48000, 480, 120, 1024, false);

        //    var stream = await file.OpenAsync(FileAccessMode.Read);
        //    var reader = new AudioSourceReader(stream);

        //    // Set output format to same as analyzer, 48k, 2 channels. Analyzer needs 32bit float samples
        //    var format = AudioEncodingProperties.CreatePcm(48000, 2, 32);
        //    format.Subtype = "Float";
        //    reader.Format = format;

        //    AudioFrame frame = null;

        //    do
        //    {
        //        frame = reader.Read();
        //        if (frame == null)
        //        {
        //            break;
        //        }
        //        analyzer.ProcessInput(frame);

        //    } while (frame != null);
        //}
    }
}