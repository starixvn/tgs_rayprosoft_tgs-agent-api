using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Report;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using TGS.Services.BackOffice.Application.Models.ReportMarket;
using Tgs.ServiceUtility;

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
            _controller = new();
        }

        private MarketReportController _controller;
        private readonly Mock<IReportClient> _clientMock = new();

        private readonly Mock<IGetService<GetMarketsReportRequest, PagedResponse<MarketReportModel>>> _getServiceMock =
            new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Markets(bool showAll)
        {
            var list = new Fixture()
                .CreateMany<MarketReportModel>(99)
                .ToArray();

            var request = new GetMarketsReportRequest
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

            _getServiceMock
                .Setup(m => m.GetAsync(It.IsAny<GetMarketsReportRequest>(), default))
                .ReturnsAsync(new PagedResponse<MarketReportModel>
                {
                    Valid = true,
                    Items = expectedRoles
                });

            var response = await _controller.GetMarketsAsync(_getServiceMock.Object, request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].ReportDate.Should().Be(expectedRoles[i].ReportDate);
            }

            _getServiceMock.Verify(x => x.GetAsync(It.IsAny<GetMarketsReportRequest>(), default));
        }
    }
}