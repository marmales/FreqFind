using Accord.Math;
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
        public static IProcessorModel<float> GetDefaultFFTOptions(int samplesCount, int sampleRate)
        {
            return new SimpleFFTModel
            {
                InputSamplesCount = samplesCount,
                SampleRate = sampleRate
            };
        }
        public static IProcessorModel<float> GetZoomDefaultFFTOptions(int samplesCount, int sampleRate)
        {
            return new ChirpModel
            {
                SampleRate = sampleRate,
                InputSamplesCount = samplesCount,
                ZoomOptions = new MagnifierModel(50, 3000)
            };
        }

        public static void GetMainProcessRange(this IProcessorModel<float> model, Complex[] data)
        {
            var chirpModel = model as ChirpModel;
            if (chirpModel == null)
                return;

            var outputData = new double[data.Length];

            data.GetFrequencyValues(ref outputData);
            var highestValueIndex = FrequencyHelpers.GetIndex(outputData, outputData.Max());
            var highestValue = outputData.LoudestFrequency(chirpModel.SampleRate);

            var leftThreshold = chirpModel.GetLeftThreshold(highestValue, highestValueIndex, outputData);
            var rightThreshold = chirpModel.GetRightThreshold(highestValue, highestValueIndex, outputData);
            chirpModel.ZoomOptions.Update(leftThreshold, rightThreshold);
        }
        static int GetLeftThreshold(this IProcessorModel<float> model, double value, int index, double[] outputdata)
        {
            if (index < MagnifierModel.MIN_CHIRP_SAMPLES / 2)
                return MagnifierModel.MIN_CHIRP_SAMPLES / 2;
            var leftMaxRange = MagnifierModel.MAX_CHIRP_SAMPLES / 2;
            int i = index - 1;
            for (; i > index - leftMaxRange && i > 0; i--)
            {
                var currentFrequencyValue = FrequencyHelpers.GetValue(outputdata, i, model.SampleRate);
                if (value - currentFrequencyValue > MagnifierModel.DECYBELS_RANGE_DIFFERENCE)
                    return i;
            }
            return i == 0 ? 0 : -1;
        }
        static int GetRightThreshold(this IProcessorModel<float> model, double value, int index, double[] outputdata)
        {
            if (index < MagnifierModel.MIN_CHIRP_SAMPLES / 2)
                return MagnifierModel.MIN_CHIRP_SAMPLES / 2;
            var rightMaxRange = MagnifierModel.MAX_CHIRP_SAMPLES / 2;
            int i = index - 1;
            for (; i < index + rightMaxRange && i < outputdata.Length; i++)
            {
                var currentFrequencyValue = FrequencyHelpers.GetValue(outputdata, i, model.SampleRate);
                if (value - currentFrequencyValue > MagnifierModel.DECYBELS_RANGE_DIFFERENCE)
                    return i;
            }
            return i == 0 ? 0 : -1;
        }
    }
    public static class FFTHelpers
    {

        public static double[] GetFrequencyValues(this Complex[] fftData, ref double[] result)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];
            for (int i = 0; i < targetArrayLength; i++)
            {
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);//System.Math.Sqrt(im2 + re2)
            }
            return result;
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
