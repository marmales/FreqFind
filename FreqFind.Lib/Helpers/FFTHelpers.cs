using Accord.Math;
using FreqFind.Common;
using FreqFind.Common.Extensions;
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
        //public static IProcessorModel<float> GetDefaultFFTOptions(int samplesCount, int sampleRate)
        //{
        //    return new SimpleFFTModel
        //    {
        //        InputSamplesCount = samplesCount,
        //        SampleRate = sampleRate
        //    };
        //}
        //public static IProcessorModel<float> GetZoomDefaultFFTOptions(int samplesCount, int sampleRate)
        //{
        //    return new ChirpModel
        //    {
        //        SampleRate = sampleRate,
        //        InputSamplesCount = samplesCount
        //    };
        //}
        public static LocalRange GetNextFundamental(this LocalRange previousRange)
        {
            return new LocalRange(previousRange.ZoomOptions)
            {
                Peak = previousRange.Peak + previousRange.ZoomOptions.BaseFrequency,
                LeftThreshold = previousRange.LeftThreshold + previousRange.ZoomOptions.BaseFrequency,
                RightThreshold = previousRange.RightThreshold + previousRange.ZoomOptions.BaseFrequency
            };
        }
        public static LocalRange GetGlobalPeak(this IProcessorModel<float> model, Complex[] data)
        {
            var loudestIndex = data.Take(data.Length / 2).GetGlobalPeakIndex();

            return model.RangeInit(loudestIndex, 10);
        }
        public static MagnifierModel GetZoomOptions(this IProcessorModel<float> fftModel, double leftThreshold, double rightThreshold, double globalPeak)
        {
            var samples = (int)((leftThreshold - rightThreshold) / ChirpModel.FREQUENCY_DIFFERENCE); // number of samples where length beetwen each sample is equal 0.1Hz

            return new MagnifierModel()
            {
                BaseFrequency = globalPeak,//FrequencyHelpers.GetFrequency(fftModel.InputSamplesCount, globalPeak, fftModel.SampleRate);
                TargetNumberOfSamples = Math.Max(samples, ChirpModel.MIN_CHIRP_SAMPLES),
                FrequencyDistance = rightThreshold - leftThreshold
            };
        }
        private static LocalRange RangeInit(this IProcessorModel<float> model, int peakIndex, int threshold)
        {
            var leftIndex = peakIndex - threshold;
            var rightIndex = peakIndex + threshold;
            var leftThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount / 2, leftIndex, model.SampleRate);
            var rightThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount / 2, rightIndex, model.SampleRate);
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
        public static int GetGlobalPeakIndex(this IEnumerable<Complex> fftData)
        {
            int targetIndex = 0;
            double maxValue = 20 * Math.Log10(fftData.ElementAt(0).Magnitude);
            double currentValue = 0;
            for (int i = 1; i < fftData.Count(); i++)
            {
                currentValue = 20 * Math.Log10(fftData.ElementAt(i).Magnitude);
                if (currentValue > maxValue)
                {
                    targetIndex = i;
                    maxValue = currentValue;
                }
                    
            }

            return targetIndex;
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
