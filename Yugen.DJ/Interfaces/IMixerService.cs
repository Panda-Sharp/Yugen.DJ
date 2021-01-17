using Yugen.DJ.Models;

namespace Yugen.DJ.Interfaces
{
    public interface IMixerService
    {
        void IsHeadphones(bool isHeadPhones, Side side);
        void ChangeVolume(double volume, Side side);
        void SetFader(double crossFader);
    }
}