using FreqFind.Lib.ViewModels;
using FreqFind.Extensions;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using Microsoft.Research.DynamicDataDisplay.Charts;
using FreqFind.Common.Extensions;
using System.Diagnostics;

namespace FreqFind.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newViewModel = e.NewValue as MainViewModel;
            var oldViewModel = e.OldValue as MainViewModel;

            if (oldViewModel != null)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                source = null;
            }

            if (newViewModel != null)
            {
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
                source = new ObservableDataSource<Point>();
                source.SetXYMapping(x => x);

                var lineGraph = fftChart.AddLineGraph(source, Color.FromRgb(0,0,0), 1, "Widmo");
            }
        }
        ObservableDataSource<Point> source;
        void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = sender as MainViewModel;
            if (vm == null) return;

            if (string.Equals(e.PropertyName, nameof(MainViewModel.TransformedData)))
            {
                this.Dispatcher.Invoke(() =>
                {
                    //source.AssignFrequencyValues(vm.TransformedData.Take(vm.TransformedData.Length / 2).ToArray());
                    source.AssignFrequencyValues(vm.TransformedData);
                });

            }
        }

        void SetAxisFormats(bool register)
        {
            if (register)
            {
                //var verticalAxis = fftChart.VerticalAxis as VerticalAxis;
                //if (verticalAxis != null)
                //    verticalAxis.AxisControl.LabelProvider.CustomFormatter = YAxisFormatter;
                var horizontalAxis = fftChart.HorizontalAxis as HorizontalAxis;
                if (horizontalAxis != null)
                    horizontalAxis.AxisControl.LabelProvider.CustomFormatter = XAxisFormatter;

            }
            else
            {
                //var verticalAxis = fftChart.VerticalAxis as VerticalAxis;
                //if (verticalAxis != null)
                //    verticalAxis.AxisControl.LabelProvider.CustomFormatter = null;
                var horizontalAxis = fftChart.HorizontalAxis as HorizontalAxis;
                if (horizontalAxis != null)
                    horizontalAxis.AxisControl.LabelProvider.CustomFormatter = null;

            }

        }

        private string XAxisFormatter(LabelTickInfo<double> arg)
        {
            throw new NotImplementedException();
        }

        private string YAxisFormatter(LabelTickInfo<double> arg)
        {
            throw new NotImplementedException();
        }
    }
}
