using System;
using System.Collections.Generic;

namespace MexcTriangularArbitrage.Schema
{
    public class TheoricalTriangularArbitrageResult
    {
        public IReadOnlyList<SymbolTicker> SymbolTickerList { get; } 
        public IReadOnlyList<MarketDepth> MarketDepthList { get; }
        public double ProfitRatio { get; }
        public double NumOfUsdt { get; }

        public TheoricalTriangularArbitrageResult(IReadOnlyList<SymbolTicker> symbolTickerList, IReadOnlyList<MarketDepth> marketDepthList, double profitRatio, double numOfUsdt)
        {
            SymbolTickerList = symbolTickerList;
            MarketDepthList = marketDepthList;
            ProfitRatio = profitRatio;
            NumOfUsdt = numOfUsdt;
        }

        public override string ToString()
        {
            var first = SymbolTickerList[0];
            var second = SymbolTickerList[1];
            var third = SymbolTickerList[2];

            var result = $@"
Time: {DateTime.Now}
    First[{first}]
    Second[{second}]
    Third[{third}]
    ProfitRatio[{ProfitRatio}]";
            return result;
        }
    }
}
