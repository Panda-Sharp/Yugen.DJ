using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class AudioGraphService : IAudioGraphService
    {
        private AudioGraph audioGraph;
        private AudioDeviceOutputNode deviceOutput;

        public event EventHandler<TimeSpan> PositionChanged;

        public AudioFileInputNode AudioFileInput { get; private set; }

        public async Task InitDevice(DeviceInformation audioDeviceInformation, bool isMaster)
        {
            if (audioDeviceInformation == null)
                return;

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = audioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            audioGraph = result.Graph;
            if (isMaster)
            {
                audioGraph.QuantumProcessed += OnQuantumProcessed;
            }

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await audioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            deviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        public async Task AddFileToDevice(StorageFile audioFile)
        {
            if (audioGraph == null)
                return;

            CreateAudioFileInputNodeResult fileInputResult = await audioGraph.CreateFileInputNodeAsync(audioFile);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
                return;

            AudioFileInput = fileInputResult.FileInputNode;
            AudioFileInput.AddOutgoingConnection(deviceOutput);
        }

        public void TogglePlay(bool isPaused)
        {
            if (isPaused)
            {
                audioGraph?.Stop();
            }
            else
            {
                audioGraph?.Start();
            }
        }

        public void ChangePitch(double ratio)
        {
            if (AudioFileInput != null)
            {
                AudioFileInput.PlaybackSpeedFactor = 1 + ratio;
            }
        }

        public void ChangeVolume(double volume)
        {
            if (deviceOutput != null)
            {
                deviceOutput.OutgoingGain = volume;
            }
        }

        public void IsHeadphones(bool isHeadphone)
        {
            if (deviceOutput != null)
            {
                deviceOutput.OutgoingGain = isHeadphone ? 1 : 0;
            }
        }

        /// <summary>
        /// If another file is already loaded into the FileInput node
        /// Stop playback since a new file is being loaded.
        /// Release the file and dispose the contents of the node
        /// </summary>
        /// <returns></returns>
        public void DisposeFileInputs()
        {
            audioGraph?.Stop();
            AudioFileInput?.Dispose();
        }

        private void OnQuantumProcessed(AudioGraph sender, object args) =>
                                                            PositionChanged?.Invoke(sender, AudioFileInput?.Position ?? new TimeSpan());
    }
}