using FreqFind.Lib.Models;
using FreqFind.Lib.ViewModels;
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


    public static class AudioHelpers
    {
        public static void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer)
        {
            var bufferLength = buffer.Length;
            var targetLength = bufferLength / 2;
            if (data.Length != targetLength)
                data = new short[targetLength];
            Buffer.BlockCopy(buffer, 0, data, 0, bufferLength);
        }
        public static  AudioSettings DefaultSettings()
        {
            return new AudioSettings()
            {
                SampleRate = 44100,
                SelectedDevice = SettingsViewModel.GetDevices().FirstOrDefault()
            };
        }
    }
}
