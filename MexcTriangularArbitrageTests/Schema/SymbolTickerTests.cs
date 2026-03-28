using MexcTriangularArbitrage.Schema;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Schema
{
    public class SymbolTickerTests
    {
        [Theory]
        [InlineData("BTC_USDT", "BTC")]
        [InlineData("ETH_USDT", "ETH")]
        [InlineData("XRP_BTC", "XRP")]
        public void KeyCurrency_ReturnsPartBeforeUnderscore(string symbol, string expected)
        {
            var ticker = new SymbolTicker { symbol = symbol };
            Assert.Equal(expected, ticker.KeyCurrency);
        }

        [Theory]
        [InlineData("BTC_USDT", "USDT")]
        [InlineData("ETH_USDT", "USDT")]
        [InlineData("XRP_BTC", "BTC")]
        public void SettlementCurrency_ReturnsPartAfterUnderscore(string symbol, string expected)
        {
            var ticker = new SymbolTicker { symbol = symbol };
            Assert.Equal(expected, ticker.SettlementCurrency);
        }

        [Theory]
        [InlineData("40000.5")]
        [InlineData("0.00123")]
        [InlineData("1")]
        public void AskAsDouble_ParsesStringCorrectly(string ask)
        {
            var ticker = new SymbolTicker { ask = ask };
            Assert.Equal(double.Parse(ask), ticker.AskAsDouble);
        }

        [Theory]
        [InlineData("39999.9")]
        [InlineData("0.00122")]
        [InlineData("1")]
        public void BidAsDouble_ParsesStringCorrectly(string bid)
        {
            var ticker = new SymbolTicker { bid = bid };
            Assert.Equal(double.Parse(bid), ticker.BidAsDouble);
        }

        [Fact]
        public void VolumeAsDouble_ParsesStringCorrectly()
        {
            var ticker = new SymbolTicker { volume = "12345.678" };
            Assert.Equal(12345.678, ticker.VolumeAsDouble);
        }

        [Fact]
        public void ToString_ContainsSymbolBidAskVolume()
        {
            var ticker = new SymbolTicker
            {
                symbol = "BTC_USDT",
                bid = "39999",
                ask = "40000",
                volume = "100"
            };
            var result = ticker.ToString();
            Assert.Contains("BTC_USDT", result);
            Assert.Contains("39999", result);
            Assert.Contains("40000", result);
            Assert.Contains("100", result);
        }
    }
}
