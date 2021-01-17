using Yugen.DJ.Models;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioPlaybackFactory
    {
        IAudioPlaybackService Create(Side type);
    }
}