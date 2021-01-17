using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;
using Yugen.Audio.Samples.Helpers;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class AudioGraphAudioPlayer : IAudioGraphAudioPlayer
    {
        private AudioGraph _audioGraph;
        private AudioDeviceOutputNode _deviceOutputNode;
        public AudioFileInputNode FileInputNode { get; private set; }

        private double _theta = 0;
        private AudioFrameInputNode _frameInputNode;

        private AudioGraph _secondaryAudioGraph;
        private AudioDeviceOutputNode _secondaryDeviceOutputNode;
        Stream fileStream;
        
        private AudioFrameOutputNode _frameOutputNode;

        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            await InitAudioGraph();
            await CreateDeviceOutputNode();

            await InitSecondaryAudioGraph();
            await CreateSecondaryDeviceOutputNode();

            InitFrameInputNode();
        }

        private async Task InitAudioGraph()
        {
            var audioGraphSettings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = AudioDevicesHelper.MasterAudioDeviceInformation
            };

            var result = await AudioGraph.CreateAsync(audioGraphSettings);
            if (result.Status == AudioGraphCreationStatus.Success)
            {
                _audioGraph = result.Graph;
                _audioGraph.QuantumProcessed += OnAudioGraphQuantumStarted;
            }
        }

        private async Task CreateDeviceOutputNode()
        {
            CreateAudioDeviceOutputNodeResult result = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (result.Status == AudioDeviceNodeCreationStatus.Success)
            {
                _deviceOutputNode = result.DeviceOutputNode;
            }
        }

        private async Task InitSecondaryAudioGraph()
        {
            var audioGraphSettings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = AudioDevicesHelper.HeadphonesAudioDeviceInformation
            };

            var result = await AudioGraph.CreateAsync(audioGraphSettings);
            if (result.Status == AudioGraphCreationStatus.Success)
            {
                _secondaryAudioGraph = result.Graph;
            }
        }

        private async Task CreateSecondaryDeviceOutputNode()
        {
            CreateAudioDeviceOutputNodeResult result = await _secondaryAudioGraph.CreateDeviceOutputNodeAsync();
            if (result.Status == AudioDeviceNodeCreationStatus.Success)
            {
                _secondaryDeviceOutputNode = result.DeviceOutputNode;
            }
        }

        private void InitFrameInputNode()
        {
            // Create the FrameInputNode at the same format as the graph, except explicitly set mono.
            AudioEncodingProperties nodeEncodingProperties = _secondaryAudioGraph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 1;
            _frameInputNode = _secondaryAudioGraph.CreateFrameInputNode(nodeEncodingProperties);
            _frameInputNode.AddOutgoingConnection(_secondaryDeviceOutputNode);

            // Initialize the Frame Input Node in the stopped state
            _frameInputNode.Stop();

            // Hook up an event handler so we can start generating samples when needed
            // This event is triggered when the node is required to provide data
            //_frameInputNode.QuantumStarted += FrameInputNodeOnQuantumStarted;

            // Start the graph since we will only start/stop the frame input node
            _audioGraph.Start();
            _secondaryAudioGraph.Start(); 
            
            _frameOutputNode = _audioGraph.CreateFrameOutputNode();
            _frameOutputNode.Start();
        }

        public async Task LoadFile(StorageFile tmpAudioFile)
        {
            CreateAudioFileInputNodeResult result = await _audioGraph.CreateFileInputNodeAsync(tmpAudioFile);
            if (result.Status == AudioFileNodeCreationStatus.Success)
            {
                FileInputNode = result.FileInputNode;
                FileInputNode.AddOutgoingConnection(_deviceOutputNode);
                FileInputNode.AddOutgoingConnection(_frameOutputNode);
                FileInputNode.Stop();

                var ras = await tmpAudioFile.OpenReadAsync();
                fileStream = ras.AsStreamForRead();
            }
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
            FileInputNode.Start();
            _frameInputNode.Start();
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
            //_audioGraph.Stop();
            FileInputNode.Stop();
            _frameInputNode.Stop();
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public void Record(StorageFile audioFile)
        {
        }


        private void FrameInputNodeOnQuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded != 0)
            {
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);
                _frameInputNode.AddFrame(audioData);
            }
        }

        unsafe private AudioFrame GenerateAudioData(uint samples)
        {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            uint bufferSize = samples * sizeof(float);
            AudioFrame frame = new AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInFloat = (float*)dataInBytes;

                float freq = 1000; // choosing to generate frequency of 1kHz
                float amplitude = 0.3f;
                int sampleRate = (int)_secondaryAudioGraph.EncodingProperties.SampleRate;
                double sampleIncrement = (freq * (Math.PI * 2)) / sampleRate;

                // Generate a 1kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++)
                {
                    double sinValue = amplitude * Math.Sin(_theta);
                    dataInFloat[i] = (float)sinValue;
                    _theta += sampleIncrement;
                }
            }

            return frame;
        }



        private void OnAudioGraphQuantumStarted(AudioGraph sender, object args)
        {
            //if (_frameOutputNode != null && _isPlaying)
            //{
            //    var frame = _frameOutputNode.GetFrame();
            //    if (frame.Duration > new TimeSpan())
            //    {
            //        _frameInputNode.AddFrame(frame);
            //    }
            //}
            //ProcessFrameOutput(frame);
        }

        private void ProcessFrameOutput(AudioFrame frame)
        {
            using (var buffer = frame.LockBuffer(AudioBufferAccessMode.Read))
            {
                ProcessAudioBuffer(buffer);
            }
        }

        unsafe private void ProcessAudioBuffer(AudioBuffer buffer)
        {
            using (var reference = buffer.CreateReference())
            {
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);
                dataInFloat = (float*)dataInBytes;

                if (capacityInBytes > 0)
                {
                    var floatCapacity = buffer.Length / 4;
                    float[] inputData = new float[floatCapacity];

                    for (var i = 0; i < floatCapacity; i++)
                    {
                        inputData[i] = dataInFloat[i];
                    }
                }
            }
        }
    }
}