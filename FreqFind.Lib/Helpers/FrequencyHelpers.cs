using FreqFind.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Lib.Helpers
{
    public static class FrequencyHelpers
    {
        public static double GetValue<T>(IList<T> frequencies, int index) 
        {
            return (double)index / frequencies.Count * SoundCard.SampleRate / 2000.0; // in KHz
        }
        public static int GetIndex<T>(IList<T> frequencies, T value)
        {
            return frequencies.Contains(value) ? frequencies.IndexOf(value) : -1;
        }
    }
}
