namespace MexcTriangularArbitrage.Schema
{
    public class OpenOrder
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string remain_quantity { get; set; }
        public string remain_amount { get; set; }
        public long create_time { get;set; }
    }
}
