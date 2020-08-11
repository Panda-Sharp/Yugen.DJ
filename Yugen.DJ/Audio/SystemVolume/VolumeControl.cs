using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Media.Devices;
using Yugen.DJ.Audio.SystemVolume.Interfaces;
using Yugen.DJ.Audio.SystemVolume.Models;

namespace Yugen.DJ.Audio.SystemVolume
{
    /// <summary>
    /// Control the system volume from UWP, using the IAudioEndpointVolume interface
    ///
    /// Wim Bokkers
    ///
    /// Credits:
    /// * Reddit user sunius (https://www.reddit.com/user/sunius)
    ///    See this thread: https://www.reddit.com/r/WPDev/comments/4kqzkb/launch_exe_with_parameter_in_uwp/d3jepi7/
    ///    And this code: https://pastebin.com/cPhVCyWj
    ///
    /// The code provided by sunius has two major drawbacks:
    ///   - It uses unsafe code
    ///   - It does not work in Release mode (the app crashes)
    ///
    /// Marshalling the Guid pointers as out parameters will fix this.
    ///
    /// This code is also available from: https://gist.github.com/wbokkers/74e05ccc1ee2371ec55c4a7daf551a26
    /// </summary>
    public static class VolumeControl
    {
        public static double GetVolume()
        {
            try
            {
                var masterVol = GetAudioEndpointVolumeInterface();
                if (masterVol == null)
                    return -1;

                // Make sure that the audio is not muted
                masterVol.SetMute(false, Guid.Empty);

                // Only adapt volume if the current level is below the specified minimum level
                return masterVol.GetMasterVolumeLevelScalar();
            }
            catch
            {
                return -1;
            }
        }

        public static void SetVolume(double level)
        {
            if (level < 0 || level > 1)
                return;

            try
            {
                var masterVol = GetAudioEndpointVolumeInterface();
                if (masterVol == null)
                    return;

                // Make sure that the audio is not muted
                masterVol.SetMute(false, Guid.Empty);
                var newAudioValue = Convert.ToSingle(level);

                masterVol.SetMasterVolumeLevelScalar(newAudioValue, Guid.Empty);
            }
            catch { }
        }

        private static IAudioEndpointVolume GetAudioEndpointVolumeInterface()
        {
            var speakerId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            var completionHandler = new ActivateAudioInterfaceCompletionHandler<IAudioEndpointVolume>();

            var hr = ActivateAudioInterfaceAsync(
                speakerId,
                typeof(IAudioEndpointVolume).GetTypeInfo().GUID,
                IntPtr.Zero,
                completionHandler,
                out var activateOperation);

            Debug.Assert(hr == (uint)HResult.S_OK);

            return completionHandler.WaitForCompletion();
        }

        [DllImport("Mmdevapi.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Error)]
        private static extern uint ActivateAudioInterfaceAsync(
                [In, MarshalAs(UnmanagedType.LPWStr)] string deviceInterfacePath,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [In] IntPtr activationParams,
                [In] IActivateAudioInterfaceCompletionHandler completionHandler,
                out IActivateAudioInterfaceAsyncOperation activationOperation);

        internal class ActivateAudioInterfaceCompletionHandler<T> : IActivateAudioInterfaceCompletionHandler
        {
            private AutoResetEvent _completionEvent;
            private T _result;

            public ActivateAudioInterfaceCompletionHandler()
            {
                _completionEvent = new AutoResetEvent(false);
            }

            public void ActivateCompleted(IActivateAudioInterfaceAsyncOperation operation)
            {
                operation.GetActivateResult(out var hr, out var activatedInterface);

                Debug.Assert(hr == (uint)HResult.S_OK);

                _result = (T)activatedInterface;

                var setResult = _completionEvent.Set();
                Debug.Assert(setResult != false);
            }

            public T WaitForCompletion()
            {
                var waitResult = _completionEvent.WaitOne();
                Debug.Assert(waitResult != false);

                return _result;
            }
        }
    }
}