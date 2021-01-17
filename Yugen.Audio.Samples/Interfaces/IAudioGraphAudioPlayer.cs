using Windows.Media.Audio;

namespace Yugen.Audio.Samples.Interfaces
{
    public interface IAudioGraphAudioPlayer : IAudioPlayer
    {
        AudioFileInputNode FileInputNode { get; }
    }
}