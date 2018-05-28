using Accord.Math;
using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Numerics;

namespace FreqFind.Lib.ViewModels
{
    public class FFTProcessorViewModel : BaseDialogViewModel, IAudioProcessor
    {
        public ISampleAggregator<float> SampleAggregator { get; set; }
        public IProcessorModel<float> Model { get; set; }
        public event EventHandler<FFTEventArgs> OnFFTCalculated;

        public FFTProcessorViewModel(IProcessorModel<float> model)
        {
            if (this.SampleAggregator != null)
                SampleAggregator.OnSamplesAccumulated -= Process;

            this.Model = model;
            this.SampleAggregator = GetAggregator(model);
            SampleAggregator.OnSamplesAccumulated += Process;
        }

        private ISampleAggregator<float> GetAggregator(IProcessorModel<float> model)
        {
            var chirp = model as ChirpModel;
            if (chirp != null)
                return new ZoomedAggregator(chirp.ZoomOptions);

            var simple = model as SimpleFFTModel;
            if (simple != null)
                return new SampleAggregator(simple.SamplesCount);

            return null;
        }

        public void Process(float[] input)
        {
            var chirp = Model as ChirpModel;
            if (chirp == null)
                return;

            var globalResult = InternalFFT(input);
            chirp.GetProcessRange(globalResult);

            var globalLeftThreshold = 80;
            var globalRightThreshold = 2400;//look at tone implementation

            OnPropertyChanged(nameof(TransformedData));

            OnFFTCalculated.Invoke(null, new FFTEventArgs(transformedData));
        }

        private static Complex[] ChirpFFT(float[] data, ChirpModel model)
        {
            if (model == null) return null;

            var fftComplex = new Complex[data.Length]; // the FFT function requires complex format
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);// make it complex format (imaginary = 0)
            }

            FFTProcessor.ChirpTransform(fftComplex, model.ZoomOptions, model.SampleRate);
            return fftComplex;
        }

        private static Complex[] InternalFFT(float[] data)
        {
            var fftComplex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);
            }
            FFTProcessor.Transform(fftComplex, false);
            //FourierTransform.FFT(fftComplex, FourierTransform.Direction.Forward);
            return fftComplex;
        }

        public void Cleanup()
        {
            SampleAggregator.OnSamplesAccumulated = null;
            TransformedData = new double[1];
        }


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

    }
}
