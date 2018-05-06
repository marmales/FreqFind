using FreqFind.Common;
using FreqFind.Common.Interfaces;
using NAudio.Wave;
using System;

namespace FreqFind.Lib.ViewModels
{
    public class AudioReaderViewModel : BaseDialogViewModel, IAudioReader<byte>
    {
        private IWaveIn waveIn;
        private WaveFormat waveFormat;

        public AudioReaderViewModel()
        {
        }
        public void Setup(int sampleRate, int channels, int deviceNumber)
        {
            waveFormat = new WaveFormat(sampleRate, channels);
            waveIn = new WaveIn()
            {
                DeviceNumber = deviceNumber,
                WaveFormat = waveFormat
            };
            waveIn.DataAvailable += OnDataAvailable;


        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            OnDataReceived.Invoke(e.Buffer);
        }

        public void Start()
        {
            if (waveIn == null || waveFormat == null)
                throw new NoSetupException();

            State = RecordingState.Recording;
            waveIn.StartRecording();
        }

        public void Stop()
        {
            State = RecordingState.Stoped;
            waveIn.StopRecording();
        }
        public Action<byte[]> OnDataReceived { get; set; }

        public RecordingState State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                state = value;
                OnPropertyChanged(nameof(State));
            }
        }
        RecordingState state;
    }



    public class NoSetupException : Exception
    {
        private static string setupMessage =
            $"Can't start recording without setting properties (call Setup method before {nameof(IAudioReader<byte>.Start)} method)";
        public NoSetupException() : base(setupMessage)
        {
        }
    }
}
