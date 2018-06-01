using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using System;

namespace FreqFind.Lib.Models
{
    public class ChirpModel : FFTModel
    {
        public const double FREQUENCY_DIFFERENCE = 0.1; // 0.1Hz
        public const int MAX_CHIRP_SAMPLES = 2048;
        public const int MIN_CHIRP_SAMPLES = 128;
        public const int DECYBELS_RANGE_DIFFERENCE = 20; //20db
        public MagnifierModel ZoomOptions { get; set; }
        public void Update(int leftthreshold, int rightthreshold)//index
        {                                        
            ZoomOptions.LeftThreshold = FrequencyHelpers.GetValue(InputSamplesCount / 2, leftthreshold, SampleRate);
            ZoomOptions.RightThreshold = FrequencyHelpers.GetValue(InputSamplesCount / 2, rightthreshold, SampleRate);

            var samples = (int)((ZoomOptions.LeftThreshold - ZoomOptions.RightThreshold) / FREQUENCY_DIFFERENCE); // number of samples where length beetwen each sample is equal 0.1Hz
            ZoomOptions.TargetNumberOfSamples = Math.Max(samples, MIN_CHIRP_SAMPLES);
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
