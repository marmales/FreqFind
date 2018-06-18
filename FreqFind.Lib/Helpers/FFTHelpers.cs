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
        public static IProcessorModel<float> GetZoomDefaultFFTOptions(int samplesCount, int sampleRate)
        {
            return new ChirpModel
            {
                InputSamplesCount = samplesCount,
                SampleRate = sampleRate
            };
        }

        static object locker = new object();
        const int MAX_PEAKS = 5;
        const double CONSIDERED_VOLUME = -66;
        const int NEIGHBOURS_COUNT = 5;
        const int VOLUME_DIFFERENCE = 20;
        public static IEnumerable<LocalRange> PreparePeaks(this List<double> data, IProcessorModel<float> model)
        {
            lock (locker)
            {
                var dataLength = data.Count;
                for (int sampleLocation = NEIGHBOURS_COUNT; sampleLocation < data.Count - NEIGHBOURS_COUNT; sampleLocation++)
                {
                    if (data[sampleLocation] < CONSIDERED_VOLUME)
                        continue;

                    var isPeak = true;
                    for (int sampleMove = 0; sampleMove < NEIGHBOURS_COUNT; sampleMove++)
                    {
                        //left
                        var leftNeighbour = sampleLocation - sampleMove;
                        if (data[leftNeighbour] < data[leftNeighbour - 1]) //data is represent with negative number
                        {
                            isPeak = false;
                            break;
                        }
                        //right
                        var rightNeighbour = sampleLocation + sampleMove;
                        if (data[rightNeighbour] < data[rightNeighbour + 1])
                        {
                            isPeak = false;
                            break;
                        }
                    }
                    if (isPeak)
                    {
                        yield return model.RangeInit(sampleLocation, NEIGHBOURS_COUNT);
                    }
                }
            }
        }
        private static LocalRange RangeInit(this IProcessorModel<float> model, int peakIndex, int threshold)
        {
            var leftIndex = peakIndex - threshold;
            var rightIndex = peakIndex + threshold;
            var leftThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount, leftIndex, model.SampleRate) * 2;
            var rightThreshold = FrequencyHelpers.GetFrequency(model.InputSamplesCount, rightIndex, model.SampleRate) * 2;
            var peak = FrequencyHelpers.GetFrequency(model.InputSamplesCount, peakIndex, model.SampleRate) * 2;

            return new LocalRange(GetZoomOptions(leftThreshold, rightThreshold, peak))
            {
                LeftThreshold = leftThreshold,
                RightThreshold = rightThreshold,
                Peak = peak
            };
        }
        private static MagnifierModel GetZoomOptions(double leftThreshold, double rightThreshold, double peak)
        {
            // number of samples where length beetwen each sample is multiple by target frequency difference(here 0.1Hz)
            var samples = (int)((rightThreshold - leftThreshold) / ChirpModel.FREQUENCY_DIFFERENCE);

            return new MagnifierModel()
            {
                BaseFrequency = peak,//FrequencyHelpers.GetFrequency(fftModel.InputSamplesCount, globalPeak, fftModel.SampleRate);
                TargetNumberOfSamples = Math.Max(samples, ChirpModel.MIN_CHIRP_SAMPLES)
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
