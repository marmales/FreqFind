using FreqFind.Lib.Helpers;
using FreqFind.Common;
using NAudio.Wave;
using System.Timers;
using System;
using FreqFind.Common.Extensions;
using FreqFind.Common.Interfaces;

namespace FreqFind.Lib.ViewModels
{
    public class MainViewModel : BaseDialogViewModel
    {
        public MainViewModel()
        {
            RecordingDevice = new WaveIn()
            {
                //WaveFormat = new WaveFormat(),
                WaveFormat = new WaveFormat(SoundCard.SampleRate, 1),
                DeviceNumber = 0
            };
            WaveProvider = WaveProvider.SetWaveFormat(RecordingDevice);
            RecordingDevice.DataAvailable += RecordingDevice_DataAvailable;
            RecordingDevice.StartRecording();

            Start(); //Start timer

        }

        private void RecordingDevice_DataAvailable(object sender, WaveInEventArgs e)
        {
            WaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        public ISoundTone CurrentTone
        {
            get { return currentTone ?? (currentTone = new ToneViewModel()); }
            set
            {
                if (currentTone == value) return;
                currentTone = value;
                OnPropertyChanged(nameof(CurrentTone));
            }
        }
        ISoundTone currentTone;

        public double[] TransformedData // hide if tone will be implemented
        {
            get
            {
                return transformedData ?? (transformedData = new double[0]);
            }
            set
            {
                if (transformedData == value) return;
                transformedData = value;
                CurrentTone.SetHighestFreq(transformedData);
                OnPropertyChanged(nameof(TransformedData));
            }
        }
        double[] transformedData;

        public double[] RawData
        {
            get
            {
                return rawData ?? (rawData = new double[0]);
            }
            set
            {
                if (rawData == value) return;
                rawData = value;
                TransformedData = FFTHelpers.FFT(rawData);
                OnPropertyChanged(nameof(RawData));
            }
        }
        double[] rawData;
        #region Timer
        protected Timer timer { get; set; }
        protected void Start(bool restart = false)
        {
            if (timer == null || restart)
                CreateTimer();

            timer.Start();
        }

        void CreateTimer()
        {
            timer = new Timer()
            {
                Interval = 10, //defined in ms
                Enabled = true,
                AutoReset = true
            };

            timer.Elapsed += OnTimeElapsed;
        }

        //go with async
        void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            var frames = new byte[SoundCard.BufferSize];
            WaveProvider.Read(frames, 0, frames.Length);
            if (frames.Length < 0 || frames[frames.Length - 2] == 0) return;

            timer.Enabled = false;

            var sample_resolution = 16;
            var bytes_per_point = sample_resolution / 8;
            //var vals = new Int32[frames.Length / bytes_per_point];
            var Ys = new double[frames.Length / bytes_per_point];
            var Xs = new double[frames.Length / bytes_per_point];

            //processed data xs2 is in khz 
            //var Ys2 = new double[frames.Length / bytes_per_point];
            //var Xs2 = new double[frames.Length / bytes_per_point];
            for (int i = 0; i < Ys.Length; i++)
            {
                var hByte = frames[i * 2 + 1];
                var lByte = frames[i * 2 + 0];
                Ys[i] = (short)((hByte << 8) | lByte);
                Xs[i] = i;
                //Ys[i] = vals[i];
                //Xs2[i] = (double)i / Ys.Length * RATE / 1000.0; //units in khz
            }

            RawData = Ys;

            timer.Enabled = true;
        }

        #endregion

        #region Recording 

        protected BufferedWaveProvider WaveProvider { get; set; }
        public WaveIn RecordingDevice
        {
            get { return recordingDevice; }
            set
            {
                if (recordingDevice == value) return;
                recordingDevice = value;

                //if(RecordingDevice != null && RecordingDevice.WaveFormat != null)
                //    WaveProvider.SetWaveFormat(recordingDevice);
                OnPropertyChanged(nameof(RecordingDevice));
            }
        }
        WaveIn recordingDevice;

        #endregion

    }
}
