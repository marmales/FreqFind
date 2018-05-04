
using FreqFind.Common.Extensions;
using NAudio.Wave;
using System;

namespace FreqFind.Lib.Helpers
{
    public interface IAudioHelpers
    {
        void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer);
    }
    public class AudioHelpers_16bitPCM_Mono : IAudioHelpers
    {

        public void ByteArrayTo16BITInputFormat(ref short[] data, byte[] buffer)
        {
            throw new NotImplementedException();
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
        private void Insert(ref float[] targetData, short[] tmpData) // short to float representation
        {
            var channels = 2;
            float tmpValue;
            var maxValue = 32767; var minValue = -32768;
            var i = 0;
            for (; i < tmpData.Length; i += channels)
            {
                tmpValue = 0.5f * (tmpData[i] + tmpData[i + 1]);
                if (tmpValue > maxValue)
                    targetData[i / channels] = maxValue;
                else if (tmpValue < minValue)
                    targetData[i / channels] = minValue;
                else
                    targetData[i / channels] = tmpValue / 32768f;

            }
        }     
    }
}
