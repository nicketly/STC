using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClosedXML.Excel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using STC.WPF.Models;
using System.Windows.Input;

namespace STC.WPF.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<double> FinalTemperatures { get; set; } = new();
        public ObservableCollection<double> ThermalConductivities { get; set; } = new();
        public ObservableCollection<double> LocalHeatFluxesDensities { get; set; } = new();
        public Axis TemperatureAxisX { get; set; }
        public Axis TemperatureAxisY { get; set; }
        public Axis LocalHeatFluxDensityAxisX { get; set; }
        public Axis LocalHeatFluxDensityAxisY { get; set; }
        public CalculationInput Input { get; set; }
        public CalculationAlgorithm Algorithm { get; set; }
        public CalculationOutput Output { get; set; }

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

        public void SetData(Models.CalculationOutput output, Models.CalculationAlgorithm algorithm, Models.CalculationInput input)
        {
            Output = output;
            Algorithm = algorithm;
            Input = input;

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
                Values = new ChartValues<double>(output.FinalTemperatures),
                LineSmoothness = 0
            });
            var maxTemp = output.FinalTemperatures.Max();
            TemperatureAxisY = new Axis
            {
                MinValue = 0,
                MaxValue = maxTemp * 1.25,
                Title = "Температура, °С"
            };
            TemperatureAxisX = new Axis
            {
                Labels = Enumerable.Range(1, FinalTemperatures.Count).Select(i => i.ToString()).ToArray(),
                Title = "Индекс границы слоя"
            };

            HeatFluxSeries.Clear();
            HeatFluxSeries.Add(new LineSeries
            {
                Values = new ChartValues<double>(algorithm.LocalHeatFluxesDensities),
                LineSmoothness = 0
            });
            var maxLocalHeatFluxDensity = algorithm.LocalHeatFluxesDensities.Max();
            LocalHeatFluxDensityAxisY = new Axis
            {
                MinValue = 0,
                MaxValue = maxLocalHeatFluxDensity * 1.2,
                Title = "Плотность потока, Вт/м"
            };
            LocalHeatFluxDensityAxisX = new Axis
            {
                Labels = Enumerable.Range(1, LocalHeatFluxesDensities.Count).Select(i => i.ToString()).ToArray(),
                Title = "Индекс слоя"
            };
        }

        public void SaveAsExcel()
        {
            if (Input == null || Output == null || Algorithm == null)
            {
                MessageBox.Show("Нет данных для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Сохранить отчёт в Excel"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Отчёт");

            int row = 1;

            // Заголовок
            ws.Cell(row++, 1).Value = "Отчёт по тепловому расчёту";

            // Входные данные
            ws.Cell(row++, 1).Value = "Входные данные";
            ws.Cell(row++, 1).Value = $"Количество слоёв: {Input.Layers.Count}";
            ws.Cell(row++, 1).Value = $"Высота стенки: {Input.WallHeight} м";
            ws.Cell(row++, 1).Value = $"Температура внутренней поверхности: {Input.TInner} °C";
            ws.Cell(row++, 1).Value = $"Температура внешней поверхности: {Input.TOuter} °C";

            row++;
            ws.Cell(row++, 1).Value = "Слои:";

            ws.Cell(row, 1).Value = "№";
            ws.Cell(row, 2).Value = "Материал";
            ws.Cell(row, 3).Value = "Внутренний радиус";
            ws.Cell(row, 4).Value = "Внешний радиус";
            row++;

            for (int i = 0; i < Input.Layers.Count; i++)
            {
                var layer = Input.Layers[i];
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = layer.SelectedMaterial?.Name ?? "-";
                ws.Cell(row, 3).Value = layer.InnerRadius;
                ws.Cell(row, 4).Value = layer.OuterRadius;
                row++;
            }

            row++;
            ws.Cell(row++, 1).Value = "Температуры на границах слоёв:";
            for (int i = 0; i < Output.FinalTemperatures.Count; i++)
            {
                ws.Cell(row++, 1).Value = $"t{i + 1}: {Output.FinalTemperatures[i]} °C";
            }

            row++;
            ws.Cell(row++, 1).Value = "Коэффициенты теплопроводности:";
            for (int i = 0; i < Algorithm.ThermalConductivities.Count; i++)
            {
                ws.Cell(row++, 1).Value = $"λ{i + 1}: {Algorithm.ThermalConductivities[i]} Вт/(м·К)";
            }

            row++;
            ws.Cell(row++, 1).Value = "Распределение плотности теплового потока:";
            for (int i = 0; i < Algorithm.LocalHeatFluxesDensities.Count; i++)
            {
                ws.Cell(row++, 1).Value = $"ql{i + 1}: {Algorithm.LocalHeatFluxesDensities[i]} Вт/м";
            }

            row++;
            ws.Cell(row++, 1).Value = $"Тепловой поток через стенку Q: {Output.TotalHeatFlow} Вт";

            workbook.SaveAs(saveFileDialog.FileName);
            MessageBox.Show("Отчёт успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        public ICommand SaveAsExcelCommand { get; }
        public ReportViewModel()
        {
            SaveAsExcelCommand = new RelayCommand(SaveAsExcel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
