﻿using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class LeftAudioPlaybackService : AudioPlaybackService
    {
        public LeftAudioPlaybackService(IAudioDeviceService audioDeviceService, IAudioGraphService masterAudioGraphService,
            IAudioGraphService headphonesAudioGraphService) : base(audioDeviceService, masterAudioGraphService, headphonesAudioGraphService)
        {
        }
    }
}