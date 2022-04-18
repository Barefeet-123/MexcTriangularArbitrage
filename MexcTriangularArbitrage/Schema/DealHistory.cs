namespace MexcTriangularArbitrage.Schema
{
    public class DealHistory
    {
        public string symbol { get; set; }
        public string order_id { get; set; }
        public string quantity { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public string fee { get; set; }
        public string trade_type { get; set; }
        public string fee_currency { get; set; }
        public bool is_taker { get; set; }
        public long create_time { get;set; }
        public double QuantityAsDouble => double.Parse(quantity);
        public double AmountAsDouble => double.Parse(amount);

    }
}
