using System;
using System.IO;
using System.Threading.Tasks;
using Vortice.Multimedia;
using Vortice.XAudio2;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class VortexAudioPlayer : IAudioPlayer
    {
        private readonly IXAudio2 _xaudio2;
        private IXAudio2MasteringVoice _masteringVoice;
        //private AudioDecoder _audioDecoder;
        //private readonly IXAudio2SourceVoice _sourceVoice;

        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public VortexAudioPlayer()
        {
            _xaudio2 = new IXAudio2();
            _xaudio2.StartEngine();
        }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            _masteringVoice = _xaudio2.CreateMasteringVoice(inputChannels, inputSampleRate);
        }

        public async Task LoadFile(StorageFile tmpAudioFile)
        {
            WaveFormat waveFormat = new WaveFormat();
            _xaudio2.CreateSourceVoice(waveFormat);
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