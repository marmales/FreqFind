using FreqFind.Common;
using FreqFind.Common.Interfaces;

namespace FreqFind.Lib.Models
{
    public class Note : Observable, INote
    {
        public Tone Tone
        {
            get { return tone; }
            set
            {
                if (tone == value) return;
                tone = value;
                OnPropertyChanged(nameof(Tone));
            }
        }
        Tone tone;

        public int Base
        {
            get { return @base; }
            set
            {
                if (@base == value) return;
                @base = value;
                OnPropertyChanged(nameof(Base));
            }
        }
        int @base;
    }
}
