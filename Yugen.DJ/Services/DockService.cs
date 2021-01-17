using AudioVisualizer;
using System;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Yugen.DJ.Interfaces;
using Yugen.DJ.Models;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;

namespace Yugen.DJ.Services
{
    public class DockService : IDockService
    {
        public IWaveformRendererService WaveformRendererService { get; private set; }

        private IAudioPlaybackService _audioPlaybackService;
        private readonly IAppService _appService;
        private readonly IBPMService _bpmService;
        private readonly ISongService _songService;
        private readonly IAudioVisualizerService _audioVisualizerService;

        private Side _side;

        public DockService(IAppService appService, IBPMService bpmService, ISongService songService,
            IWaveformRendererService waveformRendererService, IAudioVisualizerService audioVisualizerService)
        {
            _appService = appService;
            _audioVisualizerService = audioVisualizerService;
            _bpmService = bpmService;
            _songService = songService;

            WaveformRendererService = waveformRendererService;

            _songService.AudioDataGenerated += OnAudioDataGenerated;
        }


        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler AudioPropertiesLoaded;

        public event EventHandler<double> BpmGenerated;

        public event EventHandler WaveformGenerated;

        public TimeSpan NaturalDuration => _audioPlaybackService?.NaturalDuration ?? new TimeSpan();

        public MusicProperties MusicProperties => _songService?.MusicProperties;

        public IVisualizationSource PlaybackSource =>
            _audioVisualizerService.GetPlaybackSource(_audioPlaybackService?.MasterFileInput)?.Source;

        public void Init(Side side)
        {
            _side = side;

            _audioPlaybackService = _appService.AudioPlaybackService(_side);
            _audioPlaybackService.Init();

            _audioPlaybackService.PositionChanged += (sender, e) => PositionChanged?.Invoke(sender, e);
        }

        public async Task LoadSong()
        {
            await _songService.LoadFile();
            await _audioPlaybackService.LoadSong(_songService.AudioFile);

            AudioPropertiesLoaded?.Invoke(this, EventArgs.Empty);

            _ = Task.Run(async () => await _songService.GenerateAudioData(_songService.AudioFile));
        }

        public void TogglePlay(bool v) => _audioPlaybackService.TogglePlay(v);

        private void OnAudioDataGenerated(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                var bpm = _bpmService.Detect(_songService.Buffer, _songService.SampleRate, _songService.TotalMinutes);
                BpmGenerated?.Invoke(this, bpm);

                WaveformRendererService.Render(_songService.Isp, _songService.Samples);
                WaveformGenerated?.Invoke(this, EventArgs.Empty);
            });
        }

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);
    }
}