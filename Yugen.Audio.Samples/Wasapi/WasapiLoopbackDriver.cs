using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WASAPI.NET.Com;

namespace WASAPI.NET
{
   public class WasapiLoopbackDriver : WasapiInDriver
   {
      protected override int DataFlow { get { return (int)DataFlowEnum.Render; } }
      protected override int StreamFlags { get { return (int)AudioClientStreamFlagsEnum.StreamFlagsLoopback; } }
   }
}
