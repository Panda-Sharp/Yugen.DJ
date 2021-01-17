using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Yugen.Audio.Samples.Helpers;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Services;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class CsCoreViewModel : ViewModelBase
    {
        private readonly IAudioPlayer _audioPlayer = new CsCoreAudioPlayer();

        public CsCoreViewModel()
        {
            OnLoadCommand = new RelayCommand(OnLoadCommandBehavior);
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            PlayCommand = new RelayCommand(PlayCommandBehavior);
            //PlayWithoutStreamingCommand = new RelayCommand(PlayWithoutStreamingCommandBehavior);
            StopCommand = new RelayCommand(StopCommandBehavior);
            //RecordCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public ICommand OnLoadCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand PlayCommand { get; }
        //public ICommand PlayWithoutStreamingCommand { get; }
        public ICommand StopCommand { get; }
        //public ICommand RecordCommand { get; }

        public void OnLoadCommandBehavior()
        {
            _audioPlayer.Initialize(AudioDevicesHelper.MasterAudioDeviceInformation.Id);
            //audioPlayer.InitializeXAudio2(AudioDevicesHelper.HeadphonesAudioDeviceInformation.Id);
        }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (audioFile != null)
            {
                var tmpAudioFile = await audioFile.CopyAsync(ApplicationData.Current.TemporaryFolder, audioFile.Name, NameCollisionOption.ReplaceExisting);

                _audioPlayer.LoadFile(tmpAudioFile);
            }
        }

        private void PlayCommandBehavior() => _audioPlayer.Play();

        //private void PlayWithoutStreamingCommandBehavior() => _audioPlayer.PlayWithoutStreaming();

        private void StopCommandBehavior() => _audioPlayer.Stop();

        //private async void OnRecordButton()
        //{
        //    //var audioFile = await FilePickerHelper.SaveFile(
        //    //        "audio",
        //    //        ".mp3",
        //    //        ".mp3"
        //    //    );

        //    audioPlayer.Record(null);
        //}
    }
}