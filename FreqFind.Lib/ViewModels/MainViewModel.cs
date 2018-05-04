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
            //var audioThread = new Thread(() =>
            //{
                StartAudio();
            //});

            //audioThread.Start();
        }

        public float[] TransformedData // hide if tone will be implemented
        {
            get { return transformedData; }
            set
            {
                if (transformedData == value) return;
                transformedData = value;
                OnPropertyChanged(nameof(TransformedData));
            }
        }
        float[] transformedData = new float[1];

        private void StartAudio()
        {
            processor = new FFTProcessorViewModel(SoundCard.BufferSize);
            processor.OnFFTCalculated += AssignCalculatedData;

            reader = new AsioReaderViewModel();
            reader.Setup(new AudioSettings()
            {
                BufferSize = SoundCard.BufferSize,
                Channels = SoundCard.Channels,
                DeviceNumber = 0, //default
                SampleRate = SoundCard.SampleRate
            });
            reader.OnDataReceived = PrepareInputForFFT;
            SetAudioHelper(SoundCard.Channels);

            reader.Start();
        }

        private short[] receivedData = new short[1];
        private void PrepareInputForFFT(float[] data)
        {
            TransformedData = data;
            //audioHelper.ByteArrayTo16BITInputFormat(ref receivedData, data);

            //FFTHelpers.SendSamples(processor.SampleAggregator, receivedData);
        }

        public void AssignCalculatedData(object sender, FFTEventArgs e)
        {
            //FFTHelpers.GetFrequencyValues(ref transformedData, e.Result);
            //OnPropertyChanged(nameof(TransformedData));
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
