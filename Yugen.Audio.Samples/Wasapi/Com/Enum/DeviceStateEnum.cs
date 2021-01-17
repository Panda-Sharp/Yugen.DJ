using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WASAPI.NET.Com
{
   [Flags]
   internal enum DeviceStateEnum
   {
      Active = 0x00000001,
      Disabled = 0x00000002,
      NotPresent = 0x00000004,
      Unplugged = 0x00000008,
      All = 0x0000000F
   }
}
