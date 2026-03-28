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
        public double TotalProfitRatio => TotalBoughtUsdtQuantity == 0 ? 0 : TotalConvertedUsdtQuantity / TotalBoughtUsdtQuantity;

        private double TotalBoughtUsdtQuantity { get; set; }
        private double TotalConvertedUsdtQuantity { get; set; }

        public Simulator() { }

        public void Execute(CancellationToken cancellationToken = default)
        {
            var targetUsdtQuantity = GlobalSetting.TradeConfig.TargetUsdtQuantity;
            var targetRatio = GlobalSetting.TradeConfig.SimulationTargetRatio;

            var candidateGetter = new CandidateGetter();
            Console.WriteLine($"First,Second,Third,ProfitRatio,{nameof(TotalBoughtUsdtQuantity)},{nameof(TotalConvertedUsdtQuantity)},{nameof(TotalProfitRatio)}");
            while (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(1000, cancellationToken).Wait(cancellationToken);
                if (cancellationToken.IsCancellationRequested) break;
                foreach (var candidate in candidateGetter.Execute(targetUsdtQuantity, targetRatio))
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    TotalBoughtUsdtQuantity += targetUsdtQuantity;
                    TotalConvertedUsdtQuantity += candidate.ProfitRatio * targetUsdtQuantity;
                    Console.WriteLine($"{candidate.SymbolTickerList[0].symbol},{candidate.SymbolTickerList[1].symbol},{candidate.SymbolTickerList[2].symbol},{candidate.ProfitRatio:F5},{TotalBoughtUsdtQuantity:F5},{TotalConvertedUsdtQuantity:F5},{TotalProfitRatio:F5}");
                }
            }
        }
    }
}
