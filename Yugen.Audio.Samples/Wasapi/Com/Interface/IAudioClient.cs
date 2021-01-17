using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
   [ComImport, Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   interface IAudioClient
   {
      [PreserveSig]
      int Initialize(AudioClientShareModeEnum shareMode,
          AudioClientStreamFlagsEnum streamFlags,
          long hnsBufferDuration,
          long hnsPeriodicity,
          IntPtr pFormat,
          [MarshalAs(UnmanagedType.LPStruct)] Guid audioSessionGuid);

      int GetBufferSize(out uint bufferSize);

      [return: MarshalAs(UnmanagedType.I8)]
      long GetStreamLatency();

      int GetCurrentPadding(out int currentPadding);

      [PreserveSig]
      int IsFormatSupported(
          AudioClientShareModeEnum shareMode,
          IntPtr pFormat,
          IntPtr closestMatchPtr);

      int GetMixFormat(out IntPtr deviceFormatPointer);
   
      int GetDevicePeriod(out long defaultDevicePeriod, out long minimumDevicePeriod);

      int Start();

      int Stop();

      int Reset();

      int SetEventHandle(IntPtr eventHandle);

      [PreserveSig]
      int GetService([MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
         [Out, MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
   }
}
