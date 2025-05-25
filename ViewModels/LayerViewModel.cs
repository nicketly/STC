using STC.WPF.Database;
using STC.WPF.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace STC.WPF.ViewModels
{
    public class LayerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Material> Materials { get; set; }
        public CollectionViewSource GroupedMaterialsView { get; }

        private Layer _layer;
        public Layer Layer
        {
            get => _layer;
            set { _layer = value; OnPropertyChanged(); }
        }

        public LayerViewModel()
        {
            Materials = new ObservableCollection<Material>(DatabaseInitializer.LoadMaterials());

            GroupedMaterialsView = new CollectionViewSource
            {
                Source = Materials
            };
            GroupedMaterialsView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public ICollectionView GroupedMaterials => GroupedMaterialsView.View;
        public bool IsInnerRadiusReadOnly { get; set; }


        private string _innerRadiusInput;
        public string InnerRadiusInput
        {
            get => _innerRadiusInput;
            set
            {
                if (_innerRadiusInput != value)
                {
                    _innerRadiusInput = value;
                    OnPropertyChanged();

                    if (!IsInnerRadiusReadOnly)
                    {
                        // Разрешаем ввод с запятой или точкой (заменяем запятую на точку)
                        var normalized = value.Replace(',', '.');

                        // Попытка парсинга, но не сбрасываем текст, если парсинг неудачен
                        if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                        {
                            Layer.InnerRadius = parsed;
                        }
                    }
                }
            }
        }

        private string _outerRadiusInput;
        public string OuterRadiusInput
        {
            get => _outerRadiusInput;
            set
            {
                if (_outerRadiusInput != value)
                {
                    _outerRadiusInput = value;
                    OnPropertyChanged();

                    var normalized = value.Replace(',', '.');

                    if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                    {
                        Layer.OuterRadius = parsed;
                    }
                }
            }
        }


        public void UpdateInnerRadiusFromModel()
        {
            _innerRadiusInput = Layer.InnerRadius.ToString("G", CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(InnerRadiusInput));
        }

        public void UpdateOuterRadiusFromModel()
        {
            _outerRadiusInput = Layer.OuterRadius.ToString("G", CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(OuterRadiusInput));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
