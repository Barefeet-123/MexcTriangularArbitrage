using MexcTriangularArbitrage.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MexcTriangularArbitrage.Services
{
    public class CandidateGetter
    {
        private static readonly HashSet<string> _startKeyCurrencyHashSet = new() { "ETH", "BTC" };
        private const string _startSttlementCurrency = "USDT";
        private readonly HashSet<string> _targetSymbolHashSet;

        public CandidateGetter()
        {
            _targetSymbolHashSet = QueryExecutor.GetAllTargetSymbols()
                .Select(_ => _.symbol)
                .ToHashSet();
        }

        public IEnumerable<TheoricalTriangularArbitrageResult> Execute(double numOfUsdt, double minRatio = 1)
        {
            var symbolTickerInfoList = QueryExecutor.GetSymbolTickerInformation();

            var targetSymbolTickerInfoHash = symbolTickerInfoList
                .Where(_ => _targetSymbolHashSet.Contains(_.symbol))
                .Where(_ => _startKeyCurrencyHashSet.Contains(_.SettlementCurrency))
                .ToHashSet();

            var keyCurrencyForUsdtHashSet = targetSymbolTickerInfoHash
                .Select(_ => _.KeyCurrency)
                .Concat(_startKeyCurrencyHashSet)
                .ToHashSet();

            var usdtSettledSymbolTickerHashSet = symbolTickerInfoList
                .Where(_ => keyCurrencyForUsdtHashSet.Contains(_.KeyCurrency) && _.SettlementCurrency == _startSttlementCurrency)
                .ToHashSet();

            foreach (var startKeyCurrency in _startKeyCurrencyHashSet)
            {
                var first = usdtSettledSymbolTickerHashSet.FirstOrDefault(_ => _.KeyCurrency == startKeyCurrency);
                
                if (first == null)
                {
                    continue;
                }

                foreach (var second in targetSymbolTickerInfoHash.Where(_ => _.SettlementCurrency == startKeyCurrency))
                {
                    var third = usdtSettledSymbolTickerHashSet.FirstOrDefault(_ => _.KeyCurrency == second.KeyCurrency);
                    
                    if (third == null)
                    {
                        continue;
                    }

                    var symbolTickerList = new List<SymbolTicker> { first, second, third };

                    if (GetProfitRatio(symbolTickerList) < minRatio)
                    {
                        continue;
                    }

                    List<MarketDepth> marketDepthList;

                    try
                    {
                        marketDepthList = symbolTickerList.Select(_ => QueryExecutor.GetMarketDepth(_.symbol)).ToList();
                    }
                    catch (Exception ex)
                    {
                        Utils.ErrorLog(ex);
                        continue;
                    }

                    var profitRatio = GetProfitRatio(marketDepthList, numOfUsdt);

                    if (profitRatio < minRatio)
                    {
                        continue;
                    }

                    yield return new TheoricalTriangularArbitrageResult(symbolTickerList, marketDepthList, profitRatio, numOfUsdt);
                }
            }
        }

        /// <summary>
        /// シンボル情報のみから、三角取引の結果を取得する。
        /// 手数料はMXで支払うと0.16%になるのでデフォルトではその値を使う。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="feeRatio"></param>
        /// <returns></returns>
        public static double GetProfitRatio(List<SymbolTicker> symbolTickerList, double feeRatio = 0.0016)
        {
            if (symbolTickerList.Count != 3)
            {
                throw new ArgumentException($"{nameof(symbolTickerList)} must contain 3 elements.");
            }

            double currencyQuantity = 1.0; //USDTの枚数
            currencyQuantity = (currencyQuantity / symbolTickerList[0].AskAsDouble) * (1.0 - feeRatio);  //二つ目の通貨の枚数: A/USDT でAの枚数を求めるので買う
            currencyQuantity = (currencyQuantity / symbolTickerList[1].AskAsDouble) * (1.0 - feeRatio);  //三つ目の通貨の枚数: B/A   でBの枚数を求めるので買う
            currencyQuantity = (symbolTickerList[2].BidAsDouble * currencyQuantity) * (1.0 - feeRatio);  //USDTに戻った時の枚数: B/USDT でUSDTの枚数を求めるので売る
            return currencyQuantity;
        }

        /// <summary>
        /// 市場深度を用いてより正確な三角取引の結果を取得する。
        /// 手数料はMXで支払うと0.16%になるのでデフォルトではその値を使う。
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="feeRatio"></param>
        /// <param name="usdtQuantity"></param>
        /// <returns></returns>
        private static double GetProfitRatio(List<MarketDepth> marketDepthList, double usdtQuantity, double feeRatio = 0.0016)
        {
            if (usdtQuantity == 0)
            {
                return 0;
            }

            if (marketDepthList.Count != 3)
            {
                throw new ArgumentException($"{nameof(marketDepthList)} must contain 3 elements.");
            }

            double currencyQuantity = usdtQuantity; //USDTの枚数
            currencyQuantity = GetActualBidQuantity(marketDepthList[0], currencyQuantity) * (1.0 - feeRatio); ;  //二つ目の通貨の枚数: A/USDT でAの枚数を求めるので買う
            currencyQuantity = GetActualBidQuantity(marketDepthList[1], currencyQuantity) * (1.0 - feeRatio); ; //三つ目の通貨の枚数: B/A   でBの枚数を求めるので買う
            currencyQuantity = GetActualAskQuantity(marketDepthList[2], currencyQuantity) * (1.0 - feeRatio); ;     //USDTに戻った時の枚数: B/USDT でUSDTの枚数を求めるので売る
            return currencyQuantity / usdtQuantity;
        }

        private static double GetActualBidQuantity(MarketDepth marketDepth, double keyCurrencyQuantity)
        {
            var restQuantity = keyCurrencyQuantity;
            var totalConvertedQuantity = 0.0;
            foreach (var depthData in marketDepth.asks)
            {
                if (restQuantity <= 0)
                {
                    break;
                }

                double buyable = restQuantity / depthData.PriceAsDouble;
                double bought = Math.Min(buyable, depthData.QuantityAsDouble);
                totalConvertedQuantity += bought;
                restQuantity = (buyable - bought) * depthData.PriceAsDouble;
            }
            return totalConvertedQuantity;
        }

        private static double GetActualAskQuantity(MarketDepth marketDepth, double settlementCurrencyQuantity)
        {
            var restQuantity = settlementCurrencyQuantity;
            var totalConvertedQuantity = 0.0;
            foreach (var depthData in marketDepth.bids)
            {
                if (restQuantity <= 0)
                {
                    break;
                }

                double sellable = restQuantity;
                double sold = Math.Min(sellable, depthData.QuantityAsDouble);
                totalConvertedQuantity += sold * depthData.PriceAsDouble;
                restQuantity = sellable - sold;
            }
            return totalConvertedQuantity;
        }
    }
}
