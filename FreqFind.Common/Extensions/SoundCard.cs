namespace FreqFind.Common.Extensions
{
    public static class SoundCard
    {
        public static int SampleRate { get; set; } = 44100;
        public static int BufferSize { get; set; } = 8192; //2^13
    }
}
