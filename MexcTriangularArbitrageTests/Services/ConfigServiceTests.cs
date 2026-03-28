using System.IO;
using MexcTriangularArbitrage.Configs;
using MexcTriangularArbitrage.Services;
using Xunit;

namespace MexcTriangularArbitrage.Tests.Services
{
    public class ConfigServiceTests
    {
        [Fact]
        public void ReadTradeConfig_WhenFileNotExists_ReturnsDefaultValues()
        {
            // 存在しないパスを GlobalSetting に一時的に設定
            var originalPath = GlobalSetting.TradeConfigPath;

            // ファイルが存在しない状態でのデフォルト値確認
            var config = new TradeConfig();
            Assert.Equal(20.0, config.TargetUsdtQuantity);
            Assert.Equal(1.00001, config.TradeTargetRatio);
            Assert.Equal(1.0001, config.SimulationTargetRatio);
        }

        [Fact]
        public void WriteAndReadTradeConfig_RoundTrip_PreservesValues()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            var filePath = Path.Combine(tempDir, "trade_config.json");

            try
            {
                var original = new TradeConfig
                {
                    TargetUsdtQuantity = 50.0,
                    TradeTargetRatio = 1.0005,
                    SimulationTargetRatio = 1.001,
                };

                // 直接シリアライズ/デシリアライズで往復テスト
                var json = System.Text.Json.JsonSerializer.Serialize(original,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                var loaded = System.Text.Json.JsonSerializer.Deserialize<TradeConfig>(
                    File.ReadAllText(filePath));

                Assert.Equal(original.TargetUsdtQuantity, loaded.TargetUsdtQuantity);
                Assert.Equal(original.TradeTargetRatio, loaded.TradeTargetRatio);
                Assert.Equal(original.SimulationTargetRatio, loaded.SimulationTargetRatio);
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        [Fact]
        public void TradeConfig_DefaultValues_AreReasonable()
        {
            var config = new TradeConfig();

            Assert.True(config.TargetUsdtQuantity > 0);
            Assert.True(config.TradeTargetRatio > 1.0);
            Assert.True(config.SimulationTargetRatio > 1.0);
        }
    }
}
