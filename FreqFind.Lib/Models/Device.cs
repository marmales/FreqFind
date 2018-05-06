namespace FreqFind.Lib.Models
{
    public class Device
    {
        public Device(int id, int channels, string name)
        {
            this.Id = id;
            this.Channels = channels;
            this.Name = name;
        }
        public int Id { get; }
        public int Channels { get; }
        public string Name { get; }

    }
}
