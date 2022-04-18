using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MexcTriangularArbitrage.Services
{
    public class Simurator
    {
        public double TotalProfitRatio => CurrentUsdt / StartUsdt;
        private double StartUsdt { get; }
        private double CurrentUsdt { get; set; } 

        public Simurator()
        {
            CurrentUsdt = StartUsdt = 10;
        }

        public void Execute()
        {
            var executor = new TrianglularArbitrageExecutor();
            while (true)
            {
                Thread.Sleep(5000);
                foreach (var c in executor.GetCandidates(CurrentUsdt, 1))
                {
                    Console.WriteLine(c);
                    AddProfit((c.ProfitRatio - 1) * CurrentUsdt);
                    Console.WriteLine($"TotalProfitRatio = {TotalProfitRatio}");
                }
            }
        }

        public void AddProfit(double profitUsdt)
        {
            CurrentUsdt += profitUsdt;
        }
    }
}
