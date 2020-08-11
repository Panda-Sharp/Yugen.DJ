using AudioVisualizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Yugen.DJ.Audio.BPM;
using Yugen.DJ.Audio.WaveForm;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Standard.Mvvm.Input;

namespace Yugen.DJ.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        public WaveFormRenderer WaveFormRenderer = new WaveFormRenderer();
        private readonly IAudioService _audioService;

        private bool _isHeadPhones;
        private bool _isPaused = true;
        private string _playPauseButton = "\uE768";
        private string _artist;
        private string _title;
        private double _bpm;
        private double _volume = 100;
        private double _fader;
        private double _pitch = 0;
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private ICommand _openButtonCommand;

        public VinylViewModel(bool isLeft)
        {
            IsLeft = isLeft;
            IsHeadPhones = isLeft;

            _audioService = Ioc.Default.GetService<IAudioService>();
            _audioService.PositionChanged += AudioServiceOnPositionChanged;
            _audioService.FileLoaded += AudioServiceOnFileLoaded;
        }

        public CanvasControl WaveFormCanvas { get; private set; }

        public Vector3 ElementShadowOffset => new Vector3(2, 2, -10);
        public float ElementShadowBlurRadius => 5;
        public Color ElementShadowColor => Colors.Red;

        public MeterBarLevel[] Levels
        {
            get
            {
                // Create bar steps with 1db steps from -86db to +6
                const int fromDb = -86;
                const int toDb = 6;
                MeterBarLevel[] levels = new MeterBarLevel[toDb - fromDb];

                for (int i = 0; i < levels.Count(); i++)
                {
                    float ratio = (float)i / (float)levels.Count();
                    levels[i].Color = WaveFormRenderer.GradientColor(ratio);
                    levels[i].Level = i + fromDb;
                }

                return levels;
            }
        }

        public bool IsLeft { get; }

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                Set(ref _isHeadPhones, value);

                _audioService?.IsHeadphones(_isHeadPhones);
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                Set(ref _isPaused, value);

                _audioService.TogglePlay(_isPaused);

                PlayPauseButton = _isPaused ? "\uE768" : "\uE769";
            }
        }

        public string PlayPauseButton
        {
            get { return _playPauseButton; }
            set { Set(ref _playPauseButton, value); }
        }

        public string Artist
        {
            get { return _artist; }
            set { Set(ref _artist, value); }
        }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public double BPM
        {
            get { return _bpm; }
            set { Set(ref _bpm, value); }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                _audioService.ChangePitch(_pitch);
            }
        }

        public TimeSpan NaturalDuration
        {
            get { return _naturalDuration; }
            set { Set(ref _naturalDuration, value); }
        }

        public TimeSpan Position
        {
            get { return _position; }
            set { Set(ref _position, value); }
        }

        public ICommand OpenButtonCommand => _openButtonCommand
            ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        public async Task Init()
        {
            await _audioService.Init();
        }

        public void AddWaveForm(CanvasControl waveFormCanvas) => WaveFormCanvas = waveFormCanvas;

        internal void AddAudioVisualizer(DiscreteVUBar VUBarChannel0, DiscreteVUBar VUBarChannel1) =>
            _audioService.AddAudioVisualizer(VUBarChannel0, VUBarChannel1);

        private async void AudioServiceOnFileLoaded(object sender, Windows.Storage.StorageFile file)
        {
            var musicProps = await file.Properties.GetMusicPropertiesAsync();
            Artist = musicProps.Artist;
            Title = musicProps.Title;

            var stream = await file.OpenStreamForReadAsync();
            ISampleProvider isp;
            var samples = 0L;
            var buffer = new float[0];
            var sampleRate = 0;
            var totalMinutes = 0d;

            double bpm = 0;
            await Task.Run(() =>
            {
                using (var reader = new StreamMediaFoundationReader(stream))
                {
                    isp = reader.ToSampleProvider();
                    buffer = new float[reader.Length / 2];
                    isp.Read(buffer, 0, buffer.Length);

                    var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                    samples = reader.Length / bytesPerSample;

                    sampleRate = isp.WaveFormat.SampleRate;
                    totalMinutes = reader.TotalTime.TotalMinutes;
                }

                WaveFormRenderer.Render(isp, samples);

                var bpmDetector = new BPMDetector();
                bpm = bpmDetector.Detect(buffer, sampleRate, totalMinutes);
            });

            BPM = bpm;
            WaveFormCanvas.Invalidate();
        }

        private void AudioServiceOnPositionChanged(object sender, TimeSpan position)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Position = position;
            });
        }

        private async Task OpenButtonCommandBehavior()
        {
            IsPaused = true;

            await _audioService.OpenFile();

            NaturalDuration = _audioService.NaturalDuration;
        }
    }
}