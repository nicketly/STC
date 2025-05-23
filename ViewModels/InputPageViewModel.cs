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
            Layers = new ObservableCollection<LayerViewModel>();
            UpdateLayers();
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
                }
            }
        }

        //public List<string> Materials { get; set; } = new List<string>
        //{
        //    "Медь",
        //    "Алюминий",
        //    "Базальтовое волокно",
        //    "Сталь"
        //};

        private double _wallHeight;
        public double WallHeight
        {
            get => _wallHeight;
            set
            {
                if (value >= 0)
                {
                    _wallHeight = value;
                    OnPropertyChanged(nameof(WallHeight));
                }
            }
        }

        public double TInner { get; set; }
        public double TOuter { get; set; }
        public bool ValidateInput(out string errorMessage)
        {
            if (WallHeight <= 0)
            {
                errorMessage = "Высота стенки должна быть положительным числом";
                return false;
            }
            errorMessage = null;
            return true;
        }
        private void UpdateLayers()
        {
            while (Layers.Count < LayerCount)
            {
                var layer = new Layer { LayerNumber = Layers.Count + 1 };
                Layers.Add(new LayerViewModel { Layer = layer });
            }

            while (Layers.Count > LayerCount)
            {
                Layers.RemoveAt(Layers.Count - 1);
            }

            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].Layer.LayerNumber = i + 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
