using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Report;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;

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
                _mediatorMock.Object);
        }

        private MarketController _controller;
        private readonly Mock<IMediator> _mediatorMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Markets(bool showAll)
        {
            var list = CreateMarketModelFixture()
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

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetMarketsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResponse<MarketModel> {Valid = true, Items = expectedRoles});

            var response = await _controller.GetMarketsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].ReportDate.Should().Be(expectedRoles[i].ReportDate);
            }

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetMarketsRequest>(), It.IsAny<CancellationToken>()));
        }

        private Fixture CreateMarketModelFixture()
        {
            var fixture = new Fixture();
            fixture.Create<MarketModel>();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}