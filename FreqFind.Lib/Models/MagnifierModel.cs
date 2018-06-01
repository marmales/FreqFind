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


        private double leftFrequency;
        public double LeftThreshold // Array index
        {
            get { return leftFrequency; }
            set
            {
                if (leftFrequency == value) return;
                leftFrequency = value;
                OnPropertyChanged(nameof(LeftThreshold));
            }
        }

        private double rightFrequency;
        public double RightThreshold //Array index
        {
            get { return rightFrequency; }
            set
            {
                if (rightFrequency == value) return;
                rightFrequency = value;
                OnPropertyChanged(nameof(RightThreshold));
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
