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
                InputSamplesCount = samplesCount,
                SampleRate = sampleRate
            };
        }
        public static LocalRange GetNextFundamental(this LocalRange previousRange)
        {
            return new LocalRange(previousRange.ZoomOptions)//Zoom options will be the same for the all local peaks
            {
                Peak = previousRange.Peak + previousRange.ZoomOptions.BaseFrequency,
                LeftThreshold = previousRange.LeftThreshold + previousRange.ZoomOptions.BaseFrequency,
                RightThreshold = previousRange.RightThreshold + previousRange.ZoomOptions.BaseFrequency
            };
        }
        public static LocalRange GetGlobalPeak(this IProcessorModel<float> model, List<double> data)
        {
            var loudestIndex = data.Take(data.Count / 2).GetPeakIndex();

            return model.RangeInit(loudestIndex, 10);
        }
        public static MagnifierModel GetZoomOptions(this IProcessorModel<float> fftModel, double leftThreshold, double rightThreshold, double globalPeak)
        {
            var samples = (int)((rightThreshold - leftThreshold) / ChirpModel.FREQUENCY_DIFFERENCE); // number of samples where length beetwen each sample is equal 0.1Hz

            return new MagnifierModel()
            {
                BaseFrequency = globalPeak,//FrequencyHelpers.GetFrequency(fftModel.InputSamplesCount, globalPeak, fftModel.SampleRate);
                TargetNumberOfSamples = Math.Max(samples, ChirpModel.MIN_CHIRP_SAMPLES)
            };
        }
        private static LocalRange RangeInit(this IProcessorModel<float> model, int peakIndex, int threshold)
        {
            var leftIndex = peakIndex - threshold;
            var rightIndex = peakIndex + threshold;
            var leftThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount, leftIndex, model.SampleRate);
            var rightThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount, rightIndex, model.SampleRate);
            var globalPeak = FrequencyHelpers.GetFrequency(model.InputSamplesCount, peakIndex, model.SampleRate);

            return new LocalRange(model.GetZoomOptions(leftThreshold, rightThreshold, globalPeak))
            {
                LeftThreshold = leftThreshold,
                RightThreshold = rightThreshold,
                Peak = globalPeak
            };
        }
    }
    public static class FFTHelpers
    {
        public static IEnumerable<double> GetFrequencyValues(this Complex[] fftData)
        {
            foreach (var item in fftData.Take(fftData.Length / 2))
                yield return 20 * Math.Log10(item.Magnitude);
        }
        public static double[] GetFrequencyValues(this Complex[] fftData, ref double[] result)
        {
            var targetArrayLength = fftData.Length / 2;
            if (targetArrayLength != result.Length)
                result = new double[targetArrayLength];

            for (int i = 0; i < targetArrayLength; i++)
                result[i] = 20 * Math.Log10(fftData[i].Magnitude);

            return result;
        }
        public static int GetPeakIndex(this IEnumerable<double> fftData)
        {
            var maxValue = fftData.ElementAt(0);
            var targetIndex = 0;
            int i = 0;
            foreach (var item in fftData)
            {
                if (item > maxValue)
                {
                    maxValue = item;
                    targetIndex = i;
                }
                i++;
            }
            return targetIndex;
        }
        public static int GetPeakIndex(this IEnumerable<Complex> fftData)
        {
            return fftData.Select(x => 20 * Math.Log10(x.Magnitude)).GetPeakIndex();
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
