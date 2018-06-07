using FreqFind.Common;
using System;

namespace FreqFind.Lib.Models
{
    public class MagnifierModel : Observable
    {
        public MagnifierModel()
        {
        }
        private double baseFreq;
        public double BaseFrequency
        {
            get { return baseFreq; }
            set
            {
                if (baseFreq == value) return;
                baseFreq = value;
                OnPropertyChanged(nameof(BaseFrequency));
            }
        }

        private int numberOfSamples;
        public int TargetNumberOfSamples
        {
            get { return numberOfSamples; }
            set
            {
                if (numberOfSamples == value) return;
                numberOfSamples = value;
                OnPropertyChanged(nameof(TargetNumberOfSamples));
            }
        }
    }
}
