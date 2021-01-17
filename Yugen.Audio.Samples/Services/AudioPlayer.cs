using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class AudioPlayer : IAudioPlayer
    {
        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
        }

        public async Task LoadFile(StorageFile tmpAudioFile)
        {
        }

        public void LoadStream(Stream audioStream)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
        }

        public void PlayWithoutStreaming()
        {
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public void Record(StorageFile audioFile)
        {
            throw new NotImplementedException();
        }
    }
}