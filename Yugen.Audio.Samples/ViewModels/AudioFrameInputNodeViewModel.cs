using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Storage.Pickers;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class AudioFrameInputNodeViewModel : ViewModelBase
    {
        private AudioGraph _audioGraph;
        private AudioFrameInputNode _audioFrameInputNode;
        private AudioDeviceOutputNode _deviceOutputNode;
        private Stream _fileStream;

        public AudioFrameInputNodeViewModel()
        {
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public ICommand OpenCommand { get; }

        public async Task Init()
        {
            AudioGraphSettings audioGraphSettings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            var result = await AudioGraph.CreateAsync(audioGraphSettings);
            if (result == null || result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }
            _audioGraph = result.Graph;

            var createAudioDeviceOutputResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (createAudioDeviceOutputResult == null || createAudioDeviceOutputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }
            _deviceOutputNode = createAudioDeviceOutputResult.DeviceOutputNode;

            AudioEncodingProperties audioEncodingProperties = new AudioEncodingProperties
            {
                BitsPerSample = 32,
                ChannelCount = 2,
                SampleRate = 44100,
                Subtype = MediaEncodingSubtypes.Float
            };

            _audioFrameInputNode = _audioGraph.CreateFrameInputNode(audioEncodingProperties);
            _audioFrameInputNode.QuantumStarted += OnFrameInputNodeQuantumStarted;

            _audioFrameInputNode.AddOutgoingConnection(_deviceOutputNode);
            _audioGraph.Start();
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

        private unsafe void OnFrameInputNodeQuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            var bufferSize = args.RequiredSamples * sizeof(float) * 2;
            AudioFrame audioFrame = new AudioFrame((uint)bufferSize);

            if (_fileStream == null)
                return;

            using (var audioBuffer = audioFrame.LockBuffer(AudioBufferAccessMode.Write))
            {
                using (var bufferReference = audioBuffer.CreateReference())
                {
                    float* dataInFloat;

                    // Get the buffer from the AudioFrame
                    ((IMemoryBufferByteAccess)bufferReference).GetBuffer(out byte* dataInBytes, out uint capacityInBytes);
                    dataInFloat = (float*)dataInBytes;

                    var managedBuffer = new byte[capacityInBytes];

                    var lastLength = _fileStream.Length - _fileStream.Position;
                    int readLength = (int)(lastLength < capacityInBytes ? lastLength : capacityInBytes);
                    if (readLength <= 0)
                    {
                        _fileStream.Close();
                        _fileStream = null;
                        return;
                    }
                    _fileStream.Read(managedBuffer, 0, readLength);

                    for (int i = 0; i < readLength; i += 8)
                    {
                        dataInBytes[i + 4] = managedBuffer[i + 0];
                        dataInBytes[i + 5] = managedBuffer[i + 1];
                        dataInBytes[i + 6] = managedBuffer[i + 2];
                        dataInBytes[i + 7] = managedBuffer[i + 3];
                        dataInBytes[i + 0] = managedBuffer[i + 4];
                        dataInBytes[i + 1] = managedBuffer[i + 5];
                        dataInBytes[i + 2] = managedBuffer[i + 6];
                        dataInBytes[i + 3] = managedBuffer[i + 7];
                    }
                }
            }

            _audioFrameInputNode.AddFrame(audioFrame);
        }

        private async Task OpenCommandBehavior()
        {
            await GetFileStream();
            await Init();
        }
    }
}