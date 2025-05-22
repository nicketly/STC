using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace STC.WPF.Models
{
    public class Material : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _name;
        private string _category;
        private double _maxTemp;
        private string _tcf;

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
        public double MaxTemp { get => _maxTemp; set { _maxTemp = value; OnPropertyChanged(); } }
        public string TCF { get => _tcf; set { _tcf = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }       
    }
}
