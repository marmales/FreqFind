using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Common.Interfaces
{
    public interface ISoundTone
    {
        Tone Tone { get; }
        void Set(double[] frequencies);

        double HighestFrequency { get; }
        void SetHighestFreq(double[] frequencies);
    }


    public enum Tone
    {
        Ab,
        A,
        Bb,
        B,
        C,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G
    }
}
