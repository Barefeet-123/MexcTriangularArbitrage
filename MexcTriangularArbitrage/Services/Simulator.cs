using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MexcTriangularArbitrage.Services
{
    public class Simulator
    {
        public double TotalProfitRatio => TotalConvertedUsdtQuantity / TotalBoughtUsdtQuantity;

        private double TotalBoughtUsdtQuantity { get; set; }
        private double TotalConvertedUsdtQuantity { get; set; }

        public Simulator() { }

        public void Execute()
        {
            var targetUsdtQuantity = GlobalSetting.TradeConfig.TargetUsdtQuantity;
            var targetRatio = GlobalSetting.TradeConfig.SimulationTargetRatio;

            var candidateGetter = new CandidateGetter();
            Console.WriteLine($"First,Second,Third,ProfitRatio,{nameof(TotalBoughtUsdtQuantity)},{nameof(TotalConvertedUsdtQuantity)},{nameof(TotalProfitRatio)}");
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var candidate in candidateGetter.Execute(targetUsdtQuantity, targetRatio))
                {
                    TotalBoughtUsdtQuantity += targetUsdtQuantity;
                    TotalConvertedUsdtQuantity += candidate.ProfitRatio * targetUsdtQuantity;
                    Console.WriteLine($"{candidate.SymbolTickerList[0].symbol},{candidate.SymbolTickerList[1].symbol},{candidate.SymbolTickerList[2].symbol},{candidate.ProfitRatio:F5},{TotalBoughtUsdtQuantity:F5},{TotalConvertedUsdtQuantity:F5},{TotalProfitRatio:F5}");
                }
            }
        }
    }
}
