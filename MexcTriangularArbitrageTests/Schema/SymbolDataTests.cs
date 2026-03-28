using MexcTriangularArbitrage.Schema;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Schema
{
    public class SymbolDataTests
    {
        [Theory]
        [InlineData("10", 10.0)]
        [InlineData("0.5", 0.5)]
        [InlineData("100.25", 100.25)]
        public void MinAmountAsDouble_ParsesStringCorrectly(string minAmount, double expected)
        {
            var data = new SymbolData { min_amount = minAmount };
            Assert.Equal(expected, data.MinAmountAsDouble);
        }
    }
}
