using FreqFind.Common;
using System;

namespace FreqFind.Lib.Models
{
    public class MagnifierModel : Observable
    {
        public const double FREQUENCY_DIFFERENCE = 0.1; // 0.1Hz
        public const int MAX_CHIRP_SAMPLES = 2048;
        public const int MIN_CHIRP_SAMPLES = 128;
        public const int DECYBELS_RANGE_DIFFERENCE = 20; //20db
        public MagnifierModel(int leftThreshold, int rightThreshold)
        {
            Update(leftThreshold, rightThreshold);
        }

        private int leftFrequency;
        public int LeftThreshold
        {
            get { return leftFrequency; }
            set
            {
                if (leftFrequency == value) return;
                leftFrequency = value;
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
        public void Update(int leftThreshold, int rightThreshold)
        {
            LeftThreshold = leftThreshold;
            RightThreshold = rightThreshold;
            Update();
        }
        private void Update()
        {
            if (LeftThreshold == 0 || RightThreshold == 0)
                return;
            var samples = (int)((rightFrequency - leftFrequency) / FREQUENCY_DIFFERENCE); // number of samples where length beetwen each sample is equal 0.1Hz
            NumberOfSamples = Math.Max(samples, MIN_CHIRP_SAMPLES);
        }

    }
}
