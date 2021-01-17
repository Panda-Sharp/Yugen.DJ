using NAudio.Wave;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Yugen.DJ.Interfaces
{
    public interface ISongService
    {
        StorageFile AudioFile { get; }
        MusicProperties MusicProperties { get; }
        float[] Buffer { get; }
        int SampleRate { get; }
        double TotalMinutes { get; }
        ISampleProvider Isp { get; }
        long Samples { get; }

        Task LoadFile();


        event EventHandler AudioDataGenerated;

        Task GenerateAudioData(StorageFile audioFile);
    }
}