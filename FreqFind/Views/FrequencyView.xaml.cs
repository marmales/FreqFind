using FreqFind.Extensions;
using FreqFind.Lib.ViewModels;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FreqFind.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class FrequencyView : UserControl
    {
        public FrequencyView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var newViewModel = e.NewValue as FFTProcessorViewModel;
            var oldViewModel = e.OldValue as FFTProcessorViewModel;

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

                var lineGraph = fftChart.AddLineGraph(source, Color.FromRgb(0, 128, 255), 1, "Widmo");

            }
        }
        ObservableDataSource<Point> source;
        void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = sender as FFTProcessorViewModel;
            if (vm == null) return;

            if (string.Equals(e.PropertyName, nameof(FFTProcessorViewModel.TransformedData)))
            {
                this.Dispatcher.Invoke(() =>
                {
                    source.AssignFrequencyValues(vm.TransformedData, vm.Model.InputSamplesCount);
                    //source.AssignZoomedValues(vm.TransformedData, vm.CurrentRange);
                });

            }
        }
    }
}
