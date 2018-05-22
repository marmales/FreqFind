using FreqFind.Common;
using System;

namespace FreqFind.Lib.Models
{
    public class Channel : Observable
    {
        public int Volume
        {
            get { return volume; }
            set
            {
                if (volume == value) return;

                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("Volume value must be between 0 and 100");
                volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }
        int volume;

        public int Id
        {
            get { return id; }
            set
            {
                if (id == value) return;
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        int id;
    }
}
