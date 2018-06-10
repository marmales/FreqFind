using FreqFind.Lib.Helpers;
using FreqFind.Lib.Models;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.Generic;
using System.Windows;

namespace FreqFind.Extensions
{
    public static class ObservableDataSourceExtensions
    {
        public static void AssignFrequencyValues(this ObservableDataSource<Point> source, List<double> newValues, int sampleRate)
        {
            if (newValues == null) return;

            source.SuspendUpdate();
            source.Collection.Clear();
            var length = newValues.Count;
            for (int i = 0; i < length; i += 20)
            {
                var x = FrequencyHelpers.GetFrequency(length, i, sampleRate);
                source.Collection.Add(new Point { X = x, Y = newValues[i] });
            }
            source.ResumeUpdate();
        }
        public static void AssignZoomedValues(this ObservableDataSource<Point> source, List<double> newValues, LocalRange range)
        {
            source.SuspendUpdate();
            source.Collection.Clear();
            var length = newValues.Count;
            var skip = (range.RightThreshold - range.LeftThreshold) / range.ZoomOptions.TargetNumberOfSamples;
            for (int i = 0; i < length; i += 5)
            {
                var x = range.LeftThreshold + i * skip;
                source.Collection.Add(new Point { X = x, Y = newValues[i] });
            }
            source.ResumeUpdate();
        }
    }
}
