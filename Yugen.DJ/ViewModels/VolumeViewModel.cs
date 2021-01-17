using Microsoft.Toolkit.Mvvm.ComponentModel;
using Yugen.DJ.Interfaces;
using Yugen.DJ.Models;

namespace Yugen.DJ.ViewModels
{
    public class VolumeViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;

        private double _volume = 100;
        private bool _isHeadPhones;
        private Side _side;

        public VolumeViewModel(IMixerService mixerService)
        {
            _mixerService = mixerService;
        }

        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                if (_side == Side.Left)
                {
                    IsHeadPhones = true;
                }
            }
        }

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                SetProperty(ref _isHeadPhones, value);

                _mixerService?.IsHeadphones(_isHeadPhones, _side);
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                SetProperty(ref _volume, value);

                _mixerService.ChangeVolume(_volume, _side);
            }
        }
    }
}