using System.Collections.Generic;

namespace FreqFind.Lib.Helpers
{
    public static class FrequencyHelpers
    {
        public static double GetValue<T>(IList<T> frequencies, int index, int sampleRate = 44100)
        {
            return (double)index / frequencies.Count * sampleRate / 2000.0; // in KHz
        }
        public static int GetIndex<T>(IList<T> frequencies, T value)
        {
            return frequencies.Contains(value) ? frequencies.IndexOf(value) : -1;
        }
    }
}
