using System;
using System.IO;
using System.Text;

namespace MexcTriangularArbitrage.Services
{
    /// <summary>
    /// 実行した三角取引の結果を CSV ファイルに追記する。
    /// </summary>
    public class TradeHistoryWriter : IDisposable
    {
        private readonly StreamWriter _writer;
        private bool _disposed;

        private static readonly string _header =
            "Timestamp,Symbol1,Symbol2,Symbol3,ProfitRatio,UsdtIn,UsdtOut";

        public TradeHistoryWriter(string filePath)
        {
            var isNewFile = !File.Exists(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            _writer = new StreamWriter(filePath, append: true, Encoding.UTF8);
            if (isNewFile)
            {
                _writer.WriteLine(_header);
                _writer.Flush();
            }
        }

        public void Write(string symbol1, string symbol2, string symbol3,
            double profitRatio, double usdtIn, double usdtOut)
        {
            var line = string.Join(",",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                symbol1, symbol2, symbol3,
                profitRatio.ToString("F5"),
                usdtIn.ToString("F5"),
                usdtOut.ToString("F5"));
            _writer.WriteLine(line);
            _writer.Flush();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _writer.Dispose();
            _disposed = true;
        }
    }
}
