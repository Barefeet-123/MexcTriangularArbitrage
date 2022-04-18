using System.Collections.Generic;

namespace MexcTriangularArbitrage.Schema
{
    public class MarketDepth
    {
        public List<DepthData> asks { get; set; }
        public List<DepthData> bids { get; set; }
    }

    public class DepthData
    {
        public string price { get; set; }
        public string quantity { get; set; }
        public double PriceAsDouble => double.Parse(price);
        public double QuantityAsDouble => double.Parse(quantity);

    }

}
