using System.IO;
using System.Linq;
using MexcTriangularArbitrage.Services;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Services
{
    public class TradeHistoryWriterTests
    {
        [Fact]
        public void Write_CreatesFileWithHeader()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                using var writer = new TradeHistoryWriter(filePath);
                writer.Write("BTC_USDT", "ETH_BTC", "ETH_USDT", 1.0012, 20.0, 20.024);

                Assert.True(File.Exists(filePath));
                var lines = File.ReadAllLines(filePath);
                Assert.Equal("Timestamp,Symbol1,Symbol2,Symbol3,ProfitRatio,UsdtIn,UsdtOut", lines[0]);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public void Write_SingleEntry_ContainsAllFields()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                using var writer = new TradeHistoryWriter(filePath);
                writer.Write("BTC_USDT", "ETH_BTC", "ETH_USDT", 1.0012, 20.0, 20.024);

                var lines = File.ReadAllLines(filePath);
                Assert.Equal(2, lines.Length); // header + 1 data row
                var parts = lines[1].Split(',');
                Assert.Equal(7, parts.Length);
                Assert.Equal("BTC_USDT", parts[1]);
                Assert.Equal("ETH_BTC", parts[2]);
                Assert.Equal("ETH_USDT", parts[3]);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public void Write_MultipleEntries_AppendsRows()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                using var writer = new TradeHistoryWriter(filePath);
                writer.Write("BTC_USDT", "ETH_BTC", "ETH_USDT", 1.001, 20.0, 20.02);
                writer.Write("BTC_USDT", "XRP_BTC", "XRP_USDT", 1.002, 20.0, 20.04);

                var lines = File.ReadAllLines(filePath);
                Assert.Equal(3, lines.Length); // header + 2 data rows
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public void Write_AppendMode_DoesNotOverwriteExistingFile()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                // 1回目: 1件書き込み
                using (var writer = new TradeHistoryWriter(filePath))
                {
                    writer.Write("BTC_USDT", "ETH_BTC", "ETH_USDT", 1.001, 20.0, 20.02);
                }

                // 2回目: 追記
                using (var writer = new TradeHistoryWriter(filePath))
                {
                    writer.Write("BTC_USDT", "XRP_BTC", "XRP_USDT", 1.002, 20.0, 20.04);
                }

                var lines = File.ReadAllLines(filePath);
                // ヘッダーは1回目のみ、データ行が2行
                Assert.Equal(3, lines.Length);
                Assert.Equal("Timestamp,Symbol1,Symbol2,Symbol3,ProfitRatio,UsdtIn,UsdtOut", lines[0]);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public void Write_ProfitRatioFormattedToFiveDecimals()
        {
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".csv");
            try
            {
                using var writer = new TradeHistoryWriter(filePath);
                writer.Write("S1", "S2", "S3", 1.00123456, 20.0, 20.024691);

                var lines = File.ReadAllLines(filePath);
                var parts = lines[1].Split(',');
                Assert.Equal("1.00123", parts[4]); // F5 format
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
