using System;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.BackOffice;

namespace TGS.Tests.BackendOffice.UnitTests;

internal class DateLimiterTests
{
    [Test]
    public void FutureToToday_With_Both_Today_Input_Should_Return_Today()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var from = DateTimeOffset.Parse("2022-07-11 00:00:00+08:00");
        var to = DateTimeOffset.Parse("2022-07-11 00:00:00+08:00");
        var offset = TimeSpan.FromHours(8D);

        var expected = (DateOnly.Parse("2022-07-11"), DateOnly.Parse("2022-07-12"));

        var utcNow = DateTimeOffset.Parse("2022-07-10 16:00:00+00:00");

        _ = fakeSystemClock.UtcNow.Returns(utcNow);

        var sut = new DateLimiter(fakeSystemClock);

        var actual = sut.FutureToToday(from, to, offset);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FutureToToday_With_EndDate_Future_Input_Should_Return_From_As_Original_And_To_As_Today()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var from = DateTimeOffset.Parse("2022-07-09 00:00:00+08:00");
        var to = DateTimeOffset.Parse("2022-07-15 00:00:00+08:00");
        var offset = TimeSpan.FromHours(8D);

        var expected = (DateOnly.Parse("2022-07-09"), DateOnly.Parse("2022-07-12"));

        var utcNow = DateTimeOffset.Parse("2022-07-10 16:00:00+00:00");

        _ = fakeSystemClock.UtcNow.Returns(utcNow);

        var sut = new DateLimiter(fakeSystemClock);

        var actual = sut.FutureToToday(from, to, offset);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FutureToToday_With_Future_Date_Range_Input_Should_Return_Today()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var from = DateTimeOffset.Parse("2022-07-15 00:00:00+08:00");
        var to = DateTimeOffset.Parse("2022-07-20 00:00:00+08:00");
        var offset = TimeSpan.FromHours(8D);

        var expected = (DateOnly.Parse("2022-07-11"), DateOnly.Parse("2022-07-12"));

        var utcNow = DateTimeOffset.Parse("2022-07-10 16:00:00+00:00");

        _ = fakeSystemClock.UtcNow.Returns(utcNow);

        var sut = new DateLimiter(fakeSystemClock);

        var actual = sut.FutureToToday(from, to, offset);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FutureToToday_With_Past_Date_Range_Input_Should_Return_Original()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var from = DateTimeOffset.Parse("2022-07-08 00:00:00+08:00");
        var to = DateTimeOffset.Parse("2022-07-10 00:00:00+08:00");
        var offset = TimeSpan.FromHours(8D);

        var expected = (DateOnly.Parse("2022-07-08"), DateOnly.Parse("2022-07-10"));

        var utcNow = DateTimeOffset.Parse("2022-07-10 16:00:00+00:00");

        _ = fakeSystemClock.UtcNow.Returns(utcNow);

        var sut = new DateLimiter(fakeSystemClock);

        var actual = sut.FutureToToday(from, to, offset);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FutureToToday_With_Past_From_And_EndDate_As_Today_Input_Should_Return_Original()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var from = DateTimeOffset.Parse("2022-07-08 00:00:00+08:00");
        var to = DateTimeOffset.Parse("2022-07-11 00:00:00+08:00");
        var offset = TimeSpan.FromHours(8D);

        var expected = (DateOnly.Parse("2022-07-08"), DateOnly.Parse("2022-07-11"));

        var utcNow = DateTimeOffset.Parse("2022-07-10 16:00:00+00:00");

        _ = fakeSystemClock.UtcNow.Returns(utcNow);

        var sut = new DateLimiter(fakeSystemClock);

        var actual = sut.FutureToToday(from, to, offset);

        Assert.AreEqual(expected, actual);
    }
}
