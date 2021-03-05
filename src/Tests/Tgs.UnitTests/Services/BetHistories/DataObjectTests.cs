using System;
using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using TGS.Core.Models.Services.BetHistories;

namespace TGS.UnitTests.Services.BetHistories
{
    [TestFixture]
    public sealed class DataObjectTests
    {
        [Test]
        public void Can_Serialize_UrlData()
        {
            var data = (IData)new UrlData {Url = "http://google.com"};

            Action act = () => JsonSerializer.Serialize(data);
            act.Should().NotThrow();
        }

        [Test]
        public void Can_Deserialize_UrlData()
        {
            var value = @"{ type: 4, value = ""http: //google.com"" }";

            Action act = () => JsonSerializer.Deserialize<IData>(value);
            act.Should().NotThrow();
        }
    }
}