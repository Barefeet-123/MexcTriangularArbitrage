using System;
using System.Collections.Generic;
using MexcTriangularArbitrage.Schema;
using MexcTriangularArbitrage.Services;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Services
{
    public class CandidateGetterTests
    {
        // ---- GetProfitRatio(List<SymbolTicker>) ----

        [Fact]
        public void GetProfitRatio_SymbolTicker_BreakEvenWithNoFee_ReturnsOne()
        {
            // ask0=100, ask1=0.5, bid2=50, fee=0 → 1/100/0.5*50 = 1.0
            var tickers = new List<SymbolTicker>
            {
                new() { ask = "100", bid = "99" },
                new() { ask = "0.5", bid = "0.49" },
                new() { ask = "51",  bid = "50" },
            };

            var result = CandidateGetter.GetProfitRatio(tickers, feeRatio: 0.0);

            Assert.Equal(1.0, result, precision: 10);
        }

        [Fact]
        public void GetProfitRatio_SymbolTicker_ProfitablePath_ReturnsAboveOne()
        {
            // bid2=55 なので利益あり (fee=0): 1/100/0.5*55 = 1.1
            var tickers = new List<SymbolTicker>
            {
                new() { ask = "100", bid = "99" },
                new() { ask = "0.5", bid = "0.49" },
                new() { ask = "56",  bid = "55" },
            };

            var result = CandidateGetter.GetProfitRatio(tickers, feeRatio: 0.0);

            Assert.Equal(1.1, result, precision: 10);
        }

        [Fact]
        public void GetProfitRatio_SymbolTicker_UnprofitablePath_ReturnsBelowOne()
        {
            // bid2=45 なので損失 (fee=0): 1/100/0.5*45 = 0.9
            var tickers = new List<SymbolTicker>
            {
                new() { ask = "100", bid = "99" },
                new() { ask = "0.5", bid = "0.49" },
                new() { ask = "46",  bid = "45" },
            };

            var result = CandidateGetter.GetProfitRatio(tickers, feeRatio: 0.0);

            Assert.Equal(0.9, result, precision: 10);
        }

        [Fact]
        public void GetProfitRatio_SymbolTicker_FeeReducesProfit()
        {
            // fee あり → fee なしより小さい値になる
            var tickers = new List<SymbolTicker>
            {
                new() { ask = "100", bid = "99" },
                new() { ask = "0.5", bid = "0.49" },
                new() { ask = "51",  bid = "50" },
            };

            var withFee    = CandidateGetter.GetProfitRatio(tickers, feeRatio: 0.0016);
            var withoutFee = CandidateGetter.GetProfitRatio(tickers, feeRatio: 0.0);

            Assert.True(withFee < withoutFee);
        }

        [Fact]
        public void GetProfitRatio_SymbolTicker_WrongCount_ThrowsArgumentException()
        {
            var tickers = new List<SymbolTicker>
            {
                new() { ask = "100", bid = "99" },
                new() { ask = "0.5", bid = "0.49" },
            };

            Assert.Throws<ArgumentException>(() => CandidateGetter.GetProfitRatio(tickers));
        }

        // ---- GetActualBidQuantity (internal) ----

        [Fact]
        public void GetActualBidQuantity_SingleAskWithEnoughLiquidity_ReturnsExactQuantity()
        {
            // 100 USDT, ask price=10, qty=10 → buy 10 units
            var depth = new MarketDepth
            {
                asks = new List<DepthData>
                {
                    new() { price = "10", quantity = "10" }
                },
                bids = new List<DepthData>()
            };

            var result = CandidateGetter.GetActualBidQuantity(depth, 100.0);

            Assert.Equal(10.0, result, precision: 10);
        }

        [Fact]
        public void GetActualBidQuantity_MultipleAsks_ConsumesBothLevels()
        {
            // 100 USDT: ask1 price=10 qty=5 → buy 5, rest=50; ask2 price=11 qty=20 → buy 50/11
            var depth = new MarketDepth
            {
                asks = new List<DepthData>
                {
                    new() { price = "10", quantity = "5"  },
                    new() { price = "11", quantity = "20" },
                },
                bids = new List<DepthData>()
            };

            var result = CandidateGetter.GetActualBidQuantity(depth, 100.0);

            // 5 + 50/11
            double expected = 5.0 + 50.0 / 11.0;
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void GetActualBidQuantity_ZeroQuantity_ReturnsZero()
        {
            var depth = new MarketDepth
            {
                asks = new List<DepthData>
                {
                    new() { price = "10", quantity = "5" }
                },
                bids = new List<DepthData>()
            };

            var result = CandidateGetter.GetActualBidQuantity(depth, 0.0);

            Assert.Equal(0.0, result, precision: 10);
        }

        [Fact]
        public void GetActualBidQuantity_EmptyAsks_ReturnsZero()
        {
            var depth = new MarketDepth
            {
                asks = new List<DepthData>(),
                bids = new List<DepthData>()
            };

            var result = CandidateGetter.GetActualBidQuantity(depth, 100.0);

            Assert.Equal(0.0, result, precision: 10);
        }

        // ---- GetActualAskQuantity (internal) ----

        [Fact]
        public void GetActualAskQuantity_SingleBidWithEnoughLiquidity_ReturnsExactAmount()
        {
            // 10 ETH を売る, bid price=2000, qty=20 → 10*2000=20000 USDT
            var depth = new MarketDepth
            {
                asks = new List<DepthData>(),
                bids = new List<DepthData>
                {
                    new() { price = "2000", quantity = "20" }
                }
            };

            var result = CandidateGetter.GetActualAskQuantity(depth, 10.0);

            Assert.Equal(20000.0, result, precision: 10);
        }

        [Fact]
        public void GetActualAskQuantity_MultipleBids_ConsumesBothLevels()
        {
            // 10 ETH: bid1 price=2000 qty=3 → sell 3 (6000); bid2 price=1999 qty=10 → sell 7 (13993)
            var depth = new MarketDepth
            {
                asks = new List<DepthData>(),
                bids = new List<DepthData>
                {
                    new() { price = "2000", quantity = "3"  },
                    new() { price = "1999", quantity = "10" },
                }
            };

            var result = CandidateGetter.GetActualAskQuantity(depth, 10.0);

            Assert.Equal(19993.0, result, precision: 10);
        }

        [Fact]
        public void GetActualAskQuantity_ZeroQuantity_ReturnsZero()
        {
            var depth = new MarketDepth
            {
                asks = new List<DepthData>(),
                bids = new List<DepthData>
                {
                    new() { price = "2000", quantity = "10" }
                }
            };

            var result = CandidateGetter.GetActualAskQuantity(depth, 0.0);

            Assert.Equal(0.0, result, precision: 10);
        }

        [Fact]
        public void GetActualAskQuantity_EmptyBids_ReturnsZero()
        {
            var depth = new MarketDepth
            {
                asks = new List<DepthData>(),
                bids = new List<DepthData>()
            };

            var result = CandidateGetter.GetActualAskQuantity(depth, 10.0);

            Assert.Equal(0.0, result, precision: 10);
        }

        // ---- GetProfitRatio(List<MarketDepth>) (internal) ----

        [Fact]
        public void GetProfitRatio_MarketDepth_BreakEvenWithNoFee_ReturnsOne()
        {
            // 100 USDT → buy 10 units (ask=10) → buy 20 units (ask=0.5) → sell for 100 USDT (bid=5)
            var depths = new List<MarketDepth>
            {
                new()
                {
                    asks = new List<DepthData> { new() { price = "10",  quantity = "100" } },
                    bids = new List<DepthData>()
                },
                new()
                {
                    asks = new List<DepthData> { new() { price = "0.5", quantity = "100" } },
                    bids = new List<DepthData>()
                },
                new()
                {
                    asks = new List<DepthData>(),
                    bids = new List<DepthData> { new() { price = "5", quantity = "100" } }
                },
            };

            var result = CandidateGetter.GetProfitRatio(depths, usdtQuantity: 100.0, feeRatio: 0.0);

            Assert.Equal(1.0, result, precision: 10);
        }

        [Fact]
        public void GetProfitRatio_MarketDepth_ZeroUsdt_ReturnsZero()
        {
            var depths = new List<MarketDepth>
            {
                new() { asks = new List<DepthData> { new() { price = "10", quantity = "10" } }, bids = new List<DepthData>() },
                new() { asks = new List<DepthData> { new() { price = "10", quantity = "10" } }, bids = new List<DepthData>() },
                new() { asks = new List<DepthData>(), bids = new List<DepthData> { new() { price = "10", quantity = "10" } } },
            };

            var result = CandidateGetter.GetProfitRatio(depths, usdtQuantity: 0.0);

            Assert.Equal(0.0, result, precision: 10);
        }

        [Fact]
        public void GetProfitRatio_MarketDepth_WrongCount_ThrowsArgumentException()
        {
            var depths = new List<MarketDepth>
            {
                new() { asks = new List<DepthData>(), bids = new List<DepthData>() }
            };

            Assert.Throws<ArgumentException>(() =>
                CandidateGetter.GetProfitRatio(depths, usdtQuantity: 100.0));
        }
    }
}
