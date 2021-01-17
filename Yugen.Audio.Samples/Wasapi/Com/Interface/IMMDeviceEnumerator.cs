using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
   [ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   interface IMMDeviceEnumerator
   {
      int EnumAudioEndpoints(DataFlowEnum dataFlow, DeviceStateEnum stateMask, [MarshalAs(UnmanagedType.IUnknown)] out object devices);

      [PreserveSig]
      int GetDefaultAudioEndpoint(DataFlowEnum dataFlow, RoleEnum role, out IMMDevice endpoint);

      int GetDevice(string id, out IMMDevice deviceName);

      int RegisterEndpointNotificationCallback([MarshalAs(UnmanagedType.IUnknown)] object client);

      int UnregisterEndpointNotificationCallback([MarshalAs(UnmanagedType.IUnknown)] object client);
   }
}
