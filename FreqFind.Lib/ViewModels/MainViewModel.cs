using FreqFind.Common;
using FreqFind.Common.Extensions;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Linq;
using System.Windows.Input;

namespace FreqFind.Lib.ViewModels
{
    public class MainViewModel : BaseDialogViewModel
    {
        private IAudioReader<byte> reader;
        private IAudioProcessor processor;
        private IAudioHelpers audioHelper;
        private ISoundNote soundNote;
        SettingsViewModel settingsViewModel;
        public MainViewModel()
        {
            soundNote = new ToneViewModel();
        }

        public IAudioProcessor FFTViewModel
        {
            get { return processor; }
        }
        public ISoundNote NoteViewModel
        {
            get { return soundNote; }
        }

        public AudioSettings AudioOptions
        {
            get { return audioOptions ?? (audioOptions = DefaultSettings()); }
            set { audioOptions = value; }
        }

        private AudioSettings DefaultSettings()
        {
            return new AudioSettings()
            {
                BufferSize = 8192,
                SampleRate = 44100,
                SelectedDevice = SettingsViewModel.GetDevices().FirstOrDefault()
            };
        }

        private AudioSettings audioOptions;


        public ICommand OpenAudioSettingsCommand
        {
            get
            {
                return openAudioSettingsCommand ?? (openAudioSettingsCommand = new RelayCommand(
                      p => true,
                      p => DisplaySettingsWindow()));
            }
        }
        ICommand openAudioSettingsCommand;
        void DisplaySettingsWindow()
        {
            if (settingsViewModel == null)
                settingsViewModel = new SettingsViewModel(AudioOptions);

            if (reader != null)
                reader.Stop();
            settingsViewModel.ResolveDialog();
        }

        public ICommand StartCommand
        {
            get
            {
                return startCommand ?? (startCommand = new RelayCommand(
                    p => reader == null || reader.State == RecordingState.Stoped || reader.State == RecordingState.Paused,
                    p => StartAudio()));
            }
        }
        ICommand startCommand;

        public ICommand StopCommand
        {
            get
            {
                return stopCommand ?? (stopCommand = new RelayCommand(
                    p => reader != null && reader.State == RecordingState.Recording,
                    p => reader.Stop()));
            }
        }
        ICommand stopCommand;


        private void StartAudio()
        {
            if (processor != null)
            {
                processor.Cleanup();
                processor.OnFFTCalculated -= AssignCalculatedData;
            }
            processor = new FFTProcessorViewModel(AudioOptions.BufferSize);
            processor.OnFFTCalculated += AssignCalculatedData;
            OnPropertyChanged(nameof(FFTViewModel));

            if (reader == null)
            {
                reader = new AudioReaderViewModel();
                reader.OnDataReceived = PrepareInputForFFT;
            }
            audioHelper = new AudioHelpers_16bitPCM();

            reader.Setup(AudioOptions.SampleRate, AudioOptions.SelectedDevice.ChannelsCount, AudioOptions.SelectedDevice.Id);
            reader.Start();
        }

        private short[] receivedData = new short[1];
        private void PrepareInputForFFT(byte[] data)
        {
            audioHelper.ByteArrayTo16BITInputFormat(ref receivedData, data); //From byte[] To 16bit format
            FFTHelpers.SendSamples(
                processor.SampleAggregator,
                receivedData,
                AudioOptions.SelectedDevice.Channels.Select(x => x.Volume)); //Process with FFT when buffer will be filled - SoundCard.BufferSize
        }

        public void AssignCalculatedData(object sender, FFTEventArgs e)
        {
            soundNote.GetNote(e.Result, AudioOptions.SampleRate);
        }
    }
}
