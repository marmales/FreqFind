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
        public void Setup(IAudioReaderModel model)
        {
            waveFormat = new WaveFormat(model.SampleRate, model.Channels);
            waveIn = new WaveIn()
            {
                DeviceNumber = model.DeviceNumber,
                WaveFormat = waveFormat
            };
            waveIn.DataAvailable += OnDataAvailable;
        }

        private static object locker = new object();
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            OnDataReceived.BeginInvoke(e.Buffer, null, locker);
            //OnDataReceived.Invoke(e.Buffer);
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
