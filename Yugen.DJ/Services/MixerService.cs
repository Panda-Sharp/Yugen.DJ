using Yugen.DJ.Interfaces;
using Yugen.DJ.Models;

namespace Yugen.DJ.Services
{
    public class MixerService : IMixerService
    {
        private readonly IAppService _appService;

        //private double _fader = 0.5;
        private double _leftFader = 0.5;

        private double _rightFader = 0.5;
        private double _leftVolume = 100;
        private double _rightVolume = 100;

        public MixerService(IAppService appService)
        {
            _appService = appService;
        }

        public void IsHeadphones(bool isHeadPhones, Side side) =>
            _appService.AudioPlaybackService(side)?.IsHeadphones(isHeadPhones);

        public void ChangeVolume(double volume, Side side)
        {
            if (side == Side.Left)
            {
                _leftVolume = volume;
            }
            else
            {
                _rightVolume = volume;
            }

            UpdateVolume();
        }

        public void SetFader(double crossFader)
        {
            var absoluteValue = 20 - (crossFader + 10);
            var percentace = 100 * absoluteValue / 20;
            _leftFader = percentace / 100;

            absoluteValue = crossFader + 10;
            percentace = 100 * absoluteValue / 20;
            _rightFader = percentace / 100;

            UpdateVolume();
        }

        private void UpdateVolume()
        {
            _appService.AudioPlaybackService(Side.Left)?.ChangeVolume(_leftVolume, _leftFader);
            _appService.AudioPlaybackService(Side.Right)?.ChangeVolume(_rightVolume, _rightFader);
        }
    }
}