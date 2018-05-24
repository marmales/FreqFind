using FreqFind.Common.Interfaces;
using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FreqFind.Lib.Helpers
{
    public static class FFTModelHelpers
    {
        public static IProcessorModel<float> GetDefaultFFTOptions(int samplesCount)
        {
            return new SimpleFFTModel
            {
                SamplesCount = samplesCount
            };
        }
        public static IProcessorModel<float> GetZoomDefaultFFTOptions(int sampleRate)
        {
            return new ChirpModel
            {
                SampleRate = sampleRate,
                ZoomOptions = new MagnifierModel()
                {
                    LeftThreshold = 50,
                    RightThreshold = 3000,
                    NumberOfSamples = 2950
                }
            };
        }
    }
    public static class FFTHelpers
    {
        public static void GetFrequencyValues(ref double[] result, Complex[] fftData)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];
            for (int i = 0; i < targetArrayLength; i++)
            {
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);//System.Math.Sqrt(im2 + re2)
            }
        }

        public static void SendSamples(this ISampleAggregator<float> aggregator, short[] data, IEnumerable<int> channelsVolume)
        {
            var volumeList = channelsVolume.ToList();
            if (volumeList.Count == 0)
                throw new ArgumentException("Channels not found!");

            float tmpValue = 0;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            for (int i = 0; i < data.Length; i += volumeList.Count)
            {
                int channelsIndex = 0;
                tmpValue = data.Skip(i).Take(volumeList.Count).Sum(x => (volumeList[channelsIndex++] / 100f) * x);
                if (tmpValue > maxIntValue)
                    aggregator.AddSample(maxFloatValue);
                else if (tmpValue < minIntValue)
                    aggregator.AddSample(minFloatValue);
                else
                    aggregator.AddSample(tmpValue / divisior); //[-1;1]

            }
        }

        public static int ReverseBits(int val)
        {
            int result = 0;
            for (int i = 0; i < 32; i++, val >>= 1)
                result = (result << 1) | (val & 1);
            return result;
        }
    }
}
