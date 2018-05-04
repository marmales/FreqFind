using FreqFind.Lib.Helpers;
using FreqFind.Common;
using NAudio.Wave;
using System.Timers;
using System;
using FreqFind.Common.Extensions;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Models;
using System.Threading;

namespace FreqFind.Lib.ViewModels
{
    public class MainViewModel : BaseDialogViewModel
    {
        private IAudioReader<float> reader;
        private IAudioProcessor processor;
        private IAudioHelpers audioHelper;
        public MainViewModel()
        {
            StartAudio();
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

        private void StartAudio()
        {
            processor = new FFTProcessorViewModel(SoundCard.BufferSize);
            processor.OnFFTCalculated += AssignCalculatedData;

            reader = new AsioReaderViewModel();
            reader.Setup(null);
            reader.OnDataReceived = PrepareInputForFFT;

            reader.Start();
        }

        private short[] receivedData = new short[1];
        private void PrepareInputForFFT(float[] data)
        {
            processor.Process(data);
        }

        public void AssignCalculatedData(object sender, FFTEventArgs e)
        {
            FFTHelpers.GetFrequencyValues(ref transformedData, e.Result);
            OnPropertyChanged(nameof(TransformedData));
        }

        private void SetAudioHelper(int channels)
        {
            switch (channels)
            {
                case 1:
                    audioHelper = new AudioHelpers_16bitPCM_Mono();
                    break;
                case 2:
                    audioHelper = new AudioHelpers_16bitPCM_Stereo();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported number of channels.\nOnly 1 or 2 are valid.");
            }
        }
    }
}
