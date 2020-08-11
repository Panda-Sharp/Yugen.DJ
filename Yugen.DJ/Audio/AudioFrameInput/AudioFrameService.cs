using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace Yugen.DJ.Audio.AudioFrameInput
{
    /// <summary>
    /// https://github.com/cjw1115/UniversalWindowsDevDemos/tree/master/AudioFrameInputNodeDemo
    /// </summary>
    public class AudioFrameService
    {
        private Stream fileStream;
        private AudioFrameInputNode audioFrameInputNode;

        private async Task AddAudioFrameInputNodeAsync(StorageFile file, AudioGraph audioGraph, AudioDeviceOutputNode deviceOutputNode)
        {
            var ras = await file.OpenReadAsync();
            fileStream = ras.AsStreamForRead();

            var audioEncodingProperties = new AudioEncodingProperties();
            audioEncodingProperties.BitsPerSample = 32;
            audioEncodingProperties.ChannelCount = 2;
            audioEncodingProperties.SampleRate = 44100;
            audioEncodingProperties.Subtype = MediaEncodingSubtypes.Float;

            audioFrameInputNode = audioGraph.CreateFrameInputNode(audioEncodingProperties);
            audioFrameInputNode.QuantumStarted += FrameInputNode_QuantumStarted;

            audioFrameInputNode.AddOutgoingConnection(deviceOutputNode);
            audioGraph.Start();
        }

        private unsafe void FrameInputNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            var bufferSize = args.RequiredSamples * sizeof(float) * 2;
            var audioFrame = new AudioFrame((uint)bufferSize);

            if (fileStream == null)
                return;

            using (var audioBuffer = audioFrame.LockBuffer(AudioBufferAccessMode.Write))
            {
                using (var bufferReference = audioBuffer.CreateReference())
                {
                    byte* dataInBytes;
                    uint capacityInBytes;
                    float* dataInFloat;

                    // Get the buffer from the AudioFrame
                    ((IMemoryBufferByteAccess)bufferReference).GetBuffer(out dataInBytes, out capacityInBytes);
                    dataInFloat = (float*)dataInBytes;

                    var managedBuffer = new byte[capacityInBytes];

                    var lastLength = fileStream.Length - fileStream.Position;
                    var readLength = (int)(lastLength < capacityInBytes ? lastLength : capacityInBytes);

                    if (readLength <= 0)
                    {
                        fileStream.Close();
                        fileStream = null;
                        return;
                    }

                    fileStream.Read(managedBuffer, 0, readLength);

                    for (var i = 0; i < readLength; i += 8)
                    {
                        dataInBytes[i + 4] = managedBuffer[i + 0];
                        dataInBytes[i + 5] = managedBuffer[i + 1];
                        dataInBytes[i + 6] = managedBuffer[i + 2];
                        dataInBytes[i + 7] = managedBuffer[i + 3];
                        dataInBytes[i + 0] = managedBuffer[i + 4];
                        dataInBytes[i + 1] = managedBuffer[i + 5];
                        dataInBytes[i + 2] = managedBuffer[i + 6];
                        dataInBytes[i + 3] = managedBuffer[i + 7];
                    }
                }
            }

            audioFrameInputNode.AddFrame(audioFrame);
        }
    }
}