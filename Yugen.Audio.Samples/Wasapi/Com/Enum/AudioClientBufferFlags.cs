using System;

namespace WASAPI.NET.Com
{
    [Flags]
    public enum AudioClientBufferFlags
    {
        None = 0x0,
        DataDiscontinuity = 0x1,
        Silent = 0x2,
        TimestampError = 0x3
    };
}