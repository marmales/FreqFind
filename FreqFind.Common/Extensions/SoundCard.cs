namespace FreqFind.Common.Extensions
{
    public static class SoundCard
    {
        public static int SampleRate { get; set; } = 48000;
        public static int Channels { get; set; } = 2;
        public static int BufferSize { get; set; } = 8192; //2^13
    }
}
