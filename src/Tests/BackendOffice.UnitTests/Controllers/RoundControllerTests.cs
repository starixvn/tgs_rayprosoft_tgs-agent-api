using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Report;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Game;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class RoundControllerTests : BaseTests
    {
        private readonly Mock<IReportClient> _clientMock = new();

        private RoundReportController _controller;

        [Test]
        public async Task Can_Get_Rounds()
        {
            var moqReportClient = new Mock<IReportClient>();

            var sut = new GetGameRoundListQueryService(moqReportClient.Object);

            var query = new GetGameRoundListQuery(
                Guid.NewGuid(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow,
                new PageableRequest
                {
                    Page = 1,
                    Rows = 10,
                    SortBy = "updatedOn",
                    OrderBy = SortOrder.Descending,
                    ShowAll = false
                });

            await sut.GetAsync(query);

            moqReportClient.Verify(
                fake => fake.GetRoundsAsync(It.IsAny<GetRoundsRequest>()),
                Times.Exactly(1));
        }

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new(
                _clientMock.Object);
        }
    }
}
