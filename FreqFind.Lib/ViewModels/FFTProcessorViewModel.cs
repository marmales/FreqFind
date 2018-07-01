using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace FreqFind.Lib.ViewModels
{
    public class FFTProcessorViewModel : BaseDialogViewModel, IAudioProcessor
    {
        public IProcessorModel<float> Model { get; set; }
        public event EventHandler<FFTEventArgs> OnFFTCalculated;

        public FFTProcessorViewModel(IProcessorModel<float> model)
        {
            this.Model = model;

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
            var outputData = globalResult.GetFrequencyValues().ToList();

            var rangeList = outputData.PreparePeaks(Model);

            var peaks = GetLocalPeaks(input, rangeList, chirp);

            if (rangeList.Count() == 0)
                return GetLocalPeaks(input, new List<LocalRange>() { chirp.RangeInit(outputData.IndexOf(outputData.Max()), 3) }, chirp);

            return peaks;
        }
        private IEnumerable<double> GetLocalPeaks(float[] input, IEnumerable<LocalRange> models, ChirpModel mainModel)
        {
            var peaks = new List<double>();
            Parallel.ForEach<LocalRange>(models, x =>
            {
                var complexResult = ChirpFFT(input, mainModel, x);
                peaks.Add(FrequencyHelpers.GetZoomedFrequency(complexResult.GetPeakIndex(), x.LeftThreshold, x.RightThreshold, x.ZoomOptions.TargetNumberOfSamples));
            });
            return peaks;
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
        }

        public LocalRange CurrentRange { get; set; }
    }
}
