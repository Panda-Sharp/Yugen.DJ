using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.DJ.Audio.SystemVolume;
using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class AudioDeviceService : IAudioDeviceService
    {
        public DeviceInformationCollection DeviceInfoCollection { get; private set; }
        public DeviceInformation MasterAudioDeviceInformation { get; set; }
        public DeviceInformation HeadphonesAudioDeviceInformation { get; set; }

        public double GetMasterVolume() => VolumeControl.GetVolume();

        public void SetVolume(double volume) => VolumeControl.SetVolume(volume / 100);

        public async Task Init()
        {
            DeviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            MasterAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault();
            HeadphonesAudioDeviceInformation = DeviceInfoCollection.ElementAtOrDefault(1);
        }
    }
}