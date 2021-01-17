using CSCore;
using CSCore.Codecs;
using CSCore.XAudio2;
using System;
using System.IO;
using System.Threading.Tasks;
using WASAPI.NET;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class CsCoreAudioPlayer : IAudioPlayer
    {
        private IWaveSource _waveSource;
        private XAudio2 _xaudio2;
        private XAudio2MasteringVoice _masteringVoice;
        private StreamingSourceVoice _streamingSourceVoice;

        //private WasapiCapture _soundIn;
        //private PitchShifter _pitchShifter;

        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            _xaudio2 = XAudio2.CreateXAudio2();
            _masteringVoice = _xaudio2.CreateMasteringVoice(inputChannels, inputSampleRate, deviceId);
        }

        public async Task LoadFile(StorageFile tmpAudioFile)
        {
            _waveSource = CodecFactory.Instance.GetCodec(tmpAudioFile.Path);

            //var a = new WasapiLoopbackDriver();
            //a.Setup((audioDriver, b) => { });
            //a.Start();
        }

        //private void SetupLoopbackCapture()
        //{
        //    //open the default device 
        //    _soundIn = new WasapiLoopbackCapture();
        //    //Our loopback capture opens the default render device by default so the following is not needed
        //    //_soundIn.Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
        //    _soundIn.Initialize();

        //    var soundInSource = new SoundInSource(_soundIn);
        //    ISampleSource source = soundInSource.ToSampleSource().AppendSource(x => new PitchShifter(x), out _pitchShifter);

        //    //SetupSampleSource(source);

        //    // We need to read from our source otherwise SingleBlockRead is never called and our spectrum provider is not populated
        //    byte[] buffer = new byte[waveSource.WaveFormat.BytesPerSecond / 2];
        //    soundInSource.DataAvailable += (s, aEvent) =>
        //        {
        //            int read;
        //            while ((read = waveSource.Read(buffer, 0, buffer.Length)) > 0) ;
        //        };


        //    //play the audio
        //    _soundIn.Start();
        //}

        //private void SetupSampleSource(ISampleSource aSampleSource)
        //{
        //    const FftSize fftSize = FftSize.Fft4096;
        //    //create a spectrum provider which provides fft data based on some input
        //    var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels,
        //        aSampleSource.WaveFormat.SampleRate, fftSize);

        //    //linespectrum and voiceprint3dspectrum used for rendering some fft data
        //    //in oder to get some fft data, set the previously created spectrumprovider 
        //    _lineSpectrum = new LineSpectrum(fftSize)
        //    {
        //        SpectrumProvider = spectrumProvider,
        //        UseAverage = true,
        //        BarCount = 50,
        //        BarSpacing = 2,
        //        IsXLogScale = true,
        //        ScalingStrategy = ScalingStrategy.Sqrt
        //    };
        //    _voicePrint3DSpectrum = new VoicePrint3DSpectrum(fftSize)
        //    {
        //        SpectrumProvider = spectrumProvider,
        //        UseAverage = true,
        //        PointCount = 200,
        //        IsXLogScale = true,
        //        ScalingStrategy = ScalingStrategy.Sqrt
        //    };

        //    //the SingleBlockNotificationStream is used to intercept the played samples
        //    var notificationSource = new SingleBlockNotificationStream(aSampleSource);
        //    //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
        //    notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

        //    _source = notificationSource.ToWaveSource(16);
        //}

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
            _streamingSourceVoice = new StreamingSourceVoice(_xaudio2, _waveSource);

            StreamingSourceVoiceListener.Default.Add(_streamingSourceVoice);
            //add the streamingSourceVoice to the default sourcevoicelistener which processes the data requests.
            _streamingSourceVoice.Start();
        }

        public void PlayWithoutStreaming()
        {
            //using (var sourceVoice = xaudio2.CreateSourceVoice(waveSource.WaveFormat))
            //{
            //    var buffer = waveSource.ToByteArray();
            //    using (var sourceBuffer = new XAudio2Buffer(buffer.Length))
            //    {
            //        using (var stream = sourceBuffer.GetStream())
            //        {
            //            stream.Write(buffer, 0, buffer.Length);
            //        }

            //        sourceVoice.SubmitSourceBuffer(sourceBuffer);
            //    }

            //    sourceVoice.Start();
            //}
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            StreamingSourceVoiceListener.Default.Remove(_streamingSourceVoice);
            _streamingSourceVoice.Stop();
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