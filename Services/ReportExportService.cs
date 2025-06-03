using STC.WPF.Models;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace STC.WPF.Services
{
    public static class ReportExportService
    {
        public static void ExportToTxt(CalculationInput input, CalculationOutput output, CalculationAlgorithm algorithm)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                FileName = "report.txt"
            };

            if (dialog.ShowDialog() != true)
                return;

            var sb = new StringBuilder();

            sb.AppendLine("=== ОТЧЁТ ПО РАСЧЁТУ ===\n");
            sb.AppendLine($"Количество слоёв: {input.Layers.Count}");
            sb.AppendLine($"Высота стенки: {input.WallHeight} м");
            sb.AppendLine($"T внутренняя: {input.TInner} °C");
            sb.AppendLine($"T внешняя: {input.TOuter} °C\n");

            sb.AppendLine("Слои:");
            for (int i = 0; i < input.Layers.Count; i++)
            {
                var l = input.Layers[i];
                sb.AppendLine($"  Слой {i + 1}: {l.SelectedMaterial.Name} — r внутр: {l.InnerRadius}, r внешн: {l.OuterRadius}");
            }

            sb.AppendLine("\nТемпературы на границах:");
            for (int i = 0; i < output.FinalTemperatures.Count; i++)
                sb.AppendLine($"  t{i + 1} = {output.FinalTemperatures[i]:F2} °C");

            sb.AppendLine("\nКоэффициенты теплопроводности:");
            for (int i = 0; i < algorithm.ThermalConductivities.Count; i++)
                sb.AppendLine($"  λ{i + 1} = {algorithm.ThermalConductivities[i]:F5} Вт/(м·К)");

            sb.AppendLine("\nЛокальные плотности теплового потока:");
            for (int i = 0; i < algorithm.LocalHeatFluxesDensities.Count; i++)
                sb.AppendLine($"  ql{i + 1} = {algorithm.LocalHeatFluxesDensities[i]:F2} Вт/м");

            sb.AppendLine($"\nИтоговый тепловой поток Q: {output.TotalHeatFlow:F3} Вт");

            File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        }
    }
}
