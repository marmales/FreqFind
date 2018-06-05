using FreqFind.Common.Extensions;
using FreqFind.Lib.Helpers;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FreqFind.Extensions
{
    public static class ObservableDataSourceExtensions
    {
        public static void AssignFrequencyValues(this ObservableDataSource<Point> source, double[] newValues, int sampleRate)
        {
            if (newValues == null) return;

            source.SuspendUpdate();
            source.Collection.Clear();
            var length = newValues.Length;
            for (int i = 0; i < newValues.Length; i+=20)
            {
                var x = FrequencyHelpers.GetFrequency(length, i, sampleRate);
                source.Collection.Add(new Point { X = x, Y = newValues[i] });
            }
            source.ResumeUpdate();
        }
        public static void AssignTempValues(this ObservableDataSource<Point> source, float[] rawdata)
        {
            if (rawdata == null) return;

            source.SuspendUpdate();
            source.Collection.Clear();
            for (int i = 0; i < rawdata.Length; i += 10)
            {
                source.Collection.Add(new Point { X = i, Y = rawdata[i] * 10000 });
            }
            source.ResumeUpdate();
        }
        public static void AssignTempValues(this ObservableDataSource<Point> source, short[] rawdata)
        {
            if (rawdata == null) return;

            source.SuspendUpdate();
            source.Collection.Clear();
            for (int i = 0; i < rawdata.Length; i+=10)
            {
                source.Collection.Add(new Point { X = i, Y = rawdata[i]});
            }
            source.ResumeUpdate();
        }
    }
}
