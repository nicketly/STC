using STC.WPF.Services;
using STC.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STC.WPF.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void ButtonTxt_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ReportViewModel vm)
            {
                ReportExportService.ExportToTxt(vm.Input, vm.Output, vm.Algorithm);
            }
        }

    }
}
