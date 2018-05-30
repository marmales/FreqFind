using FreqFind.Common;
using FreqFind.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public static class ToneHelpers
    {
        public static bool InRange(this double freqValue, float leftThreshold, float rightThreshold)
        {
            var leftDistance = freqValue - leftThreshold;
            var rightDistance = freqValue - rightThreshold;

            return leftDistance >= 0 && rightDistance <= 0;
        }

        public static Tone GetCloserTone(this double freqValue, KeyValuePair<float, Tone> left, KeyValuePair<float, Tone> right)
        {
            var leftDistance = freqValue - left.Key;
            var rightDistance = freqValue - right.Key;

            return Math.Abs(leftDistance) < Math.Abs(rightDistance) ? left.Value : right.Value;
        }
        public static bool IsContainedInOctave(this double freq, int octaveIndex)
        {
            return GlobalSettings.TemperedTones_440Hz[octaveIndex].Last().Key > freq;
        }
    }
}
