using System;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioProcessor : ICleanup
    {
        ISampleAggregator<float> SampleAggregator { get; set; }
        void Process(float[] rawData);
        event EventHandler<FFTEventArgs> OnFFTCalculated;
    }

    public interface ISampleAggregator<T> where T : struct
    {
        void AddSample(T data);
        Action<T[]> OnSamplesAccumulated { get; set; }
    }

    public class FFTEventArgs : EventArgs
    {
        public FFTEventArgs(double[] result)
        {
            Result = result;
        }
        public double[] Result { get; set; }
    }
}
