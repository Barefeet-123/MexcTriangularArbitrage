using MexcTriangularArbitrage.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace MexcTriangularArbitrage
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(GlobalSetting.TokenConfigPath))
            {
                ConfigService.CreateTokenConfig();
            }
            var tokenConfig = ConfigService.ReadTokenConfig();
            GlobalSetting.TokenConfig = tokenConfig;

            var allSymbols = QueryExecutor.GetAllSymbols();
            var targetSymbolHashSet = allSymbols
                .Where(_ => _.state == "ENABLED" && _.etf_mark == 0)
                .ToHashSet();

            var baseUsdt = 30;
            var executor = new TrianglularArbitrageExecutor();
            var purchaser = new Purchaser(targetSymbolHashSet);

            while (true)
            {
                Thread.Sleep(10000);
                foreach (var c in executor.GetCandidates(baseUsdt, 1.00001))
                {
                    Console.WriteLine(c);
                    var afterUsdt = purchaser.ExecuteTriangleArbitrage(c.SymbolTickerList, c.MarketDepthList, baseUsdt);
                }
            }
        }
    }
}
