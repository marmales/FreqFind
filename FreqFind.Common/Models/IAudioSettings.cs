namespace FreqFind.Common.Models
{
    public interface IAudioSettings
    {
        int SampleRate { get; set; }
        int Channels { get; set; }
        int BufferSize { get; set; }

        int DeviceNumber { get; set; }
    }
}
