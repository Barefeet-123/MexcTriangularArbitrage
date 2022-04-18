using System;
using System.Runtime.CompilerServices;

namespace MexcTriangularArbitrage
{
    public class Utils
    {
        public static void ErrorLog(object message, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            //TODO: Implement logging method
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} Error {memberName},{sourceFilePath}:{sourceLineNumber} Message:[{message}]");
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
                    var message = $"Error in RetryDo. Caller:[{memberName},{sourceFilePath}:{sourceLineNumber}], Exception:[{ex}]";
                    ErrorLog(message);
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
