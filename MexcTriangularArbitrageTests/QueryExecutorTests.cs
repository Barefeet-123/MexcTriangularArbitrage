using System.Collections.Generic;
using Xunit;

namespace MexcTriangularArbitrage.Tests
{
    public class QueryExecutorTests
    {
        [Fact]
        public void GetRequestParamString_ValueWithSpace_EncodesAsPercentTwenty()
        {
            var param = new SortedDictionary<string, string>
            {
                { "symbol", "hello world" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("symbol=hello%20world", result);
        }

        [Fact]
        public void GetRequestParamString_KeyWithSpace_EncodesAsPercentTwenty()
        {
            var param = new SortedDictionary<string, string>
            {
                { "my key", "value" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("my%20key=value", result);
        }

        [Fact]
        public void GetRequestParamString_ValueWithSpace_DoesNotContainPlus()
        {
            var param = new SortedDictionary<string, string>
            {
                { "q", "foo bar" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.DoesNotContain("+", result);
        }

        [Fact]
        public void GetRequestParamString_NormalValue_RemainsUnchanged()
        {
            var param = new SortedDictionary<string, string>
            {
                { "symbol", "BTC_USDT" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("symbol=BTC_USDT", result);
        }

        [Fact]
        public void GetRequestParamString_EmptyDictionary_ReturnsEmptyString()
        {
            var param = new SortedDictionary<string, string>();

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("", result);
        }

        [Fact]
        public void GetRequestParamString_MultipleParams_JoinedWithAmpersand()
        {
            var param = new SortedDictionary<string, string>
            {
                { "limit", "50" },
                { "symbol", "BTC_USDT" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("limit=50&symbol=BTC_USDT", result);
        }

        [Fact]
        public void GetRequestParamString_MultipleSpaces_AllEncodedAsPercentTwenty()
        {
            var param = new SortedDictionary<string, string>
            {
                { "q", "a b c" }
            };

            var result = QueryExecutor.GetRequestParamString(param);

            Assert.Equal("q=a%20b%20c", result);
        }
    }
}
