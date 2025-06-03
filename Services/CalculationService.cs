using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STC.WPF.Models;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using STC.WPF.ViewModels;

namespace STC.WPF.Services
{
    public static class CalculationService
    {
        public static void Calculate(CalculationInput input, CalculationAlgorithm algorithm, CalculationOutput output)
        {
            // 1. Начальные приближения температур (t1 ... tn+1)
            algorithm.InitialTemperatures.Clear();
            for (int i = 0; i <= input.Layers.Count; i++)
            {
                double t = input.TInner + i * (input.TOuter - input.TInner) / input.Layers.Count;
                algorithm.InitialTemperatures.Add(t);
            }

            int n = input.Layers.Count;
            output.Converged = false;
            double delta = 0.5;
            double minDelta = 1e-4;
            double bestError = double.MaxValue;
            int iteration = 0;

            algorithm.Epsilon = 0.01 * (2 * Math.PI * (input.TInner - input.TOuter) / input.Layers.Count); // предварительно, уточнится позже

            while (delta >= minDelta && iteration < algorithm.MaxIterations)
            {
                bool improved = false;

                for (int i = 1; i < algorithm.InitialTemperatures.Count - 1; i++)
                {
                    double original = algorithm.InitialTemperatures[i];
                    double bestLocal = original;
                    double localBestError = double.MaxValue;

                    foreach (var d in new[] { -delta, 0.0, delta })
                    {
                        algorithm.InitialTemperatures[i] = original + d;
                        Recalculate(input, algorithm, output);

                        if (algorithm.Error < localBestError)
                        {
                            localBestError = algorithm.Error;
                            bestLocal = algorithm.InitialTemperatures[i];
                        }
                    }

                    if (Math.Abs(bestLocal - original) > 1e-8)
                    {
                        improved = true;
                    }

                    algorithm.InitialTemperatures[i] = bestLocal;
                }

                iteration++;

                Recalculate(input, algorithm, output);

                if (algorithm.Error < bestError)
                {
                    bestError = algorithm.Error;
                }

                if (algorithm.Deviations.All(d => d < algorithm.Epsilon))
                {
                    output.Converged = true;
                    output.Iterations = iteration;
                    break;
                }

                if (!improved)
                {
                    delta *= 0.1; // уменьшаем шаг, если не улучшилось
                }
            }

            // Финальный пересчёт по откорректированным температурам
            Recalculate(input, algorithm, output);

            output.FinalTemperatures = new List<double>(algorithm.InitialTemperatures);
            output.TotalHeatFlow = output.HeatFluxDensity * input.WallHeight;
        }

        private static void Recalculate(CalculationInput input, CalculationAlgorithm algorithm, CalculationOutput output)
        {
            int n = input.Layers.Count;

            algorithm.LayerMeanTemperatures = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double avg = (algorithm.InitialTemperatures[i] + algorithm.InitialTemperatures[i + 1]) / 2;
                algorithm.LayerMeanTemperatures.Add(avg);
            }

            algorithm.ThermalConductivities = new List<double>();
            foreach (var (layer, avgTemp) in input.Layers.Zip(algorithm.LayerMeanTemperatures, (l, t) => (l, t)))
            {
                double lambda = EvaluateLambda(layer.SelectedMaterial.TCF, avgTemp);
                if (lambda <= 0) lambda = 1.0;
                algorithm.ThermalConductivities.Add(lambda);
            }

            algorithm.ThermalResistances = new List<double>();
            for (int i = 0; i < n; i++)
            {
                var l = input.Layers[i];
                var lambda = algorithm.ThermalConductivities[i];
                double R = (1 / lambda) * Math.Log(l.OuterRadius / l.InnerRadius);
                algorithm.ThermalResistances.Add(R);
            }

            algorithm.LocalHeatFluxesDensities = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double q = 2 * Math.PI * (algorithm.InitialTemperatures[i] - algorithm.InitialTemperatures[i + 1]) / algorithm.ThermalResistances[i];
                algorithm.LocalHeatFluxesDensities.Add(q);
            }

            double totalR = algorithm.ThermalResistances.Sum();
            output.HeatFluxDensity = 2 * Math.PI * (input.TInner - input.TOuter) / totalR;

            algorithm.Deviations = algorithm.LocalHeatFluxesDensities
                .Select(q => Math.Abs(q - output.HeatFluxDensity))
                .ToList();

            algorithm.Error = algorithm.Deviations.Sum(d => d * d);
        }

        private static double EvaluateLambda(string tcf, double temp)
        {
            try
            {
                string expr = tcf.Replace("T", temp.ToString(CultureInfo.InvariantCulture));
                var dt = new DataTable();
                var result = dt.Compute(expr, "");
                return Convert.ToDouble(result);
            }
            catch
            {
                return 1.0;
            }
        }
    }
}
