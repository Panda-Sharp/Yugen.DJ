using AudioVisualizer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private StorageFile file;

        private AudioGraph masterAudioGraph;
        private AudioDeviceOutputNode masterDeviceOutput;
        private AudioFileInputNode masterFileInput;

        private AudioGraph headphonesAudioGraph;
        private AudioDeviceOutputNode headphonesDeviceOutput;
        private AudioFileInputNode headphonesFileInput;

        private PlaybackSource _source;
        private DiscreteVUBar VUBarChannel0;
        private DiscreteVUBar VUBarChannel1;

        public AudioService()
        {
            _audioDeviceService = Ioc.Default.GetService<IAudioDeviceService>();
        }

        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler<StorageFile> FileLoaded;

        public TimeSpan NaturalDuration => masterFileInput?.Duration ?? new TimeSpan();

        public async Task Init()
        {
            await InitMasterDevice();
            await InitHeadphonesDevice();
        }

        public async Task OpenFile()
        {
            file = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    Windows.Storage.Pickers.PickerLocationId.MusicLibrary
                );

            if (file == null)
                return;

            FileLoaded?.Invoke(null, file);

            DisposeFileInputs();

            await AddFileToMasterDevice();
            await AddFileToHeadsetDevice();

            AddFileInputToAudioVisualizer();
        }

        public void TogglePlay(bool isPaused)
        {
            if (isPaused)
            {
                masterAudioGraph?.Stop();
                headphonesAudioGraph?.Stop();
            }
            else
            {
                masterAudioGraph?.Start();
                headphonesAudioGraph?.Start();
            }
        }

        public void ChangePitch(double pitch)
        {
            var ratio = pitch == 0 ? 0 : pitch / 100;

            if (masterFileInput != null)
            {
                masterFileInput.PlaybackSpeedFactor = 1 + ratio;
            }

            if (headphonesFileInput != null)
            {
                headphonesFileInput.PlaybackSpeedFactor = 1 + ratio;
            }
        }

        public void ChangeVolume(double volume, double fader)
        {
            volume *= fader / 100;

            masterDeviceOutput.OutgoingGain = volume;

            //if (headphonesDeviceOutput != null)
            //{
            //    headphonesDeviceOutput.OutgoingGain = volume;
            //}
        }

        public void IsHeadphones(bool isHeadphone)
        {
            if (headphonesDeviceOutput != null)
            {
                headphonesDeviceOutput.OutgoingGain = isHeadphone ? 1 : 0;
            }
        }

        public void AddAudioVisualizer(DiscreteVUBar VUBarChannel0, DiscreteVUBar VUBarChannel1)
        {
            this.VUBarChannel0 = VUBarChannel0;
            this.VUBarChannel1 = VUBarChannel1;
        }

        private async Task InitMasterDevice()
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.MasterAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            masterAudioGraph = result.Graph;
            masterAudioGraph.QuantumProcessed += MasterAudioGraphOnQuantumProcessed; ;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await masterAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            masterDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private async Task InitHeadphonesDevice()
        {
            if (_audioDeviceService.HeadphonesAudioDeviceInformation == null)
                return;

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.HeadphonesAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            headphonesAudioGraph = result.Graph;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await headphonesAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            headphonesDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private void MasterAudioGraphOnQuantumProcessed(AudioGraph sender, object args)
        {
            //var data = _source?.Source?.GetData();
            PositionChanged?.Invoke(sender, masterFileInput?.Position ?? new TimeSpan());
        }

        /// <summary>
        /// If another file is already loaded into the FileInput node
        /// Stop playback since a new file is being loaded.
        /// Release the file and dispose the contents of the node
        /// </summary>
        /// <returns></returns>
        private void DisposeFileInputs()
        {
            masterAudioGraph?.Stop();
            headphonesAudioGraph?.Stop();

            masterFileInput?.Dispose();
            headphonesFileInput?.Dispose();
        }

        private async Task AddFileToMasterDevice()
        {
            CreateAudioFileInputNodeResult masterFileInputResult = await masterAudioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != masterFileInputResult.Status)
                return;

            masterFileInput = masterFileInputResult.FileInputNode;
            masterFileInput.AddOutgoingConnection(masterDeviceOutput);
        }

        private async Task AddFileToHeadsetDevice()
        {
            if (headphonesAudioGraph == null)
                return;

            CreateAudioFileInputNodeResult headphonesFileInputResult = await headphonesAudioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != headphonesFileInputResult.Status)
                return;

            headphonesFileInput = headphonesFileInputResult.FileInputNode;
            headphonesFileInput.AddOutgoingConnection(headphonesDeviceOutput);
        }

        private void AddFileInputToAudioVisualizer()
        {
            if (masterFileInput == null)
                return;

            _source = PlaybackSource.CreateFromAudioNode(masterFileInput);

            VUBarChannel0.Source = _source.Source;
            VUBarChannel1.Source = _source.Source;

            // converter
            //SourceConverter sourceConverter = new SourceConverter
            //{
            //    Source = _source.Source,

            //    AnalyzerTypes = AnalyzerType.RMS | AnalyzerType.Peak,
            //    RmsRiseTime = TimeSpan.FromMilliseconds(50),
            //    RmsFallTime = TimeSpan.FromMilliseconds(50),
            //    PeakRiseTime = TimeSpan.FromMilliseconds(500),
            //    PeakFallTime = TimeSpan.FromMilliseconds(500),

            //    //AnalyzerTypes = AnalyzerType.Spectrum,
            //    //SpectrumRiseTime = TimeSpan.FromMilliseconds(500),
            //    //SpectrumFallTime = TimeSpan.FromMilliseconds(500),

            //    //FrequencyCount =  12 * 5 * 5; // 5 octaves, 5 bars per note
            //    //MinFrequency = 110.0f;    // Note A2
            //    //MaxFrequency = 3520.0f;  // Note A7
            //    //FrequencyScale = ScaleType.Logarithmic,

            //    //CacheData = true,
            //    //ChannelCount = 2,
            //    //ChannelMapping = new float[] { 0 },
            //    //Fps = 60f,
            //    //IsSuspended = true,
            //    //PlaybackState =SourcePlaybackState.Playing
            //};
            //var data = sourceConverter.GetData();

            //VUBarChannel0.Source = sourceConverter;
            //VUBarChannel1.Source = sourceConverter;

            // ElementFactory
            //VUBarChannel0.ElementFactory
        }
    }
}