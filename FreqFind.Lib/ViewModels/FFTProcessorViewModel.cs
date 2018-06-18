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
            //if (this.SampleAggregator != null)
            //    SampleAggregator.OnSamplesAccumulated -= Process;

            this.Model = model;
            //this.SampleAggregator = new SampleAggregator(model.InputSamplesCount)
            //{
            //    OnSamplesAccumulated = Process
            //};
        }
        private void HannWindow(ref float[] input)
        {
            var multipier = 0d;
            for (int i = 0; i < input.Length; i++)
            {
                multipier = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (input.Length - 1)));
                input[i] *= (float)multipier;
            }
        }
        public IEnumerable<double> Process(float[] input)
        {
            var chirp = Model as ChirpModel;
            if (chirp == null)
                return null;
            HannWindow(ref input);

            var globalResult = InternalFFT(input);
            var outputData = globalResult.GetFrequencyValues().ToList();//.ToListAsync();
            //TransformedData = outputData;
            var rangeList = outputData.PreparePeaks(Model);

            var peaks = GetLocalPeaks(input, rangeList, chirp);
            //peaks.ForEach(x => Debug.Write(string.Concat(x, "\t")));
            //OnFFTCalculated.Invoke(null, new FFTEventArgs() { LocalPeaks = peaks });
            //OnFFTCalculated.Invoke(null, new FFTEventArgs() { Result = outputData });
            return peaks;
        }
        private IEnumerable<double> GetLocalPeaks(float[] input, IEnumerable<LocalRange> models, ChirpModel mainModel)
        {
            foreach (var range in models)
            {
                var complexResult = ChirpFFT(input, mainModel, range);
                //DisplayLocalPeaks(complexResult.ToArray(), range);
                yield return FrequencyHelpers.GetZoomedFrequency(complexResult.GetPeakIndex(), range.LeftThreshold, range.RightThreshold, range.ZoomOptions.TargetNumberOfSamples);
            }
        }
        void DisplayLocalPeaks(Complex[] output, LocalRange range)
        {
            CurrentRange = range;
            TransformedData = output.GetFrequencyValues().ToList();
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
            //Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);
            var n = data.Length;
            for (int i = 0; i < n; i++)  // Scaling (because this FFT implementation omits it)
                fftComplex[i] /= n;

            return fftComplex;
        }
        public void Cleanup()
        {
            TransformedData = new List<double>();
        }

        public LocalRange CurrentRange { get; set; }

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
    }
}
