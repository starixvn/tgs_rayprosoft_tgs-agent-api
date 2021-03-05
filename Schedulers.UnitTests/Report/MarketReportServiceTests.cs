using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Workers.Schedulers.Jobs.Reports;

namespace TGS.Tests.Schedulers.UnitTests.Report
{
    [TestFixture]
    [Parallelizable]
    public class MarketReportServiceTests
    {
        [SetUp]
        public void Setup() =>
            _service = new MarketReportService(
                _repositoryMock.Object,
                _transactionMock.Object,
                _marketMock.Object,
                _roundMock.Object,
                _logMock.Object);

        private IMarketReportService _service;
        private readonly Mock<ITgsRepository> _repositoryMock = new();
        private readonly Mock<ITransactionManager> _transactionMock = new();
        private readonly Mock<IMarketService> _marketMock = new();
        private readonly Mock<IRoundService> _roundMock = new();

        private readonly Mock<ITgsLog> _logMock = new();

        [Test]
        public async Task Can_ExecutionAsync()
        {
            var markets = CreateMarketFixture()
                .CreateMany<Market>(2)
                .AsQueryable()
                .BuildMock().Object;

            var marketRequests = CreateMarketRequestFixture()
                .CreateMany<MarketRequest>(2)
                .AsEnumerable().ToArray();

            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));

            _marketMock.Setup(e => e.GetMarketsAsync(null, false))
                .Returns(markets);
            _roundMock.Setup(e => e.GetRoundsAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(marketRequests));

            _transactionMock.Setup(s => s.ExecuteTransactionAsync(() => _repositoryMock.Object,
                    IsolationLevel.ReadCommitted, It.IsAny<string>(),
                    It.IsAny<Func<ITgsRepository, Task<TgsResponse>>>(), null, null, true))
                .Returns(Task.FromResult(new TgsResponse()));

            var response = await _service.ExecutionAsync();

            response.Error.Should().Be(0);
            response.ErrorMessage.Should().BeEmpty();
        }

        private Fixture CreateMarketFixture()
        {
            var fixture = new Fixture();
            fixture.Create<Market>();
            return fixture;
        }

        private Fixture CreateMarketRequestFixture()
        {
            var fixture = new Fixture();
            fixture.Create<MarketRequest>();
            return fixture;
        }
    }
}