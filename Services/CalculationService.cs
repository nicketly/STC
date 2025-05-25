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
            var sb = new StringBuilder();

            sb.AppendLine($"Количество слоёв: {input.Layers.Count}");
            sb.AppendLine($"Высота стенки: {input.WallHeight} м");
            sb.AppendLine();

            // Радиусы
            sb.AppendLine("Радиусы на границах слоёв:");
            int rIndex = 1;
            sb.AppendLine($"r{rIndex++} = {input.Layers.First().InnerRadius} м");
            foreach (var layer in input.Layers)
            {
                sb.AppendLine($"r{rIndex++} = {layer.OuterRadius} м");
            }

            sb.AppendLine();

            // Температуры
            sb.AppendLine("Температуры на границах слоёв:");
            int tIndex = 1;
            sb.AppendLine($"t{tIndex++} = {input.TInner} °C");
            for (int i = 1; i < input.Layers.Count; i++)
            {
                sb.AppendLine($"t{tIndex++} = ?");
            }
            sb.AppendLine($"t{tIndex} = {input.TOuter} °C");

            sb.AppendLine();

            // Материалы
            sb.AppendLine("Материалы слоёв:");
            for (int i = 0; i < input.Layers.Count; i++)
            {
                var mat = input.Layers[i].SelectedMaterial;
                sb.AppendLine($"Слой {i + 1}: {mat.Name}, Класс: {mat.Category}, Tмакс = {mat.MaxTemp} °C, λ(T) = {mat.TCF}");
            }

            return sb.ToString();
        }

    }
}
