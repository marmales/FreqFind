using FreqFind.Common;

namespace FreqFind.Lib.Models
{
    public class MagnifierModel : Observable
    {
        private int leftFreq;
        public int LeftThreshold
        {
            get { return leftFreq; }
            set
            {
                if (leftFreq == value) return;
                leftFreq = value;
                OnPropertyChanged(nameof(LeftThreshold));
            }
        }

        private int rightFrequency;
        public int RightThreshold
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
        public int NumberOfSamples
        {
            get { return numberOfSamples; }
            set
            {
                if (numberOfSamples == value) return;
                numberOfSamples = value;
                OnPropertyChanged(nameof(NumberOfSamples));
            }
        }
    }
}
