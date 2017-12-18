using FreqFind.Common;
using System;
using System.Numerics;

namespace FreqFind.Lib.Models
{
    public class TickSound : Observable
    {
        public DateTime Time { get; set; } //or describe in int as time elapsed from point 0
        public double[] RawData { get; set; }

        //public Complex[] ProcessedData { get; set; }
        public int Frequency { get; set; }
    }
}
