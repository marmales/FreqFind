using System;
using FreqFind.Common.Interfaces;
using FreqFind.Common.Models;
using NAudio.Wave;

namespace FreqFind.Lib.ViewModels
{
    public class AsioReaderViewModel : IAudioReader<float>
    {
        #region Temporary fields
        private int inputNumber = 1;
        private int sampleRate = 44100;

        #endregion  
        private AsioOut asioOut;

        public AsioReaderViewModel()
        {

            var drives = AsioOut.GetDriverNames();
        }

        public void Setup(IAudioSettings settings)
        {
            var drives = AsioOut.GetDriverNames();
            asioOut = new AsioOut("ASIO4ALL v2");

            //asioOut.InputChannelOffset = inputNumber;
            asioOut.InitRecordAndPlayback(null, 1, sampleRate);
            asioOut.AudioAvailable += OnAsioAudioAvailable;
        }
        private float[] dataSample = new float[512];
        private void OnAsioAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            e.GetAsInterleavedSamples(dataSample);
            OnDataReceived(dataSample);
        }

        public void Start()
        {
            asioOut.Play();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        public Action<float[]> OnDataReceived { get; set; }
    }
}
