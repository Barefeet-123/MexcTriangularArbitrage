using MexcTriangularArbitrage.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MexcTriangularArbitrage.Services
{
    public class Purchaser
    {
        HashSet<SymbolData> _symbolsHashSet;

        public Purchaser(HashSet<SymbolData> symbolsHashSet)
        {
            _symbolsHashSet = symbolsHashSet;
        }

        public double ExecuteTriangleArbitrage(IReadOnlyList<SymbolTicker> symbolTickerList, IReadOnlyList<MarketDepth> marketDepthList, double usdt)
        {
            try
            {
                var currencyNum = usdt;
                currencyNum = Bid(symbolTickerList[0], marketDepthList[0], currencyNum);
                currencyNum = Bid(symbolTickerList[1], marketDepthList[1], currencyNum);
                currencyNum = Ask(symbolTickerList[2], marketDepthList[2], currencyNum);
                return currencyNum;
            }
            catch(Exception ex)
            {
                Utils.ErrorLog(ex);
                return 0;
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

                double sellableQuantity = Math.Min(restSettlementQuantity, depthData.QuantityAsDouble);

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
                //var actualSoldQuantity = dealHistory.QuantityAsDouble;
                restSettlementQuantity -= dealHistory.QuantityAsDouble;
            }
            return totalAmount;
        }
    }
}
