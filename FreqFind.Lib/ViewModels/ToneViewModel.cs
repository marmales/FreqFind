using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FreqFind.Lib.ViewModels
{
    public class ToneViewModel : BaseDialogViewModel, ISoundNote
    {
        public ToneViewModel()
        {
        }

        public INote CurrentNote
        {
            get { return currentNote ?? (currentNote = new Note()); }
            set
            {
                if (currentNote == value) return;
                currentNote = value;
                OnPropertyChanged(nameof(CurrentNote));
            }
        }
        INote currentNote;
        public INote GetNote(IEnumerable<double> localPeaks)
        {
            var sortedPeaks = localPeaks.OrderBy(x => x).ToArray();
            double previousDistance = 0;
            double distanceSum = 0;
            for (int i = 0; i < sortedPeaks.Length; i++)
            {
                distanceSum += (sortedPeaks[i] - previousDistance);
                previousDistance = sortedPeaks[i];
            }

            var avg = distanceSum / sortedPeaks.Length;

            return GetNote(avg);
        }
        public INote GetNote(double[] frequencies, int sampleRate)
        {
            var freqValue = frequencies.LoudestFrequency(sampleRate) * 1000;
            return GetNote(freqValue);
        }
        private INote GetNote(double targetFrequency)
        {
            for (int octave = 1; octave < GlobalSettings.TemperedTones_440Hz.Count; octave++)
            {
                if (targetFrequency.IsContainedInOctave(octave))
                {
                    LookForValue(octave, targetFrequency);
                    return CurrentNote;
                }
            }

            //If nothing match then assign last value
            CurrentNote.Base = 9;
            CurrentNote.Tone = Tone.C;

            return CurrentNote;
        }
        private void LookForValue(int octaveIndex, double freqValue)
        {
            var left = GlobalSettings.TemperedTones_440Hz[octaveIndex - 1].Single(x => x.Value == Tone.B); //initialized with B from previous octave
            var right = GlobalSettings.TemperedTones_440Hz[octaveIndex].FirstOrDefault(); //initialized with C from current octave
            if (right.Value != Tone.C)
                throw new OperationCanceledException("First tone must be a C");
            for (int j = 0; j < GlobalSettings.TemperedTones_440Hz[octaveIndex].Count - 1; j++)
            {

                if (freqValue.InRange(left.Key, right.Key))
                {
                    CurrentNote.Base = octaveIndex;
                    CurrentNote.Tone = freqValue.GetCloserTone(left, right);
                    
                }
                left = right;
                right = GlobalSettings.TemperedTones_440Hz[octaveIndex].ElementAt(j + 1);
            }
        }
    }
}
