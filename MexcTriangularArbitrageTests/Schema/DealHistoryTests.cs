using MexcTriangularArbitrage.Schema;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Schema
{
    public class DealHistoryTests
    {
        [Theory]
        [InlineData("0.5", 0.5)]
        [InlineData("10", 10.0)]
        [InlineData("0.00123", 0.00123)]
        public void QuantityAsDouble_ParsesStringCorrectly(string quantity, double expected)
        {
            var history = new DealHistory { quantity = quantity };
            Assert.Equal(expected, history.QuantityAsDouble);
        }

        [Theory]
        [InlineData("200.5", 200.5)]
        [InlineData("0.01", 0.01)]
        [InlineData("50000", 50000.0)]
        public void AmountAsDouble_ParsesStringCorrectly(string amount, double expected)
        {
            var history = new DealHistory { amount = amount };
            Assert.Equal(expected, history.AmountAsDouble);
        }
    }
}
