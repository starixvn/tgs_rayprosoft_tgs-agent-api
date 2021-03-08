using System;
using System.Text.Json;
using ExpectedObjects;
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
            var data = (BaseData)new UrlData {Url = "http://google.com"};

            Func<string> func = () => JsonSerializer.Serialize(data);
            var value = func();

            func.Should().NotThrow();
            value.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Deserialize_UrlData()
        {
            var url = "http://google.com";
            var value = @$"{{""type"": 1,""url"": ""{url}""}}";
            var expectedObj = JsonSerializer.Deserialize<UrlData>(value);

            Func<BaseData> func = () => JsonSerializer.Deserialize<BaseData>(value);
            var data = func();

            func.Should().NotThrow();
            data.Should().BeOfType<UrlData>();

            var actual = (UrlData)data;
            expectedObj.Type.ToExpectedObject().ShouldMatch(actual.Type);
            expectedObj.Url.ToExpectedObject().ShouldMatch(actual.Url);
        }

        [Test]
        public void Can_Serialize_JsonData()
        {
            var json = @"[{""refNo"":""4026568"",""username"":""4ud28m1ecwqq""}]";
            var data = (BaseData)new JsonData {Value = json};

            Func<string> func = () => JsonSerializer.Serialize(data);
            var value = func();

            func.Should().NotThrow();
            value.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Deserialize_JsonData()
        {
            var json =
                "[{\\u0022refNo\\u0022:\\u00224026568\\u0022,\\u0022username\\u0022:\\u00224ud28m1ecwqq\\u0022}]";
            var value = @$"{{""type"":4,""value"":""{json}""}}";
            var expectedObj = JsonSerializer.Deserialize<JsonData>(value);

            Func<BaseData> func = () => JsonSerializer.Deserialize<BaseData>(value);
            var data = func();

            func.Should().NotThrow();
            data.Should().BeOfType<JsonData>();

            var actual = (JsonData)data;
            expectedObj.Type.ToExpectedObject().ShouldMatch(actual.Type);
            expectedObj.Value.ToExpectedObject().ShouldMatch(actual.Value);
        }

        [Test]
        public void Can_Serialize_XmlData()
        {
            var xml = @"<data><name>1</name><value></value></data>";
            var data = (BaseData)new XmlData {Value = xml};

            Func<string> func = () => JsonSerializer.Serialize(data);
            var value = func();

            func.Should().NotThrow();
            value.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Deserialize_XmlData()
        {
            var xml =
                "\\u003Cdata\\u003E\\u003Cname\\u003E1\\u003C/name\\u003E\\u003Cvalue\\u003E\\u003C/value\\u003E\\u003C/data\\u003E";
            var value = @$"{{""value"":""{xml}"",""type"":5}}";
            var expectedObj = JsonSerializer.Deserialize<XmlData>(value);

            Func<BaseData> func = () => JsonSerializer.Deserialize<BaseData>(value);
            var data = func();

            func.Should().NotThrow();
            data.Should().BeOfType<XmlData>();

            var actual = (XmlData)data;
            expectedObj.Type.ToExpectedObject().ShouldMatch(actual.Type);
            expectedObj.Value.ToExpectedObject().ShouldMatch(actual.Value);
        }

        [Test]
        public void Can_Serialize_TextData()
        {
            var text = "this is a book";
            var data = (BaseData)new TextData {Value = text};

            Func<string> func = () => JsonSerializer.Serialize(data);
            var value = func();

            func.Should().NotThrow();
            value.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Deserialize_TextData()
        {
            var value = "{\"value\":\"this is a book\",\"type\":3}";
            var expectedObj = JsonSerializer.Deserialize<TextData>(value);

            Func<BaseData> func = () => JsonSerializer.Deserialize<BaseData>(value);
            var data = func();

            func.Should().NotThrow();
            data.Should().BeOfType<TextData>();

            var actual = (TextData)data;
            expectedObj.Type.ToExpectedObject().ShouldMatch(actual.Type);
            expectedObj.Value.ToExpectedObject().ShouldMatch(actual.Value);
        }

        [Test]
        public void Can_Serialize_TableData()
        {
            var data = (BaseData)new TableData
            {
                Headers = new[] {"id", "name", "age"},
                Rows = new()
                {
                    new()
                    {
                        {"id", "1"},
                        {"name", "Tony"},
                        {"age", "20"}
                    },
                    new()
                    {
                        {"id", "2"},
                        {"name", "Mark"},
                        {"age", "45"}
                    }
                }
            };

            Func<string> func = () => JsonSerializer.Serialize(data);
            var value = func();

            func.Should().NotThrow();
            value.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Deserialize_TableData()
        {
            var value =
                "{\"headers\":[\"id\",\"name\",\"age\"],\"rows\":[{\"id\":\"1\",\"name\":\"Tony\",\"age\":\"20\"},{\"id\":\"2\",\"name\":\"Mark\",\"age\":\"45\"}],\"type\":2}";

            var expectedObj = JsonSerializer.Deserialize<TableData>(value);

            Func<BaseData> func = () => JsonSerializer.Deserialize<BaseData>(value);
            var data = func();

            func.Should().NotThrow();
            data.Should().BeOfType<TableData>();

            var actual = (TableData)data;
            expectedObj.Headers.ToExpectedObject().ShouldMatch(actual.Headers);
            expectedObj.Rows.ToExpectedObject().ShouldMatch(actual.Rows);
            expectedObj.Type.ToExpectedObject().ShouldMatch(actual.Type);
        }
    }
}