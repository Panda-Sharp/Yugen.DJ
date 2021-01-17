using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;

namespace Yugen.Audio.Samples.Helpers
{
    public static class AudioDevicesHelper
    {
        public static DeviceInformation MasterAudioDeviceInformation;
        public static DeviceInformation HeadphonesAudioDeviceInformation;

        public static async Task Initialize()
        {
            var defaultAudioDeviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            var DeviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            MasterAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => x.Id.Equals(defaultAudioDeviceId));

            HeadphonesAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => !x.Id.Equals(defaultAudioDeviceId));
        }
    }
}