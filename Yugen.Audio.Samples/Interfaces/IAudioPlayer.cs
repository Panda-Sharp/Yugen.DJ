using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Interfaces
{
    public interface IAudioPlayer
    {
        TimeSpan Duration { get; }
        bool IsRepeating { get; set; }
        TimeSpan Position { get; set; }
        AudioPlayerState State { get; }
        float Volume { get; set; }

        void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100);
        Task LoadFile(StorageFile tmpAudioFile);
        void LoadStream(Stream audioStream);
        void Close();

        void Play();
        void PlayWithoutStreaming();
        void Pause();
        void Stop();
        void Wait();
        void Record(StorageFile audioFile);
    }
}