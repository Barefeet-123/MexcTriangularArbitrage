using System.Linq;

namespace MexcTriangularArbitrage.Schema
{
    public class SymbolTicker
    {
        public string symbol { get; set; }
        public string volume { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string bid { get; set; }
        public string ask { get; set; }
        public string open { get; set; }
        public string last { get; set; }
        public long time { get ;set;}
        public string change_rate { get; set; }
        public double VolumeAsDouble => double.Parse(volume);
        public string KeyCurrency => symbol.Split("_").FirstOrDefault();
        public string SettlementCurrency => symbol.Split("_").LastOrDefault();
        public double BidAsDouble => double.Parse(bid);
        public double AskAsDouble => double.Parse(ask);

        public override string ToString()
        {
            return $"Symbol:{symbol}, Bid:{bid}, Ask:{ask}, Volume:{volume}";
        }
    }
}
