using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WASAPI.NET.Com;
using System.Threading;

namespace WASAPI.NET
{
   public abstract class WasapiDriver
   {
      protected static readonly Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
      protected static readonly Guid IeeeFloat = new Guid(0x00000003, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

      protected readonly object mutex = new object();
      protected bool isInited;
      protected bool isStarted;
      protected double sampleRate;
      protected int channelCount;
      protected int bitsPerSample;
      protected int bufferSize;
      protected int latencyMs;
      protected float[][] buffers;
      protected int samplesRead, samplesWritten;
      protected Thread thread;

      private IMMDeviceEnumerator enumerator;
      private IMMDevice endpoint;
      private IAudioClient audioClient;

      protected uint outputBufferSize;
      protected bool outputIsFloat;
      protected double[] f64Buf;
      protected float[] f32Buf;
      protected int[] i32Buf;
      protected short[] i16Buf;
      protected byte[] i8Buf;

      public bool IsStarted { get { lock (mutex) { return isStarted; } } }
      public double SampleRate { get { return sampleRate; } }
      public int ChannelCount { get { return channelCount; } }
      public int BufferSize { get { return bufferSize; } }

      public static bool IsSupported { get { return Environment.OSVersion.Version.Major >= 6; } }

      public WasapiDriver()
      {
         if (!IsSupported) { throw new NotSupportedException(); }
      }
      
      protected abstract void RunThread();
      protected abstract void Cleanup();

      protected void InternalSetup(int dataFlow, int flags, int bufferSize = 0)
      {
         lock (mutex)
         {
            int hr; object obj; Guid guid;
            if (endpoint == null)
            {
               if (enumerator == null)
               {
                  enumerator = Activator.CreateInstance(typeof(MMDeviceEnumerator)) as IMMDeviceEnumerator;
                  if (enumerator == null) { throw new NotSupportedException(); }
               }
               hr = enumerator.GetDefaultAudioEndpoint((DataFlowEnum)dataFlow, RoleEnum.Multimedia, out endpoint);
               if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
               if (endpoint == null) { throw new NotSupportedException(); }
            }
            if (audioClient != null) { Marshal.ReleaseComObject(audioClient); audioClient = null; }
            guid = IID_IAudioClient;
            hr = endpoint.Activate(ref guid, ClsCtxEnum.ALL, IntPtr.Zero, out obj);
            if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
            audioClient = obj as IAudioClient;
            if (audioClient == null) { throw new NotSupportedException(); }
            IntPtr mixFormatPtr = IntPtr.Zero;
            try
            {
               hr = audioClient.GetMixFormat(out mixFormatPtr);
               if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
               WaveFormat outputFormat = (WaveFormat)Marshal.PtrToStructure(mixFormatPtr, typeof(WaveFormat));
               outputIsFloat = false;
               if (outputFormat.ExtraSize >= WaveFormatEx.WaveFormatExExtraSize)
               {
                  WaveFormatEx ex = (WaveFormatEx)Marshal.PtrToStructure(mixFormatPtr, typeof(WaveFormatEx));
                  if (ex.SubFormat == IeeeFloat) { outputIsFloat = true; }
               }
               sampleRate = outputFormat.SampleRate;
               channelCount = outputFormat.Channels;
               bitsPerSample = outputFormat.BitsPerSample;
               if (bufferSize <= 0) { bufferSize = 8192; }
               latencyMs = (int)(bufferSize * 500 / sampleRate);
               if (latencyMs <= 0) { latencyMs = 1; }
               hr = audioClient.Initialize(AudioClientShareModeEnum.Shared, (AudioClientStreamFlagsEnum)flags,
                  latencyMs * 40000, 0, mixFormatPtr, Guid.Empty);
               if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
            }
            finally
            {
               if (mixFormatPtr != IntPtr.Zero) { Marshal.FreeCoTaskMem(mixFormatPtr); }
            }
            hr = audioClient.GetBufferSize(out outputBufferSize);
            if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
            this.bufferSize = bufferSize;
            buffers = new float[channelCount][];
            for (int i = 0; i < buffers.Length; i++) { buffers[i] = new float[bufferSize]; }
            isInited = true;
         }
      }

      protected int GetCurrentPadding()
      {
         int padding;
         audioClient.GetCurrentPadding(out padding);
         return padding;
      }

      protected T GetService<T>(Guid guid) where T : class
      {
         object obj; int hr = audioClient.GetService(guid, out obj);
         if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
         T service = obj as T;
         if (service == null) { throw new NotSupportedException(); }
         return service;
      }

      public void Start()
      {
         lock (mutex)
         {
            if (!isInited) { throw new InvalidOperationException("Not initialized - call Setup first"); }
            if (isStarted) { return; }
            int hr = audioClient.Start();
            if (hr != 0) { Marshal.ThrowExceptionForHR(hr); }
            samplesRead = 0;
            samplesWritten = 0;
            thread = new Thread(RunThread);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();
            isStarted = true;
         }
      }

      public void Stop()
      {
         Thread thread;
         lock (mutex)
         {
            isStarted = false;
            thread = this.thread;
            this.thread = null;
            audioClient?.Stop();
         }
         if (thread != null)
         {
            if (thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
               thread.Join();
            }
         }
      }

      public void Dispose()
      {
         try { Stop(); } catch { }
         lock (mutex)
         {
            Cleanup();
            if (audioClient != null) { Marshal.ReleaseComObject(audioClient); audioClient = null; }
            if (endpoint != null) { Marshal.ReleaseComObject(endpoint); endpoint = null; }
            if (enumerator != null) { Marshal.ReleaseComObject(enumerator); enumerator = null; }
            isInited = false;
            buffers = null;
            thread = null;
            i8Buf = null; i16Buf = null;
            i32Buf = null; f32Buf = null;
            f64Buf = null;
         }
      }
   }
}
