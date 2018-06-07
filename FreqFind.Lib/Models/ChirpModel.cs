using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace FreqFind.Lib.Models
{
    public class LocalRange : Observable
    {
        public MagnifierModel ZoomOptions { get; }
        public LocalRange(MagnifierModel model)
        {
            ZoomOptions = model;
        }
        private double peak;
        public double Peak
        {
            get { return peak; }
            set
            {
                if (peak == value) return;
                peak = value;
                OnPropertyChanged(nameof(Peak));
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
    }
    public class ChirpModel : FFTModel
    {
        public const double FREQUENCY_DIFFERENCE = 0.1; // 0.1Hz
        public const int MAX_CHIRP_SAMPLES = 2048;
        public const int MIN_CHIRP_SAMPLES = 128;
        public const int DECYBELS_RANGE_DIFFERENCE = 20; //20db

        public ChirpModel()
        {
        }
    }
    public class SimpleFFTModel : FFTModel
    {
    }

    public abstract class FFTModel : Observable, IProcessorModel<float>
    {
        private int samplesCount;
        public int InputSamplesCount
        {
            get { return samplesCount; }
            set
            {
                if (samplesCount == value) return;
                samplesCount = value;
                OnPropertyChanged(nameof(InputSamplesCount));
            }
        }
        private int sampleRate;
        public int SampleRate
        {
            get { return sampleRate; }
            set
            {
                if (sampleRate == value) return;
                sampleRate = value;
                OnPropertyChanged(nameof(SampleRate));
            }
        }
    }
}
