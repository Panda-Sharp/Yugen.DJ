using SharpDX;
using SharpDX.IO;
using SharpDX.MediaFoundation;
using SharpDX.XAudio2;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class SharpDXAudioPlayer : IAudioPlayer
    {
        private const int WaitPrecision = 1;

        private readonly XAudio2 _xaudio2;
        private MasteringVoice _masteringVoice;
        private AudioDecoder _audioDecoder;
        private SourceVoice _sourceVoice;

        private AudioBuffer[] _audioBuffersRing;
        private DataPointer[] _memBuffers;
        private Stopwatch _clock;
        private ManualResetEvent _playEvent;
        private ManualResetEvent _waitForPlayToOutput;
        private AutoResetEvent _bufferEndEvent;
        private TimeSpan _playPosition;
        private TimeSpan _nextPlayPosition;
        private TimeSpan _playPositionStart;
        private Task _playingTask;
        private int _playCounter;
        private float _localVolume;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDXAudioPlayer" /> class.
        /// </summary>
        public SharpDXAudioPlayer()
        {
            // This is mandatory when using any of SharpDX.MediaFoundation classes
            MediaManager.Startup();

            // Starts The XAudio2 engine
            _xaudio2 = new XAudio2(XAudio2Version.Version29);
            _xaudio2.StartEngine();
        }

        /// <summary>
        /// Gets the state of this instance.
        /// </summary>
        /// <value>The state.</value>
        public AudioPlayerState State { get; private set; }

        /// <summary>
        /// Gets the duration in seconds of the current sound.
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get { return _audioDecoder.Duration; }
        }

        /// <summary>
        /// Gets or sets the position in seconds.
        /// </summary>
        /// <value>The position.</value>
        public TimeSpan Position
        {
            get { return _playPosition; }
            set
            {
                _playPosition = value;
                _nextPlayPosition = value;
                _playPositionStart = value;
                _clock.Restart();
                _playCounter++;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to the sound is looping when the end of the buffer is reached.
        /// </summary>
        /// <value><c>true</c> if to loop the sound; otherwise, <c>false</c>.</value>
        public bool IsRepeating { get; set; }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>The volume.</value>
        public float Volume
        {
            get { return _localVolume; }
            set
            {
                _localVolume = value;
                _sourceVoice.SetVolume(value);
            }
        }


        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            _masteringVoice = new MasteringVoice(_xaudio2, inputChannels, inputSampleRate, deviceId);
        }

        public async Task LoadFile(StorageFile tmpAudioFile)
        {
            var fileStream = new NativeFileStream(tmpAudioFile.Path, NativeFileMode.Open, NativeFileAccess.Read);

            LoadStream(fileStream);
        }

        public void LoadStream(Stream audioStream)
        {
            _audioDecoder = new AudioDecoder(audioStream);
            _sourceVoice = new SourceVoice(_xaudio2, _audioDecoder.WaveFormat);
            _localVolume = 1.0f;

            _sourceVoice.BufferEnd += SourceVoiceBufferEnd;
            _sourceVoice.Start();

            _bufferEndEvent = new AutoResetEvent(false);
            _playEvent = new ManualResetEvent(false);
            _waitForPlayToOutput = new ManualResetEvent(false);

            _clock = new Stopwatch();

            // Pre-allocate buffers
            _audioBuffersRing = new AudioBuffer[3];
            _memBuffers = new DataPointer[_audioBuffersRing.Length];
            for (int i = 0; i < _audioBuffersRing.Length; i++)
            {
                _audioBuffersRing[i] = new AudioBuffer();
                _memBuffers[i].Size = 32 * 1024; // default size 32Kb
                _memBuffers[i].Pointer = Utilities.AllocateMemory(_memBuffers[i].Size);
            }

            // Initialize to stopped
            State = AudioPlayerState.Stopped;

            // Starts the playing thread
            _playingTask = Task.Factory.StartNew(PlayAsync, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Close this audio player.
        /// </summary>
        /// <remarks>
        /// This is similar to call Stop(), Dispose(), Wait().
        /// </remarks>
        public void Close()
        {
            Stop();
            _isDisposed = true;
            Wait();
        }


        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            if (State != AudioPlayerState.Playing)
            {
                bool waitForFirstPlay = false;
                if (State == AudioPlayerState.Stopped)
                {
                    _playCounter++;
                    _waitForPlayToOutput.Reset();
                    waitForFirstPlay = true;
                }
                else
                {
                    // The song was paused
                    _clock.Start();
                }

                State = AudioPlayerState.Playing;
                _playEvent.Set();

                if (waitForFirstPlay)
                {
                    _waitForPlayToOutput.WaitOne();
                }
            }
        }

        public void PlayWithoutStreaming() { }

        /// <summary>
        /// Pauses the sound.
        /// </summary>
        public void Pause()
        {
            if (State == AudioPlayerState.Playing)
            {
                _clock.Stop();
                State = AudioPlayerState.Paused;
                _playEvent.Reset();
            }
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        public void Stop()
        {
            if (State != AudioPlayerState.Stopped)
            {
                _playPosition = TimeSpan.Zero;
                _nextPlayPosition = TimeSpan.Zero;
                _playPositionStart = TimeSpan.Zero;
                _clock.Stop();
                _playCounter++;
                State = AudioPlayerState.Stopped;
                _playEvent.Reset();
            }
        }

        /// <summary>
        /// Wait that the player is finished.
        /// </summary>
        public void Wait()
        {
            _playingTask.Wait();
        }


        /// <summary>
        /// Internal method to play the sound.
        /// </summary>
        private void PlayAsync()
        {
            int nextBuffer = 0;

            try
            {
                while (true)
                {
                    // Check that this instanced is not disposed
                    while (!_isDisposed)
                    {
                        if (_playEvent.WaitOne(WaitPrecision))
                            break;
                    }

                    if (_isDisposed)
                        break;

                    _clock.Restart();
                    _playPositionStart = _nextPlayPosition;
                    _playPosition = _playPositionStart;
                    int currentPlayCounter = _playCounter;

                    // Get the decoded samples from the specified starting position.
                    var sampleIterator = _audioDecoder.GetSamples(_playPositionStart).GetEnumerator();

                    bool isFirstTime = true;

                    bool endOfSong = false;

                    // Playing all the samples
                    while (true)
                    {
                        // If the player is stopped or disposed, then break of this loop
                        while (!_isDisposed && State != AudioPlayerState.Stopped)
                        {
                            if (_playEvent.WaitOne(WaitPrecision))
                                break;
                        }

                        // If the player is stopped or disposed, then break of this loop
                        if (_isDisposed || State == AudioPlayerState.Stopped)
                        {
                            _nextPlayPosition = TimeSpan.Zero;
                            break;
                        }

                        // If there was a change in the play position, restart the sample iterator.
                        if (currentPlayCounter != _playCounter)
                            break;

                        // If ring buffer queued is full, wait for the end of a buffer.
                        while (_sourceVoice.State.BuffersQueued == _audioBuffersRing.Length && !_isDisposed && State != AudioPlayerState.Stopped)
                            _bufferEndEvent.WaitOne(WaitPrecision);

                        // If the player is stopped or disposed, then break of this loop
                        if (_isDisposed || State == AudioPlayerState.Stopped)
                        {
                            _nextPlayPosition = TimeSpan.Zero;
                            break;
                        }

                        // Check that there is a next sample
                        if (!sampleIterator.MoveNext())
                        {
                            endOfSong = true;
                            break;
                        }

                        // Retrieve a pointer to the sample data
                        var bufferPointer = sampleIterator.Current;

                        // If there was a change in the play position, restart the sample iterator.
                        if (currentPlayCounter != _playCounter)
                            break;

                        // Check that our ring buffer has enough space to store the audio buffer.
                        if (bufferPointer.Size > _memBuffers[nextBuffer].Size)
                        {
                            if (_memBuffers[nextBuffer].Pointer != IntPtr.Zero)
                                Utilities.FreeMemory(_memBuffers[nextBuffer].Pointer);

                            _memBuffers[nextBuffer].Pointer = Utilities.AllocateMemory(bufferPointer.Size);
                            _memBuffers[nextBuffer].Size = bufferPointer.Size;
                        }

                        // Copy the memory from MediaFoundation AudioDecoder to the buffer that is going to be played.
                        Utilities.CopyMemory(_memBuffers[nextBuffer].Pointer, bufferPointer.Pointer, bufferPointer.Size);

                        // Set the pointer to the data.
                        _audioBuffersRing[nextBuffer].AudioDataPointer = _memBuffers[nextBuffer].Pointer;
                        _audioBuffersRing[nextBuffer].AudioBytes = bufferPointer.Size;

                        // If this is a first play, restart the clock and notify play method.
                        if (isFirstTime)
                        {
                            _clock.Restart();
                            isFirstTime = false;

                            _waitForPlayToOutput.Set();
                        }

                        // Update the current position used for sync
                        _playPosition = new TimeSpan(_playPositionStart.Ticks + _clock.Elapsed.Ticks);

                        // Submit the audio buffer to xaudio2
                        _sourceVoice.SubmitSourceBuffer(_audioBuffersRing[nextBuffer], null);

                        // Go to next entry in the ringg audio buffer
                        nextBuffer = ++nextBuffer % _audioBuffersRing.Length;
                    }

                    // If the song is not looping (by default), then stop the audio player.
                    if (endOfSong && !IsRepeating && State == AudioPlayerState.Playing)
                    {
                        Stop();
                    }
                }
            }
            finally
            {
                DisposePlayer();
            }
        }

        private void DisposePlayer()
        {
            _audioDecoder.Dispose();
            _audioDecoder = null;

            _sourceVoice.Dispose();
            _sourceVoice = null;

            for (int i = 0; i < _audioBuffersRing.Length; i++)
            {
                Utilities.FreeMemory(_memBuffers[i].Pointer);
                _memBuffers[i].Pointer = IntPtr.Zero;
            }
        }

        private void SourceVoiceBufferEnd(IntPtr obj)
        {
            _bufferEndEvent.Set();
        }

        public void Record(StorageFile audioFile)
        {
            throw new NotImplementedException();
        }
    }
}