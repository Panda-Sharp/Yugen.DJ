using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WASAPI.NET.Com;

namespace WASAPI.NET
{
   public class WasapiInDriver : WasapiDriver, IAudioInDriver
   {
      private static readonly Guid IID_IAudioCaptureClient = new Guid("C8ADBD64-E71E-48a0-A4DE-185C395CD317");
      
      private Action<IAudioInDriver, float[][]> receiveCallback;
      private IAudioCaptureClient audioCaptureClient;

      #region Capture

      private void Capture()
      {
         if (outputIsFloat)
         {
            switch (bitsPerSample)
            {
               case 32: CaptureF32(); break;
               case 64: CaptureF64(); break;
            }
         }
         else
         {
            switch (bitsPerSample)
            {
               case 16: CaptureI16(); break;
               case 32: CaptureI32(); break;
            }
         }
      }
      
      #region I16

      private void CaptureI16()
      {
         IntPtr ptr = IntPtr.Zero;
         int totalSamples, samples = 0;
         try
         {
            long a, b; AudioClientBufferFlags flags;
            audioCaptureClient.GetBuffer(out ptr, out samples, out flags, out a, out b);
            if (ptr == IntPtr.Zero || samples <= 0) { return; }
            totalSamples = samples * channelCount;
            if (i16Buf == null || i16Buf.Length < totalSamples) { i16Buf = new short[totalSamples]; }
            Marshal.Copy(ptr, i16Buf, 0, totalSamples);
         }
         finally
         {
            if (ptr != IntPtr.Zero) { audioCaptureClient.ReleaseBuffer(samples); }
         }
         int index = 0;
         while (index < samples)
         {
            int samplesToWrite = bufferSize - samplesWritten;
            int samplesLeft = samples - index;
            if (samplesToWrite > samplesLeft) { samplesToWrite = samplesLeft; }
            int sampleEnd = samplesWritten + samplesToWrite;
            for (int c = 0; c < channelCount; c++)
            {
               float[] buffer = buffers[c];
               int readIndex = index * channelCount + c;
               for (int sample = samplesWritten; sample < sampleEnd; sample++, readIndex += channelCount)
               {
                  buffer[sample] = i16Buf[readIndex] * (1.0f / 32768.0f);
               }
            }
            index += samplesToWrite;
            samplesWritten += samplesToWrite;
            if (samplesWritten >= bufferSize)
            {
               receiveCallback(this, buffers);
               samplesWritten = 0;
            }
         }
      }

      #endregion
      
      #region I32

      private void CaptureI32()
      {
         IntPtr ptr = IntPtr.Zero;
         int totalSamples, samples = 0;
         try
         {
            long a, b; AudioClientBufferFlags flags;
            audioCaptureClient.GetBuffer(out ptr, out samples, out flags, out a, out b);
            if (ptr == IntPtr.Zero || samples <= 0) { return; }
            totalSamples = samples * channelCount;
            if (i32Buf == null || i32Buf.Length < totalSamples) { i32Buf = new int[totalSamples]; }
            Marshal.Copy(ptr, i32Buf, 0, totalSamples);
         }
         finally
         {
            if (ptr != IntPtr.Zero) { audioCaptureClient.ReleaseBuffer(samples); }
         }
         int index = 0;
         while (index < samples)
         {
            int samplesToWrite = bufferSize - samplesWritten;
            int samplesLeft = samples - index;
            if (samplesToWrite > samplesLeft) { samplesToWrite = samplesLeft; }
            int sampleEnd = samplesWritten + samplesToWrite;
            for (int c = 0; c < channelCount; c++)
            {
               float[] buffer = buffers[c];
               int readIndex = index * channelCount + c;
               for (int sample = samplesWritten; sample < sampleEnd; sample++, readIndex += channelCount)
               {
                  buffer[sample] = (float)(i32Buf[readIndex] * (1.0 / 2147483648.0));
               }
            }
            index += samplesToWrite;
            samplesWritten += samplesToWrite;
            if (samplesWritten >= bufferSize)
            {
               receiveCallback(this, buffers);
               samplesWritten = 0;
            }
         }
      }

      #endregion

      #region F32

      private void CaptureF32()
      {
         IntPtr ptr = IntPtr.Zero;
         int totalSamples, samples = 0;
         try
         {
            long a, b; AudioClientBufferFlags flags;
            audioCaptureClient.GetBuffer(out ptr, out samples, out flags, out a, out b);
            if (ptr == IntPtr.Zero || samples <= 0) { return; }
            totalSamples = samples * channelCount;
            if (f32Buf == null || f32Buf.Length < totalSamples) { f32Buf = new float[totalSamples]; }
            Marshal.Copy(ptr, f32Buf, 0, totalSamples);
         }
         finally
         {
            if (ptr != IntPtr.Zero) { audioCaptureClient.ReleaseBuffer(samples); }
         }
         int index = 0;
         while (index < samples)
         {
            int samplesToWrite = bufferSize - samplesWritten;
            int samplesLeft = samples - index;
            if (samplesToWrite > samplesLeft) { samplesToWrite = samplesLeft; }
            int sampleEnd = samplesWritten + samplesToWrite;
            for (int c = 0; c < channelCount; c++)
            {
               float[] buffer = buffers[c];
               int readIndex = index * channelCount + c;
               for (int sample = samplesWritten; sample < sampleEnd; sample++, readIndex += channelCount)
               {
                  buffer[sample] = f32Buf[readIndex];
               }
            }
            index += samplesToWrite;
            samplesWritten += samplesToWrite;
            if (samplesWritten >= bufferSize)
            {
               receiveCallback(this, buffers);
               samplesWritten = 0;
            }
         }
      }

      #endregion

      #region F64

      private void CaptureF64()
      {
         IntPtr ptr = IntPtr.Zero;
         int totalSamples, samples = 0;
         try
         {
            long a, b; AudioClientBufferFlags flags;
            audioCaptureClient.GetBuffer(out ptr, out samples, out flags, out a, out b);
            if (ptr == IntPtr.Zero || samples <= 0) { return; }
            totalSamples = samples * channelCount;
            if (f64Buf == null || f64Buf.Length < totalSamples) { f64Buf = new double[totalSamples]; }
            Marshal.Copy(ptr, f64Buf, 0, totalSamples);
         }
         finally
         {
            if (ptr != IntPtr.Zero) { audioCaptureClient.ReleaseBuffer(samples); }
         }
         int index = 0;
         while (index < samples)
         {
            int samplesToWrite = bufferSize - samplesWritten;
            int samplesLeft = samples - index;
            if (samplesToWrite > samplesLeft) { samplesToWrite = samplesLeft; }
            int sampleEnd = samplesWritten + samplesToWrite;
            for (int c = 0; c < channelCount; c++)
            {
               float[] buffer = buffers[c];
               int readIndex = index * channelCount + c;
               for (int sample = samplesWritten; sample < sampleEnd; sample++, readIndex += channelCount)
               {
                  buffer[sample] = (float)f64Buf[readIndex];
               }
            }
            index += samplesToWrite;
            samplesWritten += samplesToWrite;
            if (samplesWritten >= bufferSize)
            {
               receiveCallback(this, buffers);
               samplesWritten = 0;
            }
         }
      }

      #endregion

      #endregion

      protected virtual int DataFlow { get { return (int)DataFlowEnum.Capture; } }
      protected virtual int StreamFlags { get { return (int)AudioClientStreamFlagsEnum.None; } }

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
                     int dataLeft = GetCurrentPadding();
                     if (dataLeft <= 0) { break; }
                     Capture();
                  }
                  while (true);
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

      public void Setup(Action<IAudioInDriver, float[][]> receiveCallback, int bufferSize = 0)
      {
         if (receiveCallback == null) { throw new ArgumentNullException("renderCallback"); }
         Stop();
         try
         {
            Cleanup();
            InternalSetup(DataFlow, StreamFlags, bufferSize);
            if ((outputIsFloat && bitsPerSample != 32 && bitsPerSample != 64) ||
                (!outputIsFloat && bitsPerSample != 16 && bitsPerSample != 32)) { throw new NotSupportedException(); }
            audioCaptureClient = GetService<IAudioCaptureClient>(IID_IAudioCaptureClient);
            this.receiveCallback = receiveCallback;
         }
         catch
         {
            Dispose();
            throw;
         }
      }

      protected override void Cleanup()
      {
         if (audioCaptureClient != null)
         {
            Marshal.ReleaseComObject(audioCaptureClient);
            audioCaptureClient = null;
         }
      }
   }
}
