using Accord.Math;
using FreqFind.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FreqFind.Lib.ViewModels
{
    public class FFTProcessorViewModel : IAudioProcessor
    {
        public ISampleAggregator<float> SampleAggregator { get; set; }
        public FFTProcessorViewModel(int sampleLength)
        {
            this.SampleAggregator = new SampleAggregator(sampleLength);
            SampleAggregator.OnSamplesAccumulated = Process;
        }

        public void Process(float[] input)
        {
            var result = InternalFFT(input);
            OnFFTCalculated.Invoke(null, new FFTEventArgs(result));
        }

        private static Complex[] InternalFFT(float[] data)
        {
            var fftComplex = new Complex[data.Length]; // the FFT function requires complex format
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);// make it complex format (imaginary = 0)
            }
            FourierTransform.FFT(fftComplex, FourierTransform.Direction.Forward);
            //for (int i = 0; i < data.Length / 2; i++)
            //{

            //    var im2 = Math.Pow(fftComplex[i].Imaginary, 2);
            //    var re2 = Math.Pow(fftComplex[i].Real, 2);
            //    fftResult[i] = 20 * Math.Log10(System.Math.Sqrt(im2 + re2));
            //}
            return fftComplex;
        }

        public event EventHandler<FFTEventArgs> OnFFTCalculated;
    }


    public class SampleAggregator : ISampleAggregator<float>
    {
        private int targetLength;
        private int index;
        private float[] aggregatedData;
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
                OnSamplesAccumulated.Invoke(aggregatedData);
            }
        }
    }
}
