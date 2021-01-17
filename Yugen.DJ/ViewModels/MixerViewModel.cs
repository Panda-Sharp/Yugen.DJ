using Microsoft.Toolkit.Mvvm.ComponentModel;
using Yugen.DJ.Interfaces;

namespace Yugen.DJ.ViewModels
{
    public class MixerViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;
        private readonly IAudioDeviceService _audioDeviceService;

        private double _crossFader = 0;
        private double _masterVolume = 0;

        public MixerViewModel(IMixerService mixerService, IAudioDeviceService audioDeviceService)
        {
            _mixerService = mixerService;
            _audioDeviceService = audioDeviceService;

            _masterVolume = _audioDeviceService?.GetMasterVolume() * 100 ?? MasterVolume;
        }

        public double MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                SetProperty(ref _masterVolume, value);

                _audioDeviceService?.SetVolume(_masterVolume);
            }
        }

        public double CrossFader
        {
            get { return _crossFader; }
            set
            {
                SetProperty(ref _crossFader, value);

                _mixerService.SetFader(_crossFader);
            }
        }
    }
}