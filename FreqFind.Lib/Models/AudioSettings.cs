using System;
using FreqFind.Common.Models;

namespace FreqFind.Lib.Models
{
    public class AudioSettings : IAudioSettings
    {
        public int BufferSize { get; set; }

        public int Channels { get; set; }

        public int DeviceNumber { get; set; }

        public int SampleRate { get; set; }
    }
}
