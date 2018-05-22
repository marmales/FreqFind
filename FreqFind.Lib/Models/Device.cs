using FreqFind.Common;
using System.Collections.ObjectModel;

namespace FreqFind.Lib.Models
{
    public class Device : Observable
    {
        public int Id { get; }
        public int ChannelsCount { get; }
        public string Name { get; }
        public Device(int id, int channels, string name)
        {
            this.Id = id;
            this.ChannelsCount = channels;
            SetChannelsVolume(channels);
            this.Name = name;
        }


        public ObservableCollection<Channel> Channels
        {
            get { return channelsVolume ?? (channelsVolume = new ObservableCollection<Channel>()); }
        }
        ObservableCollection<Channel> channelsVolume;

        private void SetChannelsVolume(int channels)
        {
            Channels.Clear();
            for (int i = 0; i < channels; i++)
            {
                Channels.Add(new Channel { Id = i, Volume = 50 });
            }
        }

    }
}
