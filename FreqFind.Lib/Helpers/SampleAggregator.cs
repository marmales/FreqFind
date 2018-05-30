using FreqFind.Common.Interfaces;
using FreqFind.Lib.Models;
using System;

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
}
