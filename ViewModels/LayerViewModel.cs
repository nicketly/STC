using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using STC.WPF.Models;
using STC.WPF.Database;

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
            Layer = new Layer();

            GroupedMaterialsView = new CollectionViewSource
            {
                Source = Materials
            };
            GroupedMaterialsView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public ICollectionView GroupedMaterials => GroupedMaterialsView.View;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
