using Yugen.DJ.Models;

namespace Yugen.DJ.Interfaces
{
    public interface IAppService
    {
        IAudioPlaybackService AudioPlaybackService(Side side);
    }
}