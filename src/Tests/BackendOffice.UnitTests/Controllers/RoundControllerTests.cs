using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Report;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class RoundControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new(
                _clientMock.Object);
        }

        private RoundReportController _controller;
        private readonly Mock<IReportClient> _clientMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Rounds(bool showAll)
        {
            var list = new Fixture()
                .CreateMany<RoundModel>(99)
                .ToArray();

            var request = new GetRoundsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };
            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.CreatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _clientMock
                .Setup(m => m.GetRoundsAsync(It.IsAny<GetRoundsRequest>()))
                .ReturnsAsync(new PagedResponse<RoundModel> {Valid = true, Items = expectedRoles});

            var response = await _controller.GetRoundsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].ExternalRoundId.Should().Be(expectedRoles[i].ExternalRoundId);
            }

            _clientMock.Verify(x => x.GetRoundsAsync(It.IsAny<GetRoundsRequest>()));
        }
    }
}