using System;
using System.Runtime.InteropServices;

namespace Yugen.DJ.Audio.SystemVolume.Interfaces
{
    [ComImport]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioEndpointVolume
    {
        void NotImpl1();

        void NotImpl2();

        [return: MarshalAs(UnmanagedType.U4)]
        uint GetChannelCount();

        void SetMasterVolumeLevel(
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        void SetMasterVolumeLevelScalar(
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [return: MarshalAs(UnmanagedType.R4)]
        float GetMasterVolumeLevel();

        [return: MarshalAs(UnmanagedType.R4)]
        float GetMasterVolumeLevelScalar();

        void SetChannelVolumeLevel(
            [In][MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        void SetChannelVolumeLevelScalar(
            [In][MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [In][MarshalAs(UnmanagedType.R4)] float level,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        void GetChannelVolumeLevel(
            [In][MarshalAs(UnmanagedType.U4)] uint channelNumber,
            [Out][MarshalAs(UnmanagedType.R4)] out float level);

        [return: MarshalAs(UnmanagedType.R4)]
        float GetChannelVolumeLevelScalar([In][MarshalAs(UnmanagedType.U4)] uint channelNumber);

        void SetMute(
            [In][MarshalAs(UnmanagedType.Bool)] bool isMuted,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetMute();

        void GetVolumeStepInfo(
            [Out][MarshalAs(UnmanagedType.U4)] out uint step,
            [Out][MarshalAs(UnmanagedType.U4)] out uint stepCount);

        void VolumeStepUp([In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        void VolumeStepDown([In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

        [return: MarshalAs(UnmanagedType.U4)] // bit mask
        uint QueryHardwareSupport();

        void GetVolumeRange(
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeMin,
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeMax,
            [Out][MarshalAs(UnmanagedType.R4)] out float volumeStep);
    }
}