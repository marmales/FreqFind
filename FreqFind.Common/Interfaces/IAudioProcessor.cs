using System;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioProcessor : ICleanup
    {
        ISampleAggregator<float> SampleAggregator { get; set; }
        IProcessorModel<float> Model { get; set; }
        void Process(float[] data);
        event EventHandler<FFTEventArgs> OnFFTCalculated;
    }
    public interface IProcessorModel<T>
    {
        int SampleRate { get; set; }
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
