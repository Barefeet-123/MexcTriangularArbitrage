using System.Collections.Generic;

namespace MexcTriangularArbitrage.Schema
{
    /// <summary>
    /// Balance information of each currency name
    /// </summary>
    public class BalanceDictionary : Dictionary<string, Balance> { }

    public class Balance
    {
        public string frozen { get; set; }
        public string available { get; set; }
    }
}
