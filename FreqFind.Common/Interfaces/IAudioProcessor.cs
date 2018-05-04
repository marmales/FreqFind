using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioProcessor
    {
        ISampleAggregator<float> SampleAggregator { get; set; }
        //double[] Process(float[] rawData);
        event EventHandler<FFTEventArgs> OnFFTCalculated;
    }

    public interface ISampleAggregator<T> where T : struct
    {
        void AddSample(T data);
        Action<T[]> OnSamplesAccumulated { get; set; }
    }

    public class FFTEventArgs : EventArgs
    {
        public FFTEventArgs(Complex[] result)
        {
            Result = result;
        }
        public Complex[] Result { get; set; }
    }
}
