using FreqFind.Common;
using FreqFind.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Lib.Models
{
    public class ChirpModel : FFTModel
    {
        public MagnifierModel ZoomOptions { get; set; }
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
