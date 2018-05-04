using System;
using Accord.Math;
using System.Numerics;
using FreqFind.Common.Interfaces;
using FreqFind.Common.Extensions;

namespace FreqFind.Lib.Helpers
{
    public static class FFTHelpers
    {
        public static void GetFrequencyValues(ref double[] result, Complex[] fftData)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];
            for (int i = 0; i < targetArrayLength; i++)
            {
                var im2 = Math.Pow(fftData[i].Imaginary, 2);
                var re2 = Math.Pow(fftData[i].Real, 2);
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);//System.Math.Sqrt(im2 + re2)
            }
        }

        public static void SendSamples(ISampleAggregator<float> aggregator, short[] data)
        {
            var channels = SoundCard.Channels;
            float tmpValue;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            var i = 0;
            for (; i < data.Length; i += channels)
            {
                tmpValue = 0.5f * (data[i] + data[i + 1]);
                if (tmpValue > maxIntValue)
                    aggregator.AddSample(maxFloatValue);
                else if (tmpValue < minIntValue)
                    aggregator.AddSample(minFloatValue);
                else
                    aggregator.AddSample(tmpValue / divisior); //[-1;1]

            }
        }
    }
}
