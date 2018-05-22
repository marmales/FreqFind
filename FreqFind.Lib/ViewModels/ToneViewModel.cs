
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
        private IList<IDictionary<float, Tone>> notes;
        public ToneViewModel()
        {
            notes = new List<IDictionary<float, Tone>>();
            var previousTones = lowestTones;
            notes.Add(previousTones);
            for (int i = 1; i < 8; i++)
            {
                var tones = new Dictionary<float, Tone>();
                foreach (var tone in previousTones)
                {
                    tones.Add(tone.Key * 2, tone.Value);
                }
                notes.Add(tones);
                previousTones = tones;
            }
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

        public INote GetNote(double[] frequencies, int sampleRate)
        {
            var freqValue = FrequencyHelpers.GetValue(frequencies, frequencies.ToList().IndexOf(frequencies.Max()), sampleRate) * 1000;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Last().Key > freqValue)
                {
                    var right = notes[i].FirstOrDefault(); //initialized with C
                    var left = i == 0 ? right : notes[i - 1].Single(x => x.Value == Tone.B); //initialized with B
                    if (right.Value != Tone.C)
                        throw new OperationCanceledException("First tone must be a C");
                    for (int j = 0; j < notes[i].Count - 1; j++)
                    {
                        var leftDistance = freqValue - left.Key;
                        var rightDistance = freqValue - right.Key;
                        if (leftDistance >= 0 && rightDistance <= 0)
                        {
                            CurrentNote.Base = i;
                            CurrentNote.Tone = IsLeftCloser(leftDistance, rightDistance) ? left.Value : right.Value;

                            return CurrentNote;
                        }
                        left = right;
                        right = notes[i].ElementAt(j + 1);
                    }
                }
            }

            CurrentNote.Base = 9;
            CurrentNote.Tone = Tone.C;
            return CurrentNote;
        }
        private bool IsLeftCloser(double leftValue, double rightValue)
        {
            return Math.Abs(leftValue) < Math.Abs(rightValue);
        }
        private Dictionary<float, Tone> lowestTones = new Dictionary<float, Tone>
        {
            { 16.35f, Tone.C},
            { 17.32f, Tone.Db},
            { 18.35f, Tone.D},
            { 19.45f, Tone.Eb},
            { 20.60f, Tone.E},
            { 21.83f, Tone.F},
            { 23.12f, Tone.Gb},
            { 24.5f , Tone.G},
            { 25.96f, Tone.Ab},
            { 27.5f , Tone.A},
            { 29.14f, Tone.Bb},
            { 30.87f, Tone.B}
        };
    }
}
