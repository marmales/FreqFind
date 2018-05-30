using System.Collections.Generic;

namespace FreqFind.Common.Interfaces
{
    public interface ISoundNote
    {
        INote GetNote(IEnumerable<double> localPeaks);
        INote GetNote(double[] frequencies, int sampleRate);
    }

    public interface INote
    {
        Tone Tone { get; set; }
        int Base { get; set; }
    }

    public enum Tone
    {
        C,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G,
        Ab,
        A,
        Bb,
        B
    }
}
