//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoFixture;
//using FluentAssertions;
//using MockQueryable.Moq;
//using Moq;
//using NUnit.Framework;
//using Report.Client;
//using TGS.Core.DataServices.Report.Models;
//using TGS.Core.Infrastructures.Log;
//using TGS.Core.Infrastructures.Transactions;
//using TGS.Services.Report.Application.Models.Player;
//using TGS.Services.Report.Infrastructure.DataService;
//using TGS.Workers.Schedulers.Jobs.Reports;

//namespace TGS.Tests.Schedulers.UnitTests.Report
//{
//    [TestFixture]
//    [Parallelizable]
//    public class PlayerAggregateReportWorkerServiceTests
//    {
//        [SetUp]
//        public void Setup() =>
//            _service = new PlayerAggregateReportWorkerService(
//                _clientMock.Object
//            );

//        private IPlayerAggregateReportWorkerService _service;
//        private readonly Mock<IReportAggregateClient> _clientMock = new();

//        [Test]
//        public async Task Can_ExecutionAsync()
//        {
//            var fixture = new Fixture();

//            var playerAggregates = fixture
//                .CreateMany<PlayerAggregate>(2)
//                .AsQueryable()
//                .BuildMock().Object;

//            var playerAggregateModels = fixture
//                .CreateMany<PlayerAggregateModel>(2)
//                .AsEnumerable().ToArray();
//            var playerGameTypeAggregateModels = fixture
//                .CreateMany<PlayerGameTypeAggregateModel>(2)
//                .AsEnumerable().ToArray();

//            var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));

//            _playerMock.Setup(e => e.GetPlayerAggregateQueries(null, false))
//                .Returns(playerAggregates);
//            _roundMock.Setup(
//                    e => e.GetPlayerAggregatesAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
//                .Returns(Task.FromResult((playerAggregateModels, playerGameTypeAggregateModels)));

//            var response = await _service.ExecutionAsync();

//            response.Error.Should().Be(0);
//            response.ErrorMessage.Should().BeEmpty();
//        }
//    }
//}

