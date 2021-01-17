using AudioVisualizer;
using System;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Yugen.DJ.Models;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;

namespace Yugen.DJ.Interfaces
{
    public interface IDockService
    {
        IWaveformRendererService WaveformRendererService { get; }

        TimeSpan NaturalDuration { get; }
        MusicProperties MusicProperties { get; }
        IVisualizationSource PlaybackSource { get; }

        event EventHandler<double> BpmGenerated;
        event EventHandler<TimeSpan> PositionChanged;
        event EventHandler AudioPropertiesLoaded;
        event EventHandler WaveformGenerated;

        void Init(Side side);
        Task LoadSong();
        void TogglePlay(bool v);
        void ChangePitch(double pitch);
    }
}