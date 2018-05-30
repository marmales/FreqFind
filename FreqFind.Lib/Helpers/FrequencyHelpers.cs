using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public static class FrequencyHelpers
    {
        public static double LoudestFrequency<T>(this IList<T> frequencies, int sampleRate)
        {
            int highestValueIndex = GetIndex(frequencies, frequencies.Max());
            return GetValue(frequencies, highestValueIndex, sampleRate);
        }
        public static double GetValue<T>(IList<T> frequencies, int index, int sampleRate)
        {
            return (double)index / frequencies.Count * sampleRate / 2000.0; // in KHz
        }
        public static int GetIndex<T>(IList<T> frequencies, T value)
        {
            return frequencies.Contains(value) ? frequencies.IndexOf(value) : -1;
        }
    }
}
