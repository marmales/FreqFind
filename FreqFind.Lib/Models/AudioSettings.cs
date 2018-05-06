using FreqFind.Common;

namespace FreqFind.Lib.Models
{
    public class AudioSettings : Observable
    {
        public Device SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                if (value == null) return;
                selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));

            }
        }
        Device selectedDevice;

        public int SampleRate
        {
            get { return sampleRate; }
            set
            {
                if (value < 0) return;
                sampleRate = value;
                OnPropertyChanged(nameof(SampleRate));
            }
        }
        int sampleRate;

        public int BufferSize
        {
            get { return bufferSize; }
            set
            {
                if (value < 0) return;
                bufferSize = value;
                OnPropertyChanged(nameof(BufferSize));
            }
        }
        int bufferSize;


        public float LeftVolume
        {
            get { return leftVolume; }
            set
            {
                if (value < 0 || value > 1) return;
                leftVolume = value;
                rightVolume = 1 - leftVolume;
                OnPropertyChanged(nameof(LeftVolume));
                OnPropertyChanged(nameof(RightVolume));
            }
        }
        float leftVolume = 0.5f;

        public float RightVolume
        {
            get { return rightVolume; }
            set
            {
                if (value < 0 || value > 1) return;
                rightVolume = value;
                leftVolume = 1 - rightVolume;
                OnPropertyChanged(nameof(LeftVolume));
                OnPropertyChanged(nameof(RightVolume));
            }
        }
        float rightVolume = 0.5f;
    }
}
