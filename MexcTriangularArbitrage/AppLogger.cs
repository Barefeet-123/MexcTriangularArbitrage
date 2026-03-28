using Microsoft.Extensions.Logging;

namespace MexcTriangularArbitrage
{
    internal static class AppLogger
    {
        private static readonly ILoggerFactory _factory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug);
        });

        internal static ILogger<T> CreateLogger<T>() => _factory.CreateLogger<T>();

        internal static ILogger CreateLogger(string categoryName) => _factory.CreateLogger(categoryName);
    }
}
