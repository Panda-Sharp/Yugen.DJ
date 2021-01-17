using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Standard.Extensions;

namespace Yugen.DJ.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        public SettingsViewModel(IAudioDeviceService audioDeviceService)
        {
            _audioDeviceService = audioDeviceService;

            LoadAudioDevces();
        }

        public ObservableCollection<DeviceInformation> AudioDeviceInformationCollection { get; set; } = new ObservableCollection<DeviceInformation>();

        public DeviceInformation MasterAudioDeviceInformation
        {
            get { return _masterAudioDeviceInformation; }
            set
            {
                SetProperty(ref _masterAudioDeviceInformation, value);

                _audioDeviceService.MasterAudioDeviceInformation = _masterAudioDeviceInformation;
            }
        }

        public DeviceInformation HeadphonesAudioDeviceInformation
        {
            get { return _headphonesAudioDeviceInformation; }
            set
            {
                SetProperty(ref _headphonesAudioDeviceInformation, value);

                _audioDeviceService.HeadphonesAudioDeviceInformation = _headphonesAudioDeviceInformation;
            }
        }

        public void LoadAudioDevces()
        {
            AudioDeviceInformationCollection.AddRange(_audioDeviceService.DeviceInfoCollection);

            MasterAudioDeviceInformation = _audioDeviceService.MasterAudioDeviceInformation;
            HeadphonesAudioDeviceInformation = _audioDeviceService.HeadphonesAudioDeviceInformation;
        }
    }
}