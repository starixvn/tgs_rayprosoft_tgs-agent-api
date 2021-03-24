using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Workers.Schedulers.Jobs.Reports;

namespace TGS.Tests.Schedulers.UnitTests.Report
{
    [TestFixture]
    [Parallelizable]
    public class PlayerAggregateReportWorkerServiceTests
    {
        [SetUp]
        public void Setup() =>
            _service = new PlayerAggregateReportWorkerService(
                _playerMock.Object,
                _playerGameTypeMock.Object,
                _roundMock.Object,
                _transactionMock.Object,
                _logMock.Object
            );

        private IPlayerAggregateReportWorkerService _service;
        private readonly Mock<IPlayerAggregateService> _playerMock = new();
        private readonly Mock<IPlayerGameTypeAggregateService> _playerGameTypeMock = new();
        private readonly Mock<IRoundAggregateService> _roundMock = new();
        private readonly Mock<ITransactionManager> _transactionMock = new();

        private readonly Mock<ITgsLog> _logMock = new();

        [Test]
        public async Task Can_ExecutionAsync()
        {
            var fixture = new Fixture();

            var playerAggregates = fixture
                .CreateMany<PlayerAggregate>(2)
                .AsQueryable()
                .BuildMock().Object;

            var playerAggregateModels = fixture
                .CreateMany<PlayerAggregateModel>(2)
                .AsEnumerable().ToArray();
            var playerGameTypeAggregateModels = fixture
                .CreateMany<PlayerGameTypeAggregateModel>(2)
                .AsEnumerable().ToArray();

            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));

            _playerMock.Setup(e => e.GetPlayerAggregateQueries(null, false))
                .Returns(playerAggregates);
            _roundMock.Setup(
                    e => e.GetPlayerAggregatesAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((playerAggregateModels, playerGameTypeAggregateModels)));

            var response = await _service.ExecutionAsync();

            response.Error.Should().Be(0);
            response.ErrorMessage.Should().BeEmpty();
        }
    }
}