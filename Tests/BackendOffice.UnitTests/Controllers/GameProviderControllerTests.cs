using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.GameProvider;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Resources;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class GameProviderControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new GameProviderController(
                _mediatorMock.Object);
        }

        private GameProviderController _controller;
        private readonly Mock<IMediator> _mediatorMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_GameProviders(bool showAll)
        {
            var list = CreateGameProviderModelFixture()
                .CreateMany<GameProviderModel>(99)
                .ToArray();

            var request = new GetGameProvidersRequest
            {
                Code = "Code",
                Name = "Name",
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };
            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetGameProvidersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResponse<GameProviderModel>
                {
                    Valid = true,
                    Items = expectedRoles
                });

            var response = await _controller.GetGameProvidersAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            }

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetGameProvidersRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Cannot_Create_GameProvider_With_Name_Exists()
        {
            var list = CreateGameProviderModelFixture()
                .CreateMany<GameProviderModel>(99)
                .ToArray();

            var request = new Fixture().Create<CreateGameProviderRequest>();
            request.Name = list.FirstOrDefault()?.Name;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateGameProviderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GameProviderResponse {Valid = false, Message = GameProvider.CodeExisted});

            var response = await _controller.CreateGameProviderAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(GameProvider.CodeExisted);

            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateGameProviderRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Can_Create_GameProvider()
        {
            var request = new Fixture().Create<CreateGameProviderRequest>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateGameProviderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GameProviderResponse {Valid = true});

            var response = await _controller.CreateGameProviderAsync(request);
            response.Valid.Should().BeTrue();

            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateGameProviderRequest>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Can_Update_GameProvider()
        {
            var brand = CreateGameProviderModelFixture().Create<GameProviderModel>();
            var request = new Fixture().Create<UpdateGameProviderRequest>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateGameProviderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GameProviderResponse {Valid = true});

            var response = await _controller.UpdateGameProviderAsync(brand.Id, request);

            response.Valid.Should().BeTrue();
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateGameProviderRequest>(), It.IsAny<CancellationToken>()));
        }

        private Fixture CreateGameProviderModelFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<GameProviderModel>(u => u
                .With(s => s.Games, new GameProviderGameModel[] { })
                .With(s => s.Languages, new GameProviderLanguageModel[] { })
                .With(s => s.Currencies, new GameProviderCurrencyModel[] { }));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}