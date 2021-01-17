using System;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioPlaybackService
    {
        event EventHandler<TimeSpan> PositionChanged;

        TimeSpan NaturalDuration { get; }
        AudioFileInputNode MasterFileInput { get; }

        Task Init();

        Task LoadSong(StorageFile audioFile);

        void TogglePlay(bool isPaused);

        void ChangePitch(double pitch);

        void ChangeVolume(double volume, double fader);

        void IsHeadphones(bool isHeadphone);
    }
}