using AudioVisualizer;
using Windows.Media.Audio;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioVisualizerService
    {
        PlaybackSource GetPlaybackSource(AudioFileInputNode masterFileInput);
    }
}