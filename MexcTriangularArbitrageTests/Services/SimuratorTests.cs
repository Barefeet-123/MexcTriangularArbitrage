using MexcTriangularArbitrage.Services;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Services
{
    public class SimulatorTests
    {
        [Fact]
        public void TotalProfitRatio_WhenNoPurchases_ReturnsZeroInsteadOfDivideByZero()
        {
            var simulator = new Simulator();

            // TotalBoughtUsdtQuantity == 0 の状態で参照しても例外にならない
            Assert.Equal(0.0, simulator.TotalProfitRatio);
        }
    }
}
