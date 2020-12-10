using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Brand;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class BrandGameProviderScheduleControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new BrandGameProviderScheduleController(
                _mediatorMock.Object);
        }

        private BrandGameProviderScheduleController _controller;
        private readonly Mock<IMediator> _mediatorMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Schedules(bool showAll)
        {
            var list = CreateScheduleModelFixture()
                .CreateMany<ScheduleModel>(99)
                .ToArray();

            var request = new GetSchedulesRequest
            {
                Keyword = "keyword",
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };
            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.GameProviderName);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSchedulesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResponse<ScheduleModel>
                {
                    Valid = true,
                    Items = expectedRoles
                });

            var response = await _controller.GetBrandGameProviderSchedulesAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].GameProviderName.Should().Be(expectedRoles[i].GameProviderName);
            }

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetSchedulesRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Can_Get_Default_Times()
        {
            var list = CreateScheduleModelFixture()
                .CreateMany<DefaultTimeModel>(99)
                .ToArray();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetDefaultTimesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetDefaultTimesResponse
                {
                    Valid = true,
                    DefaultTimes = list
                });

            var response = await _controller.GetBrandGameProviderDefaultTimesAsync(It.IsAny<Guid>());
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.DefaultTimes.Length; i++)
            {
                response.DefaultTimes[i].From.Should().Be(list[i].From);
                response.DefaultTimes[i].To.Should().Be(list[i].To);
            }

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetDefaultTimesRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Can_Update_Schedule()
        {
            var schedule = CreateScheduleModelFixture().Create<ScheduleModel>();
            var request = new Fixture().Create<UpdateScheduleRequest>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateScheduleRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MessageResponse {Valid = true});

            var response = await _controller.UpdateBrandGameProviderScheduleAsync(schedule.Id, request);

            response.Valid.Should().BeTrue();
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateScheduleRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Can_Update_Default_Times()
        {
            var request = new Fixture().Create<UpdateDefaultTimesRequest>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateDefaultTimesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MessageResponse {Valid = true});

            var response =
                await _controller.UpdateBrandGameProviderDefaultTimesAsync(request.BrandGameProviderId, request);

            response.Valid.Should().BeTrue();
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateDefaultTimesRequest>(), It.IsAny<CancellationToken>()));
        }

        private Fixture CreateScheduleModelFixture()
        {
            var fixture = new Fixture();
            return fixture;
        }
    }
}