namespace MexcTriangularArbitrage.Configs
{
    public class TradeConfig
    {
        /// <summary>1回の取引で使用するUSDT量</summary>
        public double TargetUsdtQuantity { get; set; } = 20.0;

        /// <summary>実取引で対象とする最低利益率 (例: 1.00001 = 0.001%以上の利益)</summary>
        public double TradeTargetRatio { get; set; } = 1.00001;

        /// <summary>シミュレーションで対象とする最低利益率</summary>
        public double SimulationTargetRatio { get; set; } = 1.0001;
    }
}
