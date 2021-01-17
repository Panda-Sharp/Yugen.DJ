using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
   [ComImport, Guid("C8ADBD64-E71E-48a0-A4DE-185C395CD317"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   interface IAudioCaptureClient
   {
      int GetBuffer(out IntPtr data, out int numFramesRead, out AudioClientBufferFlags flags, out long devicePosition, out long qpcPosition);
      int ReleaseBuffer(int numFramesRead);
   }
}
