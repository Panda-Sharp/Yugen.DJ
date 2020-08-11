using AudioVisualizer;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioService
    {
        event EventHandler<TimeSpan> PositionChanged;

        event EventHandler<StorageFile> FileLoaded;

        TimeSpan NaturalDuration { get; }

        Task Init();

        Task OpenFile();

        void TogglePlay(bool isPaused);

        void ChangePitch(double pitch);

        void ChangeVolume(double volume, double fader);

        void IsHeadphones(bool isHeadphone);

        void AddAudioVisualizer(DiscreteVUBar leftVUBarChanel0, DiscreteVUBar leftVUBarChanel1);
    }
}