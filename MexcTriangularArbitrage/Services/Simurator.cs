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
        public double TotalProfitRatio => TotalConvertedUsdt / TotalBoughtUsdt;

        private double TotalBoughtUsdt { get; set; }
        private double TotalConvertedUsdt { get; set; }

        public Simurator() { }

        public void Execute()
        {
            const double targetUsdt = 20;
            const double targetRatio = 1.0001;

            var candidateGetter = new CandidateGetter();
            Console.WriteLine($"First,Second,Third,ProfitRatio,{nameof(TotalBoughtUsdt)},{nameof(TotalConvertedUsdt)},{nameof(TotalProfitRatio)}");
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var candidate in candidateGetter.Execute(targetUsdt, targetRatio))
                {
                    TotalBoughtUsdt += targetUsdt;
                    TotalConvertedUsdt += candidate.ProfitRatio * targetUsdt;
                    Console.WriteLine($"{candidate.SymbolTickerList[0].symbol},{candidate.SymbolTickerList[1].symbol},{candidate.SymbolTickerList[2].symbol},{candidate.ProfitRatio:F5},{TotalBoughtUsdt:F5},{TotalConvertedUsdt:F5},{TotalProfitRatio:F5}");
                }
            }
        }
    }
}
