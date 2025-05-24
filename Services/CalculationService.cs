using STC.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC.WPF.Services
{
    public static class CalculationService
    {
        public static string Calculate(CalculationInput input)
        {
            return $"Всего слоёв: {input.Layers.Count}";
        }
    }
}
