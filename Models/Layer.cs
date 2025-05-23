using STC.WPF.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STC.WPF.Models
{
    public class Layer : INotifyPropertyChanged
    {
        private int _layerNumber;
        public int LayerNumber
        {
            get => _layerNumber;
            set
            {
                _layerNumber = value;
                OnPropertyChanged(nameof(LayerNumber));
            }
        }

        private double _innerRadius;
        public double InnerRadius
        {
            get => _innerRadius;
            set
            {
                if (value >= 0)
                {
                    _innerRadius = value;
                    OnPropertyChanged(nameof(InnerRadius));
                }
            }
        }

        private double _outerRadius;
        public double OuterRadius
        {
            get => _outerRadius;
            set
            {
                if (value >= 0)
                {
                    _outerRadius = value;
                    OnPropertyChanged(nameof(OuterRadius));
                }
            }
        }

        private Material _selectedMaterial;
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
            
    }
}
