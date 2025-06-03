using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC.WPF.ViewModels
{
    public class ViewModelLocator
    {
        public static ReportViewModel ReportVM { get; } = new ReportViewModel();
    }
}
