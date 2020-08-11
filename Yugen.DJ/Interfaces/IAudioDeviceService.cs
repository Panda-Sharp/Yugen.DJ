using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioDeviceService
    {
        DeviceInformationCollection DeviceInfoCollection { get; }
        DeviceInformation MasterAudioDeviceInformation { get; set; }
        DeviceInformation HeadphonesAudioDeviceInformation { get; set; }

        double GetMasterVolume();

        void SetVolume(double volume);

        Task Init();
    }
}