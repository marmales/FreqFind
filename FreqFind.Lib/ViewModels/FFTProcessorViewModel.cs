using Accord.Math;
using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using System;
using System.Numerics;

namespace FreqFind.Lib.ViewModels
{
    public class FFTProcessorViewModel : BaseDialogViewModel, IAudioProcessor
    {
        public ISampleAggregator<float> SampleAggregator { get; set; }
        public double[] TransformedData // hide if tone will be implemented
        {
            get { return transformedData; }
            set
            {
                if (transformedData == value) return;
                transformedData = value;
                OnPropertyChanged(nameof(TransformedData));
            }
        }
        double[] transformedData = new double[1];

        public FFTProcessorViewModel(int sampleLength)
        {
            this.SampleAggregator = new SampleAggregator(sampleLength);
            SampleAggregator.OnSamplesAccumulated = Process;
        }

        public void Process(float[] input)
        {
            var result = InternalFFT(input);

            FFTHelpers.GetFrequencyValues(ref transformedData, result);
            OnPropertyChanged(nameof(TransformedData));

            OnFFTCalculated.Invoke(null, new FFTEventArgs(transformedData));
        }

        private static Complex[] InternalFFT(float[] data)
        {
            var fftComplex = new Complex[data.Length]; // the FFT function requires complex format
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);// make it complex format (imaginary = 0)
            }
            //FourierTransform.FFT(fftComplex, FourierTransform.Direction.Backward);
            FFTProcessor.ChirpTransform(fftComplex, TempGlobalSettings.LeftFreq, TempGlobalSettings.RightFreq, TempGlobalSettings.FreqSamplesCount, 44100);
            return fftComplex;
        }

        public void Cleanup()
        {
            SampleAggregator.OnSamplesAccumulated = null;
            TransformedData = new double[1];
        }

        public event EventHandler<FFTEventArgs> OnFFTCalculated;
    }


    public class SampleAggregator : ISampleAggregator<float>
    {
        private int targetLength;
        private int index;
        private float[] aggregatedData;

        private static object locker = new object();
        public SampleAggregator(int length)
        {
            targetLength = length;
            index = 0;
            aggregatedData = new float[targetLength];
        }
        public Action<float[]> OnSamplesAccumulated { get; set; }

        public void AddSample(float data)
        {
            aggregatedData[index++] = data;
            if (index >= targetLength)
            {
                index = 0;
                var result = OnSamplesAccumulated.BeginInvoke(aggregatedData, null, locker);
            }
        }
    }
}
