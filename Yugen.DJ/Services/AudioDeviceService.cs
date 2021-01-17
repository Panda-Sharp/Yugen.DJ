using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Yugen.DJ.Audio.SystemVolume;
using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class AudioDeviceService : IAudioDeviceService
    {
        public DeviceInformationCollection DeviceInfoCollection { get; private set; }
        public DeviceInformation MasterAudioDeviceInformation { get; set; }
        public DeviceInformation HeadphonesAudioDeviceInformation { get; set; }

        public double GetMasterVolume() => SystemVolumeHelper.GetVolume();

        public void SetVolume(double volume) => SystemVolumeHelper.SetVolume(volume / 100);

        public async Task Init()
        {
            var defaultAudioDeviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            DeviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            MasterAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => x.Id.Equals(defaultAudioDeviceId));

            HeadphonesAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => !x.Id.Equals(defaultAudioDeviceId));            
        }
    }
}