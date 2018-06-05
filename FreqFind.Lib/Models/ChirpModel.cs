using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace FreqFind.Lib.Models
{
    public class ChirpModel : FFTModel
    {
        public const double FREQUENCY_DIFFERENCE = 0.1; // 0.1Hz
        public const int MAX_CHIRP_SAMPLES = 2048;
        public const int MIN_CHIRP_SAMPLES = 128;
        public const int DECYBELS_RANGE_DIFFERENCE = 20; //20db
        
        public ChirpModel()
        {
        }



        //void init(int globalPeak, int threshold)//index
        //{
        //    var leftIndex = globalPeak - threshold;
        //    var rightIndex = globalPeak + threshold;
        //    var leftThreshold = FrequencyHelpers.GetFrequency(InputSamplesCount / 2, leftIndex, SampleRate);
        //    var rightThreshold = FrequencyHelpers.GetFrequency(InputSamplesCount / 2, rightIndex, SampleRate);

        //    var samples = (int)((leftThreshold - rightThreshold) / FREQUENCY_DIFFERENCE); // number of samples where length beetwen each sample is equal 0.1Hz

        //    zoomOptions.BaseFrequency = FrequencyHelpers.GetFrequency(InputSamplesCount, globalPeak, SampleRate);
        //    zoomOptions.TargetNumberOfSamples = Math.Max(samples, MIN_CHIRP_SAMPLES);
        //    zoomOptions.FrequencyDistance = rightThreshold - leftThreshold;

        //    Range.Add(new LocalRange()
        //    {
        //        LeftThreshold = leftThreshold,
        //        RightThreshold = rightThreshold,
        //        Peak = zoomOptions.BaseFrequency
        //    });
        //}

        //public void AddLocal(double previousFrequency)
        //{
        //    var peak = previousFrequency + zoomOptions.BaseFrequency;
        //    Range.Add(new LocalRange()
        //    {
        //        LeftThreshold = peak - zoomOptions.FrequencyDistance / 2,
        //        RightThreshold = peak + zoomOptions.FrequencyDistance / 2,
        //        Peak = peak
        //    });
        //}
        //public int GetSamplesZoomCount()
        //{
        //    return zoomOptions.TargetNumberOfSamples;
        //}
    }
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
