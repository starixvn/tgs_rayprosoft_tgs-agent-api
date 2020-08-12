using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class EnumerableExtensionsTests
    {
        private readonly List<int> _list = new List<int> { 2, 201, 3, 304, 1, 116, 2 };

        [Test]
        public void Can_Execute_Each()
        {
            var listVerifyForEach = new List<int>();

            _list.Each(val => listVerifyForEach.Add(val));

            _list.Should().BeEquivalentTo(listVerifyForEach);
        }

        [Test]
        public async Task Can_Execute_EachAsync()
        {
            var listVerifyForEachAsync = new List<int>();

            await _list.EachAsync(
                val =>
                {
                    listVerifyForEachAsync.Add(val);
                    return Task.FromResult(0);
                }
            );

            _list.Should().BeEquivalentTo(listVerifyForEachAsync);
        }

        [Test]
        public void Can_Execute_EachWithIndex()
        {
            var listVerifyForEachWithIndex = new List<int>();

            _list.EachWithIndex((val, index) => listVerifyForEachWithIndex.Add(val));

            _list.Should().BeEquivalentTo(listVerifyForEachWithIndex);
        }

        [Test]
        public void Can_Execute_ToHashSet()
        {
            var verifyHashSet = new HashSet<int> { 2, 201, 3, 2, 304, 1, 116 };

            var hashSetVal = _list.ToHashSet(val => val);

            hashSetVal.Should().BeEquivalentTo(verifyHashSet);
        }

        [TestCase(0)]
        public void Can_Execute_ToArraySegmentsOfMax_When_Limit_Is_0(int limit)
        {
            Assert.Throws(typeof(ArgumentException), () => new List<int>().ToArraySegmentsOfMax(limit));
        }

        [TestCase(2)]
        public void Can_Execute_ToArraySegmentsOfMax_When_Counts_Is_0(int limit)
        {
            var listForCountsIs0 = new List<int>();
            var limitList = listForCountsIs0.ToArraySegmentsOfMax(limit);

            Assert.AreEqual(limitList.Count, listForCountsIs0.Count);
        }

        [TestCase(3)]
        public void Can_Execute_ToArraySegmentsOfMax(int limit)
        {
            var limitList = _list.ToArraySegmentsOfMax(limit);
            var realcount = _list.Count;
            var listcount = limitList.Sum(i => i.Count());

            Assert.AreEqual(listcount, realcount);
        }

        [Test]
        public void Can_Execute_DistinctBy_Null()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ((List<int>)null).DistinctBy(val => val));
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public TestData(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        [Test]
        public void Can_Execute_DistinctBy()
        {
            var first = new TestData(1, "Asia");
            var second = new TestData(2, "Fusion");
            var third = new TestData(3, "Asia");
            var fouth = new TestData(1, "Asia");

            var listForDistinctBy = new List<TestData> { first, second, third, fouth };

            var varifyListForDistinctBy = new List<TestData> { first, second };

            var list = listForDistinctBy.DistinctBy(val => val.Name).ToList();

            CollectionAssert.AreEqual(
                list,
                varifyListForDistinctBy);
        }

        [Test]
        public async Task Can_Execute_WhenAll()
        {
            var number = 3;

            var tasks = new Task<int>[number];
            for (var i = 0; i < number; i++)
            {
                tasks[i] = Task.FromResult(1);
            }

            var result = await tasks.WhenAll();
            result.Length.Should().Be(number);
        }

        public class TestCanRead
        {
            public int Id { get; set; }
        }

        [Test]
        public void Can_Get_Null_When_CanRead_Is_False()
        {
            var first = new TestCanRead { Id = 1 };

            TestCanRead[] requestObj = { first };
            string[] header = { "Id", "Name" };

            requestObj.ToCsv(header).Should().Be("Id,Name\r\n1");
        }

        [Test]
        public void Can_Execute_ToCsv_By_ExpandObj()
        {
            dynamic expandO1 = new ExpandoObject();
            expandO1.Name = "Asia,\"Fusion";
            expandO1.ID = 1;

            dynamic expandO2 = new ExpandoObject();
            expandO2.Name = null;
            expandO2.ID = 2;

            string[] headers = { "Name", "Id" };
            object[] arrayObject = { expandO1, expandO2 };
            arrayObject.ToCsv(headers).Should().Be("Name,Id\r\n\"Asia,\"\"Fusion\",1\r\n,2");
        }

        [Test]
        public void Can_Execute_ToCsv_When_Header_Is_Null()
        {
            var first = new TestData(1, "Asia,Fusion");
            var second = new TestData(2, "Area,Taiwan");

            var arrayObject = new[] { first, second };

            arrayObject.ToCsv().Should().Be("Id,Name\r\n1,\"Asia,Fusion\"\r\n2,\"Area,Taiwan\"");
        }

        [Test]
        public void Can_Execute_ToCsv_When_Header_Is_Not_Null()
        {
            var first = new TestData(1, "Asia,Fusion");
            var second = new TestData(2, "Area,Taiwan");

            TestData[] arrayObject = { first, second };
            string[] headers = { "Id,NAME" };

            arrayObject.ToCsv(headers).Should().Be("Id,NAME\r\n1,\"Asia,Fusion\"\r\n2,\"Area,Taiwan\"");
        }

        [Test]
        public void Can_Execute_ToPercentStrin_By_Empty()
        {
            ((decimal?)null).ToPercentString().Should().Be(string.Empty);
        }

        [Test]
        public void Can_Execute_ToPercentString()
        {
            decimal? requestDecimal = 3;

            requestDecimal.ToPercentString().Should().Be("300.00 %");
        }

        [TestCase(1000)]
        public void Can_Execute_ToDataTable(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var input = new Fixture().Build<JackpotContribution>().CreateMany(4).ToArray();
                var sw = new Stopwatch();
                sw.Restart();
                var response = input.ToDataTable();
                sw.Stop();
                response.Rows.Count.Should().Be(input.Length);
                input.Last().ExternalRoundId.Should().Be(response.Rows[3]["ExternalRoundId"].ToString());
            }
        }
    }

    internal class JackpotContribution
    {
        public Guid Id { get; set; }

        public string JackpotExternalId { get; set; }

        public Guid JackpotId { get; set; }

        public Guid GameActionId { get; set; }

        public Guid BetId { get; set; }

        public Guid PlayerId { get; set; }

        public Guid BrandId { get; set; }

        public Guid GameId { get; set; }

        public decimal ContributedAmount { get; set; }

        public decimal ContributedAmountConverted { get; set; }

        public string ConversionCurrencyCode { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal JackpotBalance { get; set; }

        public decimal ReserveAmount { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset SentOn { get; set; }

        public DateTime StoredOn { get; set; }

        public decimal HitAmount { get; set; }

        public decimal HitAmountConverted { get; set; }

        public string HitConversionCurrencyCode { get; set; }

        public decimal HitExchangeRate { get; set; }

        public decimal PostHitWalletBalance { get; set; }

        public string ExternalRoundId { get; set; }

        public string PlayerIp { get; set; }

        public int PlayerType { get; set; }

        public decimal ContributionPercentage { get; set; }

        public string ContributionFrom { get; set; }

        public decimal DistributionPercentage { get; set; }
    }
}