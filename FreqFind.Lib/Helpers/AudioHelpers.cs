using FreqFind.Lib.Models;
using NAudio.Wave;
using System;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public static class DeviceMapper
    {
        public static Device Map(int deviceIndex, WaveInCapabilities device)
        {
            return new Device(deviceIndex, device.Channels, device.ProductName);
        }
    }


    public interface IAudioHelpers
    {
        void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer);
    }
    public class AudioHelpers_16bitPCM_Mono : IAudioHelpers
    {

        public void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer)
        {
            var bufferLength = buffer.Length;
            var targetLength = bufferLength / 2;
            if (data.Length != targetLength)
                data = new short[targetLength];
            Buffer.BlockCopy(buffer, 0, data, 0, bufferLength);
            if (data.Any(x => x < 0))
            {

            }
        }
    }
    public class AudioHelpers_16bitPCM_Stereo : IAudioHelpers
    {
        public void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer)
        {
            var bufferLength = buffer.Length;
            var targetLength = bufferLength / 2;
            if (data.Length != targetLength)
                data = new short[targetLength];
            Buffer.BlockCopy(buffer, 0, data, 0, bufferLength);
        }
    }
}
