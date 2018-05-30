using FreqFind.Common.Interfaces;
using System.Collections.Generic;

namespace FreqFind.Common
{
    public static class GlobalSettings
    {
        public static List<Dictionary<float, Tone>> TemperedTones_440Hz
        {
            get { return temperedTones_440Hz ?? (temperedTones_440Hz = CreateTonesDictionary()); }
        }
        private static List<Dictionary<float, Tone>> temperedTones_440Hz;

        private static List<Dictionary<float, Tone>> CreateTonesDictionary()
        {
            var allTones = new List<Dictionary<float, Tone>>();
            var lowestTones = new Dictionary<float, Tone>
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
            var previousOctave = lowestTones;
            allTones.Add(previousOctave);
            for (int i = 1; i < 8; i++)
            {
                var currentOctave = new Dictionary<float, Tone>();
                foreach (var tone in previousOctave)
                {
                    currentOctave.Add(tone.Key * 2, tone.Value);
                }
                allTones.Add(currentOctave);
                previousOctave = currentOctave;
            }
            return allTones;
        }

    }
}
