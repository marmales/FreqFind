using FreqFind.Common.Interfaces;
using FreqFind.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreqFind.Lib.Helpers
{
    public abstract class SampleAggregatorBase : ISampleAggregator<float>
    {
        protected int targetLength;
        protected int index;
        protected float[] aggregatedData;

        private static object locker = new object();

        public Action<float[]> OnSamplesAccumulated { get; set; }

        public void AddSample(float data)
        {
            aggregatedData[index++] = data;
            if (index >= targetLength)
            {
                index = 0;
                var result = OnSamplesAccumulated.BeginInvoke(aggregatedData, null, locker);
            }
        }
    }
    public class SampleAggregator : SampleAggregatorBase
    {
        public SampleAggregator(int length)
        {
            targetLength = length;
            index = 0;
            aggregatedData = new float[targetLength];
        }

        
    }


    public static class AggregatorHelpers
    {
        //TODO: specyfy custom linq extension to merge sound from channels
        public static void Add16BitSamples(this ISampleAggregator<float> aggregator, short[] data, IEnumerable<int> channelsVolume)
        {
            var volumeList = channelsVolume.ToList();
            if (volumeList.Count == 0)
                throw new ArgumentException("Channels not found!");

            float tmpValue = 0;
            var maxIntValue = 32767; var minIntValue = -32768; var divisior = 32768f;
            var maxFloatValue = maxIntValue / divisior; var minFloatValue = minIntValue / divisior;
            for (int i = 0; i < data.Length; i += volumeList.Count)
            {
                int channelsIndex = 0;
                tmpValue = data.Skip(i).Take(volumeList.Count).Sum(x => (volumeList[channelsIndex++] / 100f) * x); // average value from all channels
                if (tmpValue > maxIntValue)
                    aggregator.AddSample(maxFloatValue);
                else if (tmpValue < minIntValue)
                    aggregator.AddSample(minFloatValue);
                else
                    aggregator.AddSample(tmpValue / divisior); //[-1;1]

            }
        }
    }
}
