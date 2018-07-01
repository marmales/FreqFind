using FreqFind.Common;
using FreqFind.Common.Extensions;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace FreqFind.Lib.ViewModels
{
    public static class TempGlobalSettings
    {

    }
    public class MainViewModel : BaseDialogViewModel
    {
        private IAudioReader<byte> reader;
        private IAudioProcessor processor;
        private ISoundNote soundNote;
        SettingsViewModel settingsViewModel;
        public MainViewModel()
        {
            soundNote = new ToneViewModel();
        }

        #region Properties
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
            get { return audioOptions ?? (audioOptions = AudioHelpers.DefaultSettings()); }
            set { audioOptions = value; }
        }
        private AudioSettings audioOptions;


        private IProcessorModel<float> fftOptions;
        public IProcessorModel<float> FFTOptions
        {
            get { return fftOptions ?? (fftOptions = FFTModelHelpers.GetZoomDefaultFFTOptions(0, AudioOptions.SampleRate)); }
            set
            {
                if (fftOptions == value) return;
                fftOptions = value;
                OnPropertyChanged(nameof(FFTOptions));
            }
        }
        #endregion


        private void StartAudio()
        {
            if (processor != null)
            {
                processor.Cleanup();
            }
            processor = new FFTProcessorViewModel(FFTOptions);
            OnPropertyChanged(nameof(FFTViewModel));

            if (reader == null)
            {
                reader = new AudioReaderViewModel();
            }

            reader.Setup(GetReaderModel());
            (reader as AudioReaderViewModel).WaveIn.BufferMilliseconds = 250;
            (reader as AudioReaderViewModel).WaveIn.DataAvailable += WaveIn_DataAvailable;
            reader.Start();
        }

        private short[] receivedData = new short[1];
        private void WaveIn_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            (new Thread(() =>
            {
                var sw = new Stopwatch();
                sw.Start();
                AudioHelpers.ByteArrayTo16BITInputFormat(ref receivedData, e.Buffer);
                var input = receivedData.Convert2ChannelsToFloat();

                if (processor.Model.InputSamplesCount != input.Count())
                    processor.Model.InputSamplesCount = input.Count();

                var result = processor.Process(input.ToArray());
                var note = NoteViewModel.GetNote(result);
                sw.Stop();
                Debug.WriteLine(string.Format("Note {0}\tTime elapsed {1}", string.Format(note.Tone.ToString(), note.Base), sw.Elapsed));
            })).Start();
        }

        #region Commands
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

        IAudioReaderModel GetReaderModel()
        {
            return new AudioReaderModel
            {
                DeviceNumber = AudioOptions.SelectedDevice.Id,
                Channels = AudioOptions.SelectedDevice.Channels.Count,
                SampleRate = AudioOptions.SampleRate
            };
        }
        #endregion
    }
}
