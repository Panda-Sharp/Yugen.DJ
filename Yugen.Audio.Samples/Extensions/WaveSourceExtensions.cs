using CSCore;
using System;
using System.IO;

namespace Yugen.Audio.Samples.Extensions
{
    public static class WaveSourceExtensions
    {
        public static byte[] ToByteArray(this IWaveSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            using (MemoryStream buffer = new MemoryStream())
            {
                int read;
                byte[] temporaryBuffer = new byte[source.WaveFormat.BytesPerSecond];
                while ((read = source.Read(temporaryBuffer, 0, temporaryBuffer.Length)) > 0)
                {
                    buffer.Write(temporaryBuffer, 0, read);
                }

                return buffer.ToArray();
            }
        }
    }
}