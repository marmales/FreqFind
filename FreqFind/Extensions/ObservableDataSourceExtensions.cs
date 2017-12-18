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
        public static void AssignFrequencyValues(this ObservableDataSource<Point> source, double[] newValues)
        {
            if (newValues == null) return;

            source.SuspendUpdate();
            source.Collection.Clear();
            for (int i = 0; i < newValues.Length; i++)
            {
                var x = FrequencyHelpers.GetValue(newValues, i);
                //if (x > 3)
                    //break; // take only first 3khz
                source.Collection.Add(new Point { X = x, Y = newValues[i] });
            }
            source.ResumeUpdate();
        }
    }
}
