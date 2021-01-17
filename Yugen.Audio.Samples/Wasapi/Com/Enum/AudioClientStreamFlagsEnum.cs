using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WASAPI.NET.Com
{
   [Flags]
   public enum AudioClientStreamFlagsEnum
   {
      None = 0x0,
      StreamFlagsLoopback = 0x00020000,
   };
}
