using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WASAPI.NET.Com;
using System.Threading;

namespace WASAPI.NET
{
   public class WasapiOutDriver : WasapiDriver, IAudioOutDriver
   {
      private static readonly Guid IID_IAudioRenderClient = new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
      
      private Func<IAudioOutDriver, float[][], int> renderCallback;
      private IAudioRenderClient audioRenderClient;

      public WasapiOutDriver() : base() { }

      #region Render

      private int Render()
      {
         if (outputIsFloat)
         {
            switch (bitsPerSample)
            {
               case 32: return RenderF32();
               case 64: return RenderF64();
            }
         }
         else
         {
            switch (bitsPerSample)
            {
               case 16: return RenderI16();
               case 32: return RenderI32();
            }
         }
         return 0;
      }
      
      #region I16

      private int RenderI16()
      {
         int i, c, sample, samplesToRender, totalSamples, writeIndex;
         int sampleStart = samplesRead;
         int sampleEnd = samplesWritten;
         samplesToRender = sampleStart + (int)outputBufferSize - GetCurrentPadding();
         if (sampleEnd > samplesToRender) { sampleEnd = samplesToRender; }
         samplesToRender = sampleEnd - sampleStart;
         if (samplesToRender <= 0) { return 0; }
         totalSamples = samplesToRender * channelCount;
         if (i16Buf == null || i16Buf.Length < totalSamples) { i16Buf = new short[totalSamples]; }
         for (c = 0; c < channelCount; c++)
         {
            float[] chanBuffer = buffers[c];
            for (sample = sampleStart, writeIndex = c; sample < sampleEnd; sample++, writeIndex += channelCount)
            {
               i = (int)(chanBuffer[sample] * 32767);
               if (i < -32768) { i = -32768; } else if (i > 32767) { i = 32767; }
               i16Buf[writeIndex] = (short)i;
            }
         }
         samplesRead += samplesToRender;
         IntPtr ptr; audioRenderClient.GetBuffer(samplesToRender, out ptr);
         Marshal.Copy(i16Buf, 0, ptr, totalSamples);
         audioRenderClient.ReleaseBuffer(samplesToRender, AudioClientBufferFlags.None);
         return samplesToRender;
      }

      #endregion

      #region I32

      private int RenderI32()
      {
         int c, sample, samplesToRender, totalSamples, writeIndex;
         int sampleStart = samplesRead;
         int sampleEnd = samplesWritten;
         samplesToRender = sampleStart + (int)outputBufferSize - GetCurrentPadding();
         if (sampleEnd > samplesToRender) { sampleEnd = samplesToRender; }
         samplesToRender = sampleEnd - sampleStart;
         if (samplesToRender <= 0) { return 0; }
         totalSamples = samplesToRender * channelCount;
         if (i32Buf == null || i32Buf.Length < totalSamples) { i32Buf = new int[totalSamples]; }
         for (c = 0; c < channelCount; c++)
         {
            float[] chanBuffer = buffers[c];
            for (sample = sampleStart, writeIndex = c; sample < sampleEnd; sample++, writeIndex += channelCount)
            {
               long v = (long)(chanBuffer[sample] * 2147483647);
               if (v < -2147483648) { v = -2147483648; } else if (v > 2147483647) { v = 2147483647; }
               i32Buf[writeIndex] = (int)v;
            }
         }
         samplesRead += samplesToRender;
         IntPtr ptr; audioRenderClient.GetBuffer(samplesToRender, out ptr);
         Marshal.Copy(i32Buf, 0, ptr, totalSamples);
         audioRenderClient.ReleaseBuffer(samplesToRender, AudioClientBufferFlags.None);
         return samplesToRender;
      }

      #endregion

      #region F32

      private int RenderF32()
      {
         int c, sample, samplesToRender, totalSamples, writeIndex;
         int sampleStart = samplesRead;
         int sampleEnd = samplesWritten;
         samplesToRender = sampleStart + (int)outputBufferSize - GetCurrentPadding();
         if (sampleEnd > samplesToRender) { sampleEnd = samplesToRender; }
         samplesToRender = sampleEnd - sampleStart;
         if (samplesToRender <= 0) { return 0; }
         totalSamples = samplesToRender * channelCount;
         if (f32Buf == null || f32Buf.Length < totalSamples) { f32Buf = new float[totalSamples]; }
         for (c = 0; c < channelCount; c++)
         {
            float[] chanBuffer = buffers[c];
            for (sample = sampleStart, writeIndex = c; sample < sampleEnd; sample++, writeIndex += channelCount)
            {
               f32Buf[writeIndex] = chanBuffer[sample];
            }
         }
         samplesRead += samplesToRender;
         IntPtr ptr; audioRenderClient.GetBuffer(samplesToRender, out ptr);
         Marshal.Copy(f32Buf, 0, ptr, totalSamples);
         audioRenderClient.ReleaseBuffer(samplesToRender, AudioClientBufferFlags.None);
         return samplesToRender;
      }

      #endregion

      #region F64

      private int RenderF64()
      {
         int c, sample, samplesToRender, totalSamples, writeIndex;
         int sampleStart = samplesRead;
         int sampleEnd = samplesWritten;
         samplesToRender = sampleStart + (int)outputBufferSize - GetCurrentPadding();
         if (sampleEnd > samplesToRender) { sampleEnd = samplesToRender; }
         samplesToRender = sampleEnd - sampleStart;
         if (samplesToRender <= 0) { return 0; }
         totalSamples = samplesToRender * channelCount;
         if (f64Buf == null || f64Buf.Length < totalSamples) { f64Buf = new double[totalSamples]; }
         for (c = 0; c < channelCount; c++)
         {
            float[] chanBuffer = buffers[c];
            for (sample = sampleStart, writeIndex = c; sample < sampleEnd; sample++, writeIndex += channelCount)
            {
               f64Buf[writeIndex] = chanBuffer[sample];
            }
         }
         samplesRead += samplesToRender;
         IntPtr ptr; audioRenderClient.GetBuffer(samplesToRender, out ptr);
         Marshal.Copy(f64Buf, 0, ptr, totalSamples);
         audioRenderClient.ReleaseBuffer(samplesToRender, AudioClientBufferFlags.None);
         return samplesToRender;
      }

      #endregion

      #endregion

      protected override void RunThread()
      {
         int sleepMs = latencyMs >> 1;
         if (sleepMs <= 0) { sleepMs = 1; }
         try
         {
            do
            {
               lock (mutex)
               {
                  if (!isStarted || !isInited || bufferSize <= 0) { return; }
                  do
                  {
                     if (samplesRead >= samplesWritten)
                     {
                        samplesRead = 0;
                        samplesWritten = renderCallback(this, buffers);
                        if (samplesWritten > bufferSize) { samplesWritten = bufferSize; }
                        if (samplesWritten <= 0)
                        {
                           samplesWritten = bufferSize;
                           for (int i = 0; i < buffers.Length; i++) { Array.Clear(buffers[i], 0, bufferSize); }
                        }
                     }
                  }
                  while (Render() > 0);
               }
               Thread.Sleep(sleepMs);
            }
            while (true);
         }
         catch
         {
            Stop();
         }
      }
      
      public void Setup(Func<IAudioOutDriver, float[][], int> renderCallback, int bufferSize = 0)
      {
         if (renderCallback == null) { throw new ArgumentNullException("renderCallback"); }
         Stop();
         try
         {
            Cleanup();
            InternalSetup((int)DataFlowEnum.Render, (int)AudioClientStreamFlagsEnum.None, bufferSize);
            if ((outputIsFloat && bitsPerSample != 32 && bitsPerSample != 64) ||
                (!outputIsFloat && bitsPerSample != 16 && bitsPerSample != 32)) { throw new NotSupportedException(); }
            audioRenderClient = GetService<IAudioRenderClient>(IID_IAudioRenderClient);
            this.renderCallback = renderCallback;
         }
         catch
         {
            Dispose();
            throw;
         }
      }

      protected override void Cleanup()
      {
         if (audioRenderClient != null)
         {
            Marshal.ReleaseComObject(audioRenderClient);
            audioRenderClient = null;
         }
      }
   }
}
