using MexcTriangularArbitrage.Schema;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Schema
{
    public class DepthDataTests
    {
        [Theory]
        [InlineData("40000.5", 40000.5)]
        [InlineData("0.001", 0.001)]
        [InlineData("1", 1.0)]
        public void PriceAsDouble_ParsesStringCorrectly(string price, double expected)
        {
            var data = new DepthData { price = price };
            Assert.Equal(expected, data.PriceAsDouble);
        }

        [Theory]
        [InlineData("100.5", 100.5)]
        [InlineData("0.5", 0.5)]
        [InlineData("999", 999.0)]
        public void QuantityAsDouble_ParsesStringCorrectly(string quantity, double expected)
        {
            var data = new DepthData { quantity = quantity };
            Assert.Equal(expected, data.QuantityAsDouble);
        }
    }
}
