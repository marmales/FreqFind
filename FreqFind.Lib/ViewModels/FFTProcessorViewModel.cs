using Accord.Math;
using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Process(float[] input)
        {
            var chirp = Model as ChirpModel;
            if (chirp == null)
                return;

            var globalResult = InternalFFT(input);
            chirp.GetMainProcessRange(globalResult);

            var peaks = GetPeaks(GetAllModels(chirp, 2), input);
            OnFFTCalculated.Invoke(null, new FFTEventArgs() { LocalPeaks = peaks });
        }
        private static IEnumerable<ChirpModel> GetAllModels(ChirpModel mainModel, int count)
        {
            var left = mainModel;
            var right = mainModel;
            var models = new List<ChirpModel>() { mainModel };
            for (int i = 0; i < count; i++)
            {
                left = GetLocalRange(left, false);
                right = GetLocalRange(right, true);
                yield return left;
                yield return right;
            }
        }
        private static IEnumerable<double> GetPeaks(IEnumerable<ChirpModel> models, float[] input)
        {
            var sortedModels = models.OrderBy(MiddleOfTheZoom);

            foreach (var model in sortedModels)
            {
                var localProcessing = new double[model.ZoomOptions.TargetNumberOfSamples];
                yield return ChirpFFT(input, model).GetFrequencyValues(ref localProcessing).LoudestFrequency(model.SampleRate);
            }
        }
        private static double MiddleOfTheZoom(ChirpModel model)
        {
            var left = model.ZoomOptions.LeftThreshold;
            var right = model.ZoomOptions.RightThreshold;
            return left + (right - left) / 2;
        }
        private static ChirpModel GetLocalRange(ChirpModel model, bool rightDirection)
        {
            var multiplier = rightDirection ? Math.Pow(2, 1) : Math.Pow(2, -1);
            var newLeftThreshold = model.ZoomOptions.LeftThreshold * multiplier;
            var newRightThreshold = model.ZoomOptions.RightThreshold * multiplier;

            return new ChirpModel
            {
                SampleRate = model.SampleRate,
                ZoomOptions = new MagnifierModel(newLeftThreshold, newRightThreshold)
            };
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
            return fftComplex.Take(model.ZoomOptions.TargetNumberOfSamples).ToArray();
        }

        private static Complex[] InternalFFT(float[] data)
        {
            var fftComplex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);
            }
            FFTProcessor.Transform(fftComplex, false);
            return fftComplex;
        }
        private ISampleAggregator<float> GetAggregator(IProcessorModel<float> model)
        {
            return new SampleAggregator(model.InputSamplesCount);
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
