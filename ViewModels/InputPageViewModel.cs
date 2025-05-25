using STC.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using STC.WPF.Services;

namespace STC.WPF.ViewModels
{
    public class InputPageViewModel : INotifyPropertyChanged
    {
        public InputPageViewModel()
        {
            LayerCount = 1;
            Layers = new ObservableCollection<LayerViewModel>();
            UpdateLayers();

            CalculateCommand = new RelayCommand(Calculate);
        }

        public ObservableCollection<LayerViewModel> Layers { get; set; }

        private int _layerCount = 1;
        public int LayerCount
        {
            get => _layerCount;
            set
            {
                if (_layerCount != value)
                {
                    _layerCount = value;
                    OnPropertyChanged(nameof(LayerCount));
                    UpdateLayers();
                    InitializeTemperatures();
                }
            }
        }

        private string _wallHeightInput;
        public string WallHeightInput
        {
            get => _wallHeightInput;
            set
            {
                if (_wallHeightInput != value)
                {
                    _wallHeightInput = value;
                    OnPropertyChanged(nameof(WallHeightInput));
                    if (double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                    {
                        WallHeight = parsed;
                    }
                }
            }
        }
        public double WallHeight { get; private set; }

        private string _tInnerInput;
        public string TInnerInput
        {
            get => _tInnerInput;
            set
            {
                if (_tInnerInput != value)
                {
                    _tInnerInput = value;
                    OnPropertyChanged(nameof(TInnerInput));
                    if (double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                    {
                        TInner = parsed;
                    }
                }
            }
        }
        public double TInner { get; private set; }

        private string _tOuterInput;
        public string TOuterInput
        {
            get => _tOuterInput;
            set
            {
                if (_tOuterInput != value)
                {
                    _tOuterInput = value;
                    OnPropertyChanged(nameof(TOuterInput));
                    if (double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                    {
                        TOuter = parsed;
                    }
                }
            }
        }
        public double TOuter { get; private set; }

        public List<double> Temperatures { get; set; } = new();

        private void InitializeTemperatures()
        {
            Temperatures.Clear();
            for (int i = 0; i <= LayerCount; i++)
            {
                double temp = TInner + i * (TOuter - TInner) / LayerCount;
                Temperatures.Add(temp);
            }
            OnPropertyChanged(nameof(Temperatures));
        }

        public bool ValidateInput(out string errorMessage)
        {
            if (WallHeight <= 0)
            {
                errorMessage = "Высота стенки может принимать толко положительное значение";
                return false;
            }

            if (Layers.Any(l => l.Layer.InnerRadius <= 0 || l.Layer.OuterRadius <= 0))
            {
                errorMessage = "Радиусы могут принимать толко положительные значения";
                return false;
            }

            for (int i = 0; i < LayerCount; i++)
            {
                var layer = Layers[i].Layer;
                if (layer.InnerRadius >= layer.OuterRadius)
                {
                    errorMessage = $"В слое {i + 1} внутренний радиус должен быть меньше внешнего";
                    return false;
                }

            }

            errorMessage = null;
            return true;
        }

        private void UpdateLayers()
        {
            while (Layers.Count < LayerCount)
            {
                var layer = new Layer { LayerNumber = Layers.Count + 1 };
                var vm = new LayerViewModel { Layer = layer };

                if (Layers.Count > 0)
                {
                    var prevLayer = Layers.Last().Layer;
                    layer.InnerRadius = prevLayer.OuterRadius;
                    vm.IsInnerRadiusReadOnly = true;
                }

                vm.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(LayerViewModel.OuterRadiusInput))
                    {
                        int index = Layers.IndexOf(vm);
                        if (index >= 0 && index < Layers.Count - 1)
                        {
                            var nextLayerVM = Layers[index + 1];
                            nextLayerVM.Layer.InnerRadius = vm.Layer.OuterRadius;
                            nextLayerVM.UpdateInnerRadiusFromModel();
                        }
                    }
                };

                Layers.Add(vm);
            }

            while (Layers.Count > LayerCount)
                Layers.RemoveAt(Layers.Count - 1);

            for (int i = 0; i < Layers.Count; i++)
                Layers[i].Layer.LayerNumber = i + 1;
        }

        private void Calculate()
        {
            if (!ValidateInput(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var input = new CalculationInput
            {
                WallHeight = WallHeight,
                TInner = TInner,
                TOuter = TOuter,
                Layers = Layers.Select(l => l.Layer).ToList()
            };

            var result = CalculationService.Calculate(input);
            MessageBox.Show($"Расчёт завершён. Результат: {result}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public ICommand CalculateCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
