using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;

namespace STC.WPF.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<double> FinalTemperatures { get; set; } = new();
        public ObservableCollection<double> ThermalConductivities { get; set; } = new();
        public ObservableCollection<double> LocalHeatFluxesDensities { get; set; } = new();

        private double _totalHeatFlow;
        public double TotalHeatFlow
        {
            get => _totalHeatFlow;
            set
            {
                _totalHeatFlow = value;
                OnPropertyChanged();
            }
        }

        private bool _hasData;
        public bool HasData
        {
            get => _hasData;
            set
            {
                _hasData = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection TemperatureSeries { get; set; } = new();
        public SeriesCollection HeatFluxSeries { get; set; } = new();

        public void SetData(Models.CalculationOutput output, Models.CalculationAlgorithm algorithm)
        {
            FinalTemperatures.Clear();
            ThermalConductivities.Clear();
            LocalHeatFluxesDensities.Clear();

            foreach (var t in output.FinalTemperatures)
                FinalTemperatures.Add(t);

            foreach (var l in algorithm.ThermalConductivities)
                ThermalConductivities.Add(l);

            foreach (var q in algorithm.LocalHeatFluxesDensities)
                LocalHeatFluxesDensities.Add(q);

            TotalHeatFlow = output.TotalHeatFlow;
            HasData = true;

            TemperatureSeries.Clear();
            TemperatureSeries.Add(new LineSeries
            {
                Title = "Температуры",
                Values = new ChartValues<double>(output.FinalTemperatures)
            });

            HeatFluxSeries.Clear();
            HeatFluxSeries.Add(new LineSeries
            {
                Title = "Плотность потока",
                Values = new ChartValues<double>(algorithm.LocalHeatFluxesDensities)
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
