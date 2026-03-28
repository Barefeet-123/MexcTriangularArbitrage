using System;
using Xunit;

namespace MexcTriangularArbitrage.Tests
{
    public class UtilsTests
    {
        // JST (UTC+9) の 1970-01-01 09:00:00 は UTC の Unix エポック (0ms)
        [Fact]
        public void ToUtcUnixTimeMilliseconds_JstEpoch_ReturnsZero()
        {
            var jstEpoch = new DateTime(1970, 1, 1, 9, 0, 0);
            Assert.Equal(0L, Utils.ToUtcUnixTimeMilliseconds(jstEpoch));
        }

        // JST 1970-01-01 10:00:00 = UTC 01:00:00 = 3600000ms
        [Fact]
        public void ToUtcUnixTimeMilliseconds_OneHourAfterJstEpoch_Returns3600000()
        {
            var dt = new DateTime(1970, 1, 1, 10, 0, 0);
            Assert.Equal(3_600_000L, Utils.ToUtcUnixTimeMilliseconds(dt));
        }

        // JST 1970-01-01 09:00:01 = 1000ms
        [Fact]
        public void ToUtcUnixTimeMilliseconds_OneSecondAfterJstEpoch_Returns1000()
        {
            var dt = new DateTime(1970, 1, 1, 9, 0, 1);
            Assert.Equal(1_000L, Utils.ToUtcUnixTimeMilliseconds(dt));
        }

        [Fact]
        public void RetryDo_SuccessOnFirstAttempt_ReturnsValue()
        {
            var result = Utils.RetryDo(() => 42);
            Assert.Equal(42, result);
        }

        [Fact]
        public void RetryDo_SuccessOnSecondAttempt_ReturnsValue()
        {
            int callCount = 0;
            var result = Utils.RetryDo(() =>
            {
                callCount++;
                if (callCount < 2) throw new Exception("fail");
                return callCount;
            });
            Assert.Equal(2, result);
        }

        [Fact]
        public void RetryDo_AlwaysFails_ThrowsOnFinalAttempt()
        {
            Assert.Throws<Exception>(() =>
                Utils.RetryDo<int>(() => throw new Exception("always fail"), count: 3)
            );
        }

        [Fact]
        public void RetryDo_AlwaysFails_AttemptsExactlyCountTimes()
        {
            int callCount = 0;
            try
            {
                Utils.RetryDo<int>(() =>
                {
                    callCount++;
                    throw new Exception("fail");
                }, count: 3);
            }
            catch { }
            Assert.Equal(3, callCount);
        }
    }
}
