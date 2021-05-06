using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Report;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Market;
using TGS.Core.Models.Messages;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class MarketControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new(
                _clientMock.Object);
        }

        private MarketReportController _controller;
        private readonly Mock<IReportClient> _clientMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Markets(bool showAll)
        {
            var list = new Fixture()
                .CreateMany<MarketModel>(99)
                .ToArray();

            var request = new GetMarketsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };
            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.ReportDate);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _clientMock
                .Setup(m => m.GetMarketsAsync(It.IsAny<GetMarketsRequest>()))
                .ReturnsAsync(new PagedResponse<MarketModel> {Valid = true, Items = expectedRoles});

            var response = await _controller.GetMarketsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].ReportDate.Should().Be(expectedRoles[i].ReportDate);
            }

            _clientMock.Verify(x => x.GetMarketsAsync(It.IsAny<GetMarketsRequest>()));
        }
    }
}