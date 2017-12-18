
using FreqFind.Common;
using FreqFind.Common.Extensions;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using System.Linq;

namespace FreqFind.Lib.ViewModels
{
    public class ToneViewModel : Observable, ISoundTone
    {
        public Tone Tone
        {
            get { return tone; }
            private set
            {
                if (tone == value) return; 
                tone = value;
                OnPropertyChanged(nameof(Tone));
            }
        }
        Tone tone;

        public double HighestFrequency//delete if tone will be implemented
        {
            get { return highestFrequency; }
            private set
            {
                if (highestFrequency == value) return; //add module for comparing double values
                highestFrequency = value;
                OnPropertyChanged(nameof(HighestFrequency));
            }
        }
        double highestFrequency;
        public void Set(double[] frequencies)
        {

        }

        public void SetHighestFreq(double[] frequencies)
        {
            var index = FrequencyHelpers.GetIndex(frequencies, frequencies.Max());//index of frequency with the highest amplitude
            if (index < 0)
                return;

            HighestFrequency = FrequencyHelpers.GetValue(frequencies, index);
        }
    }
}
