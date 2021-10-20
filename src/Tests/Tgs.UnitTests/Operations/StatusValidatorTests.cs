using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.Tests.TestsCommon;

namespace TGS.UnitTests.Operations
{
    [TestFixture]
    [Category(Named.Unit)]
    [Parallelizable]
    public sealed class StatusValidatorTests : NUnitTestsBase
    {
        private Mock<IBrandOperations> _brandOperationsMock;
        private Mock<IGameProviderOperations> _gameProviderOperationsMock;
        private IStatusValidator _statusValidator;

        [Test]
        public async Task ThrowIfBrandStatusIsNotOneOfAsync_Ensures_Status()
        {
            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(Guid.NewGuid());
            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("No status provided");
        }

        [Test]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureGameStatus()
        {
            var status = new[] { Status.Active };
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<string>(), status, Array.Empty<GameStatus>());

            action.Should().ThrowAsync<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureStatus()
        {
            var gameStatus = new[] { GameStatus.Active };
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<string>(), Array.Empty<Status>(), gameStatus);

            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("No status provided");
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Maintenance, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Not_Throws_When_Ok(
            Status? gameProviderStatus,
            Status? brandGameProviderStatus,
            GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = true,
                SubGame = false
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().NotThrowAsync();
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Maintenance, Status.Maintenance,
            GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Not_Throws_When_Ok_If_SubGame_Not_Available_To_Brand(
            Status? gameProviderStatus,
            Status? brandGameProviderStatus,
            GameStatus? gameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = false,
                SubGame = true
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().NotThrowAsync();
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, GameStatus.Inactive,
            new[] { Status.Active }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, GameStatus.Inactive,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive, GameStatus.Inactive,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_GameStatus_Is_Not_One_OfAsync(
            Status? gameProviderStatus,
            Status? brandGameProviderStatus,
            GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses,
            GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = true,
                SubGame = false
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should()
                .ThrowAsync<InvalidStatusException>()
                .WithMessage($"Game '{externalGameId}' doesn't exist or is not active");
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,
            new[] { Status.Active }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active, GameStatus.Maintenance })]

        // @formatter:on
        public async Task
            ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_GameStatus_Is_Not_One_Of_If_SubGame_Not_Available_To_Brand(
                Status? gameProviderStatus,
                Status? brandGameProviderStatus,
                GameStatus? gameStatus,
                Status[] expectedStatuses,
                GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = false,
                SubGame = true
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should()
                .ThrowAsync<InvalidStatusException>()
                .WithMessage($"Game '{externalGameId}' doesn't exist or is not active");
        }

        // @formatter:off
        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of(
            Status? gameProviderStatus,
            Status? brandGameProviderStatus,
            GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses,
            GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = true,
                SubGame = false
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should()
                .ThrowAsync<InvalidStatusException>()
                .WithMessage("GameProvider doesn't exist or is not active");
        }

        // @formatter:off
        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active,
            new[] { Status.Active, Status.Maintenance }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task
            ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of_If_SubGame_Not_Available_To_Brand(
                Status? gameProviderStatus,
                Status? brandGameProviderStatus,
                GameStatus? gameStatus,
                Status[] expectedStatuses,
                GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = false,
                SubGame = true
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should()
                .ThrowAsync<InvalidStatusException>()
                .WithMessage("GameProvider doesn't exist or is not active");
        }

        // @formatter:off
        [TestCase(null, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Active,
            null, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, null,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, null,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = gameStatus == null
                ? null
                : new CachedGame
                {
                    Id = gameId,
                    ExternalId = externalGameId,
                    Status = gameStatus.Value,
                    AvailableToBrand = true,
                    SubGame = false
                };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        // @formatter:off
        [TestCase(null, Status.Active,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Active,
            null,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, null,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task
            ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null_If_SubGame_Not_Available_To_Brand(
                Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
                Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = gameStatus == null
                ? null
                : new CachedGame
                {
                    Id = gameId,
                    ExternalId = externalGameId,
                    Status = gameStatus.Value,
                    AvailableToBrand = false,
                    SubGame = true
                };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        // @formatter:off
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus, Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = true,
                SubGame = false
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<GameProviderUnderMaintenanceException>();
        }

        // @formatter:off
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task
            ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException_If_SubGame_Not_Available_To_Brand(
                Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
                Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = false,
                SubGame = true
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<GameProviderUnderMaintenanceException>();
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Maintenance, GameStatus.Active,
            new[] { Status.Active }, new[] { GameStatus.Active })]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, GameStatus.Maintenance,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameUnderMaintenanceException(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = true,
                SubGame = false
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<GameUnderMaintenanceException>();
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Maintenance,
            new[] { Status.Active }, new[] { GameStatus.Active })]
        // @formatter:on
        public async Task
            ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameUnderMaintenanceException_If_SubGame_Is_Not_Available_To_Brand(
                Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
                Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            var cachedGame = new CachedGame
            {
                Id = gameId,
                ExternalId = externalGameId,
                Status = gameStatus.Value,
                AvailableToBrand = false,
                SubGame = true
            };

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetCachedGameAsync(gpId, gameId))
                .Returns(Task.FromResult(cachedGame));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            await action.Should().ThrowAsync<GameUnderMaintenanceException>();
        }

        [Test]
        public async Task ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Ensures_Status()
        {
            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid.NewGuid(),
                    Guid.NewGuid());
            await action.Should().ThrowAsync<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public async Task ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult((Status?)Status.Inactive));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult((Status?)Status.Inactive));

            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId,
                    Status.Active);
            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        [TestCase(null, Status.Active)]
        [TestCase(Status.Active, null)]
        public async Task ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(
            Status? gameProviderStatus, Status? gameProviderConfigurationStatus)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult(gameProviderConfigurationStatus));

            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId,
                    Status.Active);
            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        [TestCase(Status.Maintenance, Status.Active)]
        [TestCase(Status.Active, Status.Maintenance)]
        public async Task ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(
            Status? gameProviderStatus, Status? gameProviderConfigurationStatus)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult(gameProviderConfigurationStatus));

            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId,
                    Status.Active);

            await action.Should().ThrowAsync<GameProviderUnderMaintenanceException>();
        }

        [Test]
        public async Task ThrowIfLicenseeStatusIsNotOneOfAsync_Not_Throws_When_Ok()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Active));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task
            ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Inactive));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        [Test]
        public async Task ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Null()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)null));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            await action.Should().ThrowAsync<InvalidStatusException>();
        }

        [Test]
        public async Task ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_LicenseeUnderMaintenanceException()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Maintenance));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            await action.Should().ThrowAsync<BrandUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfPlayerStatusIsDisabled_Not_Throws_When_Ok()
        {
            Action action = () => _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);
            action.Should().NotThrow();
        }

        [Test]
        public void ThrowIfPlayerStatusIsDisabled_Throws_PlayerDisabledException()
        {
            var disabled = PlayerStatus.Disabled;
            Action action = () => _statusValidator.ThrowIfPlayerDisabled(disabled);
            action.Should().Throw<PlayerDisabledException>().And.DisabledStatus.Should().Be(disabled);
        }

        protected override void InitializeBeforeEachTest()
        {
            _brandOperationsMock = new();
            _gameProviderOperationsMock = new();
            _statusValidator = new StatusValidator(_brandOperationsMock.Object, _gameProviderOperationsMock.Object);
        }
    }
}
