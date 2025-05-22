// ReferenceViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using STC.WPF.Models;
using STC.WPF.Database;

namespace STC.WPF.ViewModels
{
    public class ReferenceViewModel : INotifyPropertyChanged
    {
        private bool _isTableVisible = true;
        private string _toggleTableButtonText = "Скрыть таблицу";

        public ObservableCollection<Material> Materials { get; set; }
        private Material _selectedMaterial;
        public Material SelectedMaterial 
        { 
            get => _selectedMaterial; 
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged();
            }
        }

        public string ToggleTableButtonText
        {
            get => _toggleTableButtonText;
            set { _toggleTableButtonText = value; OnPropertyChanged(); }
        }

        public bool IsTableVisible
        {
            get => _isTableVisible;
            set { _isTableVisible = value; OnPropertyChanged(); }
        }

        public ICommand AddMaterialCommand { get; }
        public ICommand DeleteMaterialCommand { get; }
        public ICommand SaveMaterialsCommand { get; }
        public ICommand ToggleTableVisibilityCommand { get; }

        public ReferenceViewModel()
        {
            DatabaseInitializer.EnsureDatabaseCreated();

            Materials = new ObservableCollection<Material>(DatabaseInitializer.LoadMaterials());
            SelectedMaterial = Materials.FirstOrDefault();

            AddMaterialCommand = new RelayCommand(AddMaterial);
            DeleteMaterialCommand = new RelayCommand(DeleteMaterial, CanDelete);
            SaveMaterialsCommand = new RelayCommand(SaveMaterials);
            ToggleTableVisibilityCommand = new RelayCommand(ToggleTableVisibility);
        }

        private void AddMaterial()
        {
            var newMaterial = new Material { Name = "Новый", Category = "Класс", MaxTemp = 0, TCF = "a+B*T" };
            Materials.Add(newMaterial);
            SelectedMaterial = newMaterial;
        }

        private void DeleteMaterial()
        {
            if (SelectedMaterial != null)
                Materials.Remove(SelectedMaterial);
        }

        private bool CanDelete()
        {
            return SelectedMaterial != null;
        }

        private void SaveMaterials()
        {
            DatabaseInitializer.SaveMaterials(Materials.ToList());
            MessageBox.Show("Материалы сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ToggleTableVisibility()
        {
            IsTableVisible = !IsTableVisible;
            ToggleTableButtonText = IsTableVisible ? "Скрыть таблицу" : "Показать таблицу";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
