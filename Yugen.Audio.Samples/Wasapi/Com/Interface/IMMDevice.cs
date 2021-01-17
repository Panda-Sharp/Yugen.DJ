using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
   [ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
   interface IMMDevice
   {
      int Activate(ref Guid id, ClsCtxEnum clsCtx, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);

      int OpenPropertyStore(StorageAccessModeEnum stgmAccess, [MarshalAs(UnmanagedType.IUnknown)] out object properties);

      int GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);

      int GetState(out DeviceStateEnum state);
   }
}
