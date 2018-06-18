using FreqFind.Common;
using FreqFind.Common.Interfaces;
using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (localPeaks.Count() == 0)
                return null;
            var distances = GetMostFrequentDistances(localPeaks);
            if (distances.Count() == 0)
                return null;
            return GetNote(distances.Average());
        }
        public INote GetNote(double[] frequencies, int sampleRate)
        {
            var freqValue = frequencies.LoudestFrequency(sampleRate) * 1000;
            return GetNote(freqValue);
        }
        public IEnumerable<double> GetMostFrequentDistances(IEnumerable<double> peaks)
        {
            var threshold = 10;//Hz
            var sortedPeaks = peaks.OrderBy(x => x).ToList();
            var dict = new List<Tuple<double, int>>();
            var distances = GetDistances(sortedPeaks);
            var countList = new List<int>();
            distances.ForEach(x => countList.Add(0));

            for (int i = 0; i < distances.Count; i++)
            {
                for (int j = 0; j < dict.Count; j++)
                {
                    var tmpDistance = distances[j];
                    if (tmpDistance + threshold < distances[i] && tmpDistance - threshold > distances[i])
                        countList[i]++;
                }
            }
            if (countList.Count == 0)
                yield break;
            var max = countList.Max();
            for (int i = 0; i < countList.Count; i++)
            {
                if (countList[i] == max)
                    yield return distances[i];
            }

        }

        private List<double> GetDistances(List<double> sortedPeaks)
        {
            var distances = new List<double>();
            var previousPeak = sortedPeaks.ElementAt(0);
            foreach (var peak in sortedPeaks.Skip(1))
            {
                distances.Add(peak - previousPeak);
                previousPeak = peak;
            }
            return distances;
        }
        private INote GetNote(double targetFrequency)
        {
            for (int octave = 1; octave < GlobalSettings.TemperedTones_440Hz.Count; octave++)
            {
                if (targetFrequency.IsContainedInOctave(octave))
                {
                    LookForValue(octave, targetFrequency);
                    OnPropertyChanged(nameof(CurrentNote));
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
                    CurrentNote = new Note()
                    {
                        Base = octaveIndex,
                        Tone = freqValue.GetCloserTone(left, right)
                    };

                }
                left = right;
                right = GlobalSettings.TemperedTones_440Hz[octaveIndex].ElementAt(j + 1);
            }
        }
    }
}
