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
            this.SampleAggregator = new SampleAggregator(model.InputSamplesCount)
            {
                OnSamplesAccumulated = Process
            };
        }

        public void Process(float[] input)
        {
            var chirp = Model as ChirpModel;
            if (chirp == null)
                return;


            var globalResult = InternalFFT(input);
            var outputData = globalResult.GetFrequencyValues().ToList();//.ToListAsync();
#if DEBUG
            //TransformedData = outputData;
#endif

            var rangeList = new List<LocalRange> { chirp.GetGlobalPeak(outputData) };
            while (rangeList.Count < 5)
                rangeList.Add(rangeList.Last().GetNextFundamental());

            var peaks = GetLocalPeaks(input, rangeList, chirp);

            OnFFTCalculated.Invoke(null, new FFTEventArgs() { LocalPeaks = peaks });
        }
        private static IEnumerable<double> GetLocalPeaks(float[] input, IEnumerable<LocalRange> models, ChirpModel mainModel)
        {
            foreach (var range in models)
            {
                var complexResult = ChirpFFT(input, mainModel, range);
                yield return FrequencyHelpers.GetZoomedFrequency(complexResult.GetPeakIndex(), range.LeftThreshold, range.RightThreshold, range.ZoomOptions.TargetNumberOfSamples);
            }
        }
        private static IEnumerable<Complex> ChirpFFT(float[] data, ChirpModel model, LocalRange range)
        {
            if (model == null) return null;

            var fftComplex = new Complex[data.Length]; // the FFT function requires complex format
            for (int i = 0; i < fftComplex.Length; i++)
            {
                fftComplex[i] = new Complex(data[i], 0.0);// make it complex format (imaginary = 0)
            }

            return FFTProcessor.ChirpTransform(fftComplex, range, model.SampleRate);
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
        public void Cleanup()
        {
            SampleAggregator.OnSamplesAccumulated = null;
            TransformedData = new List<double>();
        }

#if DEBUG
        private List<double> transformedData = new List<double>();
        public List<double> TransformedData
        {
            get { return transformedData; }
            set
            {
                if (transformedData == value) return;
                transformedData = value;
                OnPropertyChanged(nameof(TransformedData));
            }
        }

#endif
    }
}
