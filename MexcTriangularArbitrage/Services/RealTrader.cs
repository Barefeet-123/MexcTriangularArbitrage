using MexcTriangularArbitrage.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MexcTriangularArbitrage.Services
{
    public class RealTrader
    {
        readonly HashSet<SymbolData> _symbolsHashSet;

        public RealTrader()
        {
            _symbolsHashSet = QueryExecutor.GetAllTargetSymbols();
        }

        public double Execute()
        {
            const double targetUsdtQuantity = 20;
            const double targetRatio = 1.00001;

            var triangularArbitrageExecutor = new CandidateGetter();
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var candidate in triangularArbitrageExecutor.Execute(targetUsdtQuantity, targetRatio))
                {
                    Console.WriteLine(candidate);
                    try
                    {
                        var symbolTickerList = candidate.SymbolTickerList;
                        var marketDepthList = candidate.MarketDepthList;
                        var currencyQuantity = targetUsdtQuantity;
                        currencyQuantity = Bid(symbolTickerList[0], marketDepthList[0], currencyQuantity);
                        currencyQuantity = Bid(symbolTickerList[1], marketDepthList[1], currencyQuantity);
                        currencyQuantity = Ask(symbolTickerList[2], marketDepthList[2], currencyQuantity);
                    }
                    catch (Exception ex)
                    {
                        Utils.ErrorLog(ex);
                    }
                }
            }
        }

        private double Bid(SymbolTicker symbolTicker, MarketDepth marketDepth, double keyCurrencyQuantity)
        {
            var totalQuantity = 0.0;
            var restAmount = keyCurrencyQuantity;
            var minAmount = _symbolsHashSet.FirstOrDefault(_ => _.symbol == symbolTicker.symbol).MinAmountAsDouble;
            foreach (var depthData in marketDepth.asks)
            {
                if (restAmount <= minAmount)
                {
                    break;
                }

                double buyableQuantity = restAmount / depthData.PriceAsDouble;

                var postData = new PlaceOrderPostData
                {
                    symbol = symbolTicker.symbol,
                    price = depthData.price,
                    quantity = buyableQuantity.ToString(),
                    trade_type = "BID",
                    order_type = "IMMEDIATE_OR_CANCEL",
                };

                var orderId = QueryExecutor.PostPlaceOrder(postData);
                
                if(orderId == null)
                {
                    continue;
                }

                var dealHistoryList = QueryExecutor.GetDealHistory(symbolTicker.symbol);
                var dealHistory = dealHistoryList.FirstOrDefault(_ => _.order_id == orderId);

                if(dealHistory == null)
                {
                    continue;
                }

                var actualBoughtQuantity = dealHistory.QuantityAsDouble;
                totalQuantity += actualBoughtQuantity;

                restAmount = (buyableQuantity - actualBoughtQuantity) * depthData.PriceAsDouble;
            }

            return totalQuantity;
        }

        private double Ask(SymbolTicker symbolTicker, MarketDepth marketDepth, double settlementCurrencyQuantity)
        {
            var totalAmount = 0.0;
            var minAmount = _symbolsHashSet.FirstOrDefault(_ => _.symbol == symbolTicker.symbol).MinAmountAsDouble;
            var restSettlementQuantity = settlementCurrencyQuantity;
            foreach (var depthData in marketDepth.bids)
            {   
                if (restSettlementQuantity * depthData.PriceAsDouble <= minAmount)
                {
                    break;
                }

                double sellableQuantity = Math.Min(restSettlementQuantity, depthData.QuantityAsDouble / depthData.PriceAsDouble); ;

                var postData = new PlaceOrderPostData
                {
                    symbol = symbolTicker.symbol,
                    price = depthData.price,
                    quantity = sellableQuantity.ToString(),
                    trade_type = "ASK",
                    order_type = "IMMEDIATE_OR_CANCEL",
                };

                var orderId = QueryExecutor.PostPlaceOrder(postData);
                
                if (orderId == null)
                {
                    continue;
                }

                var dealHistoryList = QueryExecutor.GetDealHistory(symbolTicker.symbol);
                var dealHistory = dealHistoryList.FirstOrDefault(_ => _.order_id == orderId);

                if (dealHistory == null)
                {
                    continue;
                }

                totalAmount += dealHistory.AmountAsDouble;
                restSettlementQuantity -= dealHistory.AmountAsDouble;
            }
            return totalAmount;
        }
    }
}
