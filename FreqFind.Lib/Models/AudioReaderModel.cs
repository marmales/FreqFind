using FreqFind.Common.Interfaces;

namespace FreqFind.Lib.Models
{
    public class AudioReaderModel : IAudioReaderModel
    {
        public int Channels { get; set; }
        public int DeviceNumber { get; set; }
        public int SampleRate { get; set; }
    }
}
