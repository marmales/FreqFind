using System;

namespace FreqFind.Common.Interfaces
{
    public interface IAudioReader<T>
    {
        Action<T[]> OnDataReceived { get; set; }
        void Setup(IAudioReaderModel model);
        void Start();
        void Stop();

        RecordingState State { get; set; }
    }
    public interface IAudioReaderModel
    {
        int Channels { get; set; }
        int DeviceNumber { get; set; }
        int SampleRate { get; set; }
    }
    public enum RecordingState
    {
        Stoped,
        Recording,
        Paused
    }
}
