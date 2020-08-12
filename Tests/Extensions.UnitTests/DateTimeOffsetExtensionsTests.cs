using System;
using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class DateTimeOffsetExtensionsTests
    {
        [TestCase("2012-11-20T00:00:00Z", "2012-11-20T00:00:00+00:00")]
        public void Can_Execute_ToIso8601String(string queryString, string result)
        {
            var utc = DateTimeOffset.Parse(queryString);
            utc.ToIso8601String().Should().Be(result);
        }

        [TestCase("2012-11-20T00:00:00Z", "2012-11-20 00:00:00")]
        public void Can_Execute_ToDateTimeFormat(string queryString, string result)
        {
            var utc = DateTimeOffset.Parse(queryString);
            utc.ToDateTimeFormat().Should().Be(result);
        }

        [TestCase("2012-11-20T00:00:00Z", "2012-11-20 00:00+00:00")]
        public void Can_Execute_ToUtcDateTimeFormat(string queryString, string result)
        {
            var utc = DateTimeOffset.Parse(queryString);
            utc.ToUtcDateTimeFormat().Should().Be(result);
        }

        [TestCase("2012-11-20T01:42:16Z", 634889725369999999)]
        public void Can_Execute_ToMaxTimeForSecond(string queryString, long result)
        {
            var utc = DateTimeOffset.Parse(queryString);
            utc.ToMaxTimeForSecond().Ticks.Should().Be(result);
        }

        [TestCase("2012-11-20T01:42:16Z", 1353375736000)]
        public void Can_Execute_ToUnixTimeMilliseconds(string queryString, long result)
        {
            var utc = DateTimeOffset.Parse(queryString);
            utc.ToUnixTimeMilliseconds().Should().Be(result);
        }
    }
}