using System;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class DateTimeExtensionsTests
    {
        private static readonly object[] TestToUnixTimeSeconds =
        {
            new object[] { new DateTime(2007, 5, 24, 11, 36, 22, DateTimeKind.Utc), 1180006582 },
            new object[] { new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), -62135596800 }
        };

        [Test, TestCaseSource(nameof(TestToUnixTimeSeconds))]
        public void Can_Execute_ToUnixTimeSeconds(DateTime queryDateTime, long result)
        {
            queryDateTime.ToUnixTimeSeconds().Should().Be(result);
        }

        [Test]
        public void Can_Execute_TestToUnixTimeSecondsByNonUTC()
        {
            var queryDateTime = new DateTime(2007, 5, 30, 11, 36, 22, DateTimeKind.Unspecified);

            var verifyDateTimeOffset = new DateTimeOffset(queryDateTime);

            verifyDateTimeOffset.DateTime.ToUnixTimeSeconds().Should().Be(queryDateTime.ToUnixTimeSeconds());
        }

        private static readonly object[] TestToUtcDateTime =
        {
            new object[] {new DateTime(2007, 5, 24, 11, 36, 22, DateTimeKind.Utc), 1180006582 }
        };

        [Test, TestCaseSource(nameof(TestToUtcDateTime))]
        public void Can_Execute_ToUTCDateTime(DateTime result, long queryLong)
        {
            queryLong.UnixTimeSecondsToUtcDateTime().Should().Be(result);
        }
    }
}