using System;
using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;

namespace Infrastructures.UnitTests
{
    [TestFixture]
    public class BrandSettlementIntervalGeneratorTests
    {
        [Test]
        public void DailyTests()
        {
            var (cFrom, cTo, nFrom, nTo) =
                BrandSettlementIntervalGenerator.GetInterval(SettlementIntervalType.Daily);

            var timeSpan = new TimeSpan(8, 0, 0);
            var date = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month,
                DateTime.Now.Day, 0,
                0, 0, timeSpan);

            cFrom.Should().Be(date);
            cTo.Should().Be(date);
            nFrom.Should().Be(date.AddDays(1));
            nTo.Should().Be(date.AddDays(1));
        }

        [Test]
        public void DailyWithDateTests()
        {
            var timeSpan = new TimeSpan(8, 0, 0);
            var date = new DateTimeOffset(2021, 6,
                6, 0,
                0, 0, timeSpan);
            var (cFrom, cTo, nFrom, nTo) =
                BrandSettlementIntervalGenerator.GetInterval(SettlementIntervalType.Daily, date);
            cFrom.Should().Be(date);
            cTo.Should().Be(date);
            nFrom.Should().Be(date.AddDays(1));
            nTo.Should().Be(date.AddDays(1));
        }

        [Test]
        public void WeeklyWithDateTests()
        {
            var timeSpan = new TimeSpan(8, 0, 0);
            var date = new DateTimeOffset(2021, 6,
                3, 0,
                0, 0, timeSpan);
            var (cFrom, cTo, nFrom, nTo) =
                BrandSettlementIntervalGenerator.GetInterval(SettlementIntervalType.Weekly, date);
            cFrom.Should().Be(date);
            cTo.Should().Be(new DateTimeOffset(2021, 6,
                6, 0,
                0, 0, timeSpan));
            nFrom.Should().Be(new DateTimeOffset(2021, 6,
                7, 0,
                0, 0, timeSpan));
            nTo.Should().Be(new DateTimeOffset(2021, 6,
                13, 0,
                0, 0, timeSpan));
        }

        [Test]
        public void MonthlyWithDateTests()
        {
            var timeSpan = new TimeSpan(8, 0, 0);
            var date = new DateTimeOffset(2021, 6,
                3, 0,
                0, 0, timeSpan);
            var (cFrom, cTo, nFrom, nTo) =
                BrandSettlementIntervalGenerator.GetInterval(SettlementIntervalType.Monthly, date);
            cFrom.Should().Be(date);
            cTo.Should().Be(new DateTimeOffset(2021, 6,
                30, 0,
                0, 0, timeSpan));
            nFrom.Should().Be(new DateTimeOffset(2021, 7,
                1, 0,
                0, 0, timeSpan));
            nTo.Should().Be(new DateTimeOffset(2021, 7,
                31, 0,
                0, 0, timeSpan));
        }
    }
}