using STC.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC.WPF.ViewModels
{
    public class InputPageViewModel : INotifyPropertyChanged
    {
        public InputPageViewModel()
        {
            LayerCount = 1;
            Layers = new ObservableCollection<Layer>();
            UpdateLayers();
        }

        public ObservableCollection<Layer> Layers { get; set; } = new ObservableCollection<Layer>();

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
                }
            }
        }

        public List<string> Materials { get; set; } = new List<string>
        {
            "Медь",
            "Алюминий",
            "Базальтовое волокно",
            "Сталь"
        };

        private void UpdateLayers()
        {
            while (Layers.Count < LayerCount)
            {
                Layers.Add(new Layer { LayerNumber = Layers.Count + 1 });
            }

            while (Layers.Count > LayerCount)
            {
                Layers.RemoveAt(Layers.Count - 1);
            }

            // Обновить номера всех слоёв
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].LayerNumber = i + 1;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
