using System;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioReader<T>
    {
        Action<T[]> OnDataReceived { get; set; }
        void Setup(int sampleRate, int channels, int deviceNumber);
        void Start();
        void Stop();

        RecordingState State { get; set; }
    }

    public enum RecordingState
    {
        Stoped,
        Recording,
        Paused
    }
}
