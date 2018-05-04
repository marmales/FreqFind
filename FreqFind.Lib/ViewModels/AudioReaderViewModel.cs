using System;
using FreqFind.Common.Interfaces;
using FreqFind.Common.Models;
using NAudio.Wave;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace FreqFind.Lib.ViewModels
{
    public class AudioReaderViewModel : IAudioReader<byte>
    {
        private IWaveIn waveIn;
        private WaveFormat waveFormat;
        private BufferedWaveProvider provider;
        
        public AudioReaderViewModel()
        {
        }
        public void Setup(IAudioSettings settings)
        {
            waveFormat = new WaveFormat(settings.SampleRate, settings.Channels);
            waveIn = new WaveInEvent()
            {
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


            waveIn.StartRecording();
        }

        public void Stop()
        {
        }
        public Action<byte[]> OnDataReceived { get; set; }
    }



    public class NoSetupException : Exception
    {
        private static string setupMessage = 
            $"Can't start recording without setting properties (call {nameof(IAudioReader<byte>.Setup)} method before {nameof(IAudioReader<byte>.Start)} method)";
        public NoSetupException() : base (setupMessage)
        {
        }
    }
}
