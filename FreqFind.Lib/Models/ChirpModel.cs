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
    public class ChirpModel : Observable, IProcessorModel<float>
    {
        public MagnifierModel ZoomOptions { get; set; }
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

    public class SimpleFFTModel : Observable, IProcessorModel<float>
    {
        private int samplesCount;
        public int SamplesCount
        {
            get { return samplesCount; }
            set
            {
                if (samplesCount == value) return;
                samplesCount = value;
                OnPropertyChanged(nameof(SamplesCount));
            }
        }

    }
}
