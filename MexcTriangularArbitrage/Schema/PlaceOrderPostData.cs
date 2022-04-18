using System;

namespace MexcTriangularArbitrage.Schema
{
    public class PlaceOrderPostData
    {
        public string symbol { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string trade_type { get; set; }
        public string order_type { get; set; }
        public string client_order_id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    }
}
