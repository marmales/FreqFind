using FreqFind.Lib.Models;
using NAudio.Wave;
using System;

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
        AudioSettings DefaultSettings();
    }
    public class AudioHelpers_16bitPCM : IAudioHelpers
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
