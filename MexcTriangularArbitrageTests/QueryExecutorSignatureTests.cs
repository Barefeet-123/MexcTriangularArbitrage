using Xunit;

namespace MexcTriangularArbitrage.Tests
{
    public class QueryExecutorSignatureTests
    {
        // 有名な HMAC-SHA256 テストベクタ
        // key="key", data="The quick brown fox jumps over the lazy dog"
        // expected=f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8
        [Fact]
        public void ComputeActualSignature_KnownVector_ReturnsExpectedHmac()
        {
            const string key  = "key";
            const string data = "The quick brown fox jumps over the lazy dog";
            const string expected = "f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8";

            var result = QueryExecutor.ComputeActualSignature(data, key);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ComputeActualSignature_EmptyInputs_ReturnsExpectedHmac()
        {
            // HMAC-SHA256("", "") の既知値
            const string expected = "b613679a0814d9ec772f95d778c35fc5ff1697c493715653c6c712144292c5ad";

            var result = QueryExecutor.ComputeActualSignature("", "");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ComputeActualSignature_ReturnsLowercase()
        {
            var result = QueryExecutor.ComputeActualSignature("data", "key");

            Assert.Equal(result.ToLower(), result);
        }

        [Fact]
        public void ComputeActualSignature_Returns64HexCharacters()
        {
            var result = QueryExecutor.ComputeActualSignature("data", "key");

            Assert.Equal(64, result.Length);
        }

        [Fact]
        public void ComputeActualSignature_DifferentKey_ReturnsDifferentResult()
        {
            var result1 = QueryExecutor.ComputeActualSignature("data", "key1");
            var result2 = QueryExecutor.ComputeActualSignature("data", "key2");

            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void ComputeActualSignature_DifferentData_ReturnsDifferentResult()
        {
            var result1 = QueryExecutor.ComputeActualSignature("data1", "key");
            var result2 = QueryExecutor.ComputeActualSignature("data2", "key");

            Assert.NotEqual(result1, result2);
        }
    }
}
