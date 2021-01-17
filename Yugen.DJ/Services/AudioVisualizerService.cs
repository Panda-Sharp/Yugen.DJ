using AudioVisualizer;
using Windows.Media.Audio;
using Yugen.DJ.Interfaces;

namespace Yugen.DJ.Services
{
    public class AudioVisualizerService : IAudioVisualizerService
    {
        public PlaybackSource GetPlaybackSource(AudioFileInputNode masterFileInput)
        {
            if (masterFileInput == null)
                return null;

            return PlaybackSource.CreateFromAudioNode(masterFileInput);

            //data
            //var _source = PlaybackSource.CreateFromAudioNode(masterFileInput);
            //var data = _source?.Source?.GetData();

            // converter
            //SourceConverter sourceConverter = new SourceConverter
            //{
            //    Source = _source.Source,

            //    AnalyzerTypes = AnalyzerType.RMS | AnalyzerType.Peak,
            //    RmsRiseTime = TimeSpan.FromMilliseconds(50),
            //    RmsFallTime = TimeSpan.FromMilliseconds(50),
            //    PeakRiseTime = TimeSpan.FromMilliseconds(500),
            //    PeakFallTime = TimeSpan.FromMilliseconds(500),

            //    //AnalyzerTypes = AnalyzerType.Spectrum,
            //    //SpectrumRiseTime = TimeSpan.FromMilliseconds(500),
            //    //SpectrumFallTime = TimeSpan.FromMilliseconds(500),

            //    //FrequencyCount =  12 * 5 * 5; // 5 octaves, 5 bars per note
            //    //MinFrequency = 110.0f;    // Note A2
            //    //MaxFrequency = 3520.0f;  // Note A7
            //    //FrequencyScale = ScaleType.Logarithmic,

            //    //CacheData = true,
            //    //ChannelCount = 2,
            //    //ChannelMapping = new float[] { 0 },
            //    //Fps = 60f,
            //    //IsSuspended = true,
            //    //PlaybackState =SourcePlaybackState.Playing
            //};
            //var data = sourceConverter.GetData();

            //VUBarChannel0.Source = sourceConverter;
            //VUBarChannel1.Source = sourceConverter;

            // ElementFactory
            //VUBarChannel0.ElementFactory
        }
    }
}