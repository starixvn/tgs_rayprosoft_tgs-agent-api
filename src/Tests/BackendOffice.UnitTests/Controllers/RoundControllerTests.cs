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
using TGS.Services.Report.Application.Models.Round;

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
            _controller = new RoundController(
                _mediatorMock.Object);
        }

        private RoundController _controller;
        private readonly Mock<IMediator> _mediatorMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Rounds(bool showAll)
        {
            var list = CreateRoundModelFixture()
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

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRoundsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResponse<RoundModel>
                {
                    Valid = true,
                    Items = expectedRoles
                });

            var response = await _controller.GetRoundsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].ExternalRoundId.Should().Be(expectedRoles[i].ExternalRoundId);
            }

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetRoundsRequest>(), It.IsAny<CancellationToken>()));
        }

        private Fixture CreateRoundModelFixture()
        {
            var fixture = new Fixture();
            fixture.Create<RoundModel>();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}