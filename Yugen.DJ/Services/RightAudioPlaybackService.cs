using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class RightAudioPlaybackService : AudioPlaybackService
    {
        public RightAudioPlaybackService(IAudioDeviceService audioDeviceService, IAudioGraphService masterAudioGraphService,
            IAudioGraphService headphonesAudioGraphService) : base(audioDeviceService, masterAudioGraphService, headphonesAudioGraphService)
        {
        }
    }
}