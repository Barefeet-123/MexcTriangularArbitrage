namespace MexcTriangularArbitrage.Schema
{
    public class SymbolData
    {
        public string symbol { get; set; }
        public string state { get; set; }
        public string timeZone { get; set; }
        public string fullName { get; set; }
        public int symbolStatus { get; set; }
        public string vcoinName { get; set; }
        public int vcoinStatus { get; set; }
        public int price_scale { get; set; }
        public int quantity_scale { get; set; }
        public string min_amount { get; set; }
        public string max_amount { get; set; }
        public string maker_fee_rate { get; set; }
        public string taker_fee_rate { get; set; }
        public bool limited { get; set; }
        public int etf_mark { get; set; }
        public string symbol_partition { get; set; }

        public double MinAmountAsDouble => double.Parse(min_amount);
    }
}
