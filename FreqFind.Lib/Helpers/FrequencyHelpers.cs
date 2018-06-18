using System;
using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public static class FrequencyHelpers
    {
        public static double LoudestFrequency<T>(this IList<T> frequencies, int sampleRate)
        {
            int highestValueIndex = frequencies.IndexOf(frequencies.Max());
            return GetFrequency(frequencies.Count, highestValueIndex, sampleRate);
        }
        public static double GetFrequency(int samplesCount, int index, int sampleRate)
        {
            return (double)index / samplesCount * sampleRate / 2; // in Hz CHECK
        }
        public static double GetZoomedFrequency(int index, double leftThreshold, double rightThreshold, int zoomedSamples)
        {
            var diff = (rightThreshold - leftThreshold) / zoomedSamples;
            return leftThreshold + index * diff;
        }
        public static int GetIndex(double frequency, int samplesCount, int sampleRate)
        {
            return (int)Math.Round(frequency * samplesCount / sampleRate); // * 2
        }
    }
}