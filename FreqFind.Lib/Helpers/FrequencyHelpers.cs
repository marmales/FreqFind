using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public static class FrequencyHelpers
    {
        public static double LoudestFrequency<T>(this IList<T> frequencies, int sampleRate)
        {
            int highestValueIndex = GetIndex(frequencies, frequencies.Max());
            return GetValue(frequencies.Count, highestValueIndex, sampleRate);
        }
        public static double GetValue(int samplesCount, int index, int sampleRate)
        {
            return (double)index / samplesCount * sampleRate; // in Hz
        }
        public static int GetIndex<T>(IList<T> frequencies, T value)
        {
            return frequencies.Contains(value) ? frequencies.IndexOf(value) : -1;
        }
    }
}
