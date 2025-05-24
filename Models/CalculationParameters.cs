using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STC.WPF.Models
{
    //INPUT
    public class CalculationInput
    {
        public double WallHeight { get; set; }                                    // l
        public double TInner { get; set; }                                        // t1
        public double TOuter { get; set; }                                        // t[n+1]
        public List<Layer> Layers { get; set; }                                   // Layer Parameters 
    }

    //ALGORITHM
    public class CalculationAlgorithm
    { 
        public CalculationAlgorithm(CalculationOutput output)
        {
            Epsilon = 0.01 * output.HeatFluxDensity;
        }
        public List<double> InitialTemperatures { get; set; } = new();            // t2..tn
        public List<double> LayerMeanTemperatures { get; set; } = new();          // T1..Tn
        public List<double> ThermalConductivities { get; set; } = new();          // λn(T)
        public List<double> ThermalResistances { get; set; } = new();             // R1..Rn
        public List<double> LocalHeatFluxesDensities { get; set; } = new();       // ql1..qln
        public List<double> Deviations { get; set; } = new();                     // Δql1..Δqln
        public double Error { get; set; } = new();                                // Error Function to minimize
        public double Epsilon { get; private set; }                               // Accuracy
        public int MaxIterations { get; set; } = 100;                             // Max Iterations

    }

    //OUTPUT
    public class CalculationOutput
    {
        public List<double> FinalTemperatures { get; set; } = new();
        public double HeatFluxDensity { get; set; }
        public double TotalHeatFlow { get; set; }
        public bool Converged { get; set; }
        public int Iterations { get; set; }
    }
       
}
