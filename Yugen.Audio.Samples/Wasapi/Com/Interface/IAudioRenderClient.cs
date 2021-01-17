using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
   [ComImport, Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   interface IAudioRenderClient
   {
      int GetBuffer(int numFramesRequested, out IntPtr ptr);
      int ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags flags);
   }
}
