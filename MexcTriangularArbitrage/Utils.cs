using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MexcTriangularArbitrage
{
    public class Utils
    {
        private static ILogger _logger = AppLogger.CreateLogger<Utils>();

        public static void ErrorLog(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.LogError("{MemberName},{SourceFilePath}:{SourceLineNumber} Message:[{Message}]",
                memberName, sourceFilePath, sourceLineNumber, message);
        }

        public static T RetryDo<T>(
            Func<T> func,
            int count = 3,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            for(int i = 1; i < count; i++)
            {
                try {
                    return func();
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex,
                        "RetryDo attempt {Attempt}/{Count} failed. Caller:[{MemberName},{SourceFilePath}:{SourceLineNumber}]",
                        i, count, memberName, sourceFilePath, sourceLineNumber);
                }
            }
            return func();
        }

        public static long ToUtcUnixTimeMilliseconds(DateTime dt)
        {
            var dto = new DateTimeOffset(dt.Ticks, TimeSpan.FromHours(9));
            return dto.ToUnixTimeMilliseconds();
        }
    }
}
