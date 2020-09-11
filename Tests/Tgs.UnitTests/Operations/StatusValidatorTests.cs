using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.Tests.TestsCommon;

namespace Tgs.UnitTests.Operations
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public sealed class StatusValidatorTests : NUnitTestsBase
    {
        private IStatusValidator _statusValidator;
        private Mock<ILicenseeOperations> _licenseeOperationsMock;
        private Mock<IGameProviderOperations> _gameProviderOperationsMock;

        protected override void InitializeBeforeEachTest()
        {
            _licenseeOperationsMock = new Mock<ILicenseeOperations>();
            _gameProviderOperationsMock = new Mock<IGameProviderOperations>();
            _statusValidator = new StatusValidator(_licenseeOperationsMock.Object, _gameProviderOperationsMock.Object);
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Ensures_Status()
        {
            Func<Task> action = async () => await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(Guid.NewGuid(), new Status[0]);
            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Null()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _licenseeOperationsMock.Setup(x => x.GetLicenseeStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)null));

            Func<Task> action = async () => await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _licenseeOperationsMock.Setup(x => x.GetLicenseeStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Inactive));

            Func<Task> action = async () => await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_LicenseeUnderMaintenanceException()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _licenseeOperationsMock.Setup(x => x.GetLicenseeStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Maintenance));

            Func<Task> action = async () => await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<LicenseeUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Not_Throws_When_Ok()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _licenseeOperationsMock.Setup(x => x.GetLicenseeStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?)Status.Active));

            Func<Task> action = async () => await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().NotThrow();
        }

        [Test]
        public void ThrowIfPlayerStatusIsDisabled_Throws_PlayerDisabledException()
        {
            var disabled = PlayerStatus.Disabled;
            Action action = () => _statusValidator.ThrowIfPlayerDisabled(disabled);
            action.Should().Throw<PlayerDisabledException>().And.DisabledStatus.Should().Be(disabled);
        }

        [Test]
        public void ThrowIfPlayerStatusIsDisabled_Not_Throws_When_Ok()
        {
            Action action = () => _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);
            action.Should().NotThrow();
        }

        [Test]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Ensures_Status()
        {
            Func<Task> action = async () => await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid.NewGuid(), Guid.NewGuid(), new Status[0]);
            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [TestCase(null, Status.Active)]
        [TestCase(Status.Active, null)]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(Status? gameProviderStatus, Status? gameProviderConfigurationStatus)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult(gameProviderConfigurationStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId, Status.Active);
            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult((Status?)Status.Inactive));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult((Status?)Status.Inactive));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId, Status.Active);
            action.Should().Throw<InvalidStatusException>();
        }

        [TestCase(Status.Maintenance, Status.Active)]
        [TestCase(Status.Active, Status.Maintenance)]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(Status? gameProviderStatus, Status? gameProviderConfigurationStatus)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
               .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult(gameProviderConfigurationStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId, Status.Active);

            action.Should().Throw<GameProviderUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureStatus()
        {
            var gameStatus = new GameStatus[] { GameStatus.Active };
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(), Array.Empty<Status>(), gameStatus);

            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureGameStatus()
        {
            var status = new Status[] { Status.Active };
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(), status, Array.Empty<GameStatus>());

            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [TestCase(Status.Active, new Status[] { Status.Active }, GameStatus.Active, new GameStatus[] { GameStatus.Active })]
        [TestCase(Status.Maintenance, new Status[] { Status.Active, Status.Maintenance }, GameStatus.Maintenance, new GameStatus[] { GameStatus.Active, GameStatus.Maintenance })]
        [TestCase(Status.Maintenance, new Status[] { Status.Active, Status.Maintenance }, GameStatus.Active, new GameStatus[] { GameStatus.Active, GameStatus.Maintenance })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Not_Throws_When_Ok(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().NotThrow();
        }

        [TestCase(null, new Status[] { Status.Active }, GameStatus.Active, new GameStatus[] { GameStatus.Active })]
        [TestCase(Status.Active, new Status[] { Status.Active }, null, new GameStatus[] { GameStatus.Active })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>();
        }

        [TestCase(Status.Maintenance, new Status[] { Status.Active }, GameStatus.Maintenance, new GameStatus[] { GameStatus.Active, GameStatus.Maintenance, })]
        [TestCase(Status.Maintenance, new Status[] { Status.Active }, GameStatus.Active, new GameStatus[] { GameStatus.Active })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<GameProviderUnderMaintenanceException>();
        }

        [TestCase(Status.Inactive, new Status[] { Status.Active }, GameStatus.Active, new GameStatus[] { GameStatus.Active })]
        [TestCase(Status.Inactive, new Status[] { Status.Active, Status.Maintenance }, GameStatus.Active, new GameStatus[] { GameStatus.Active })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>()
                .WithMessage("GameProvider doesn't exist or is not active");
        }

        [TestCase(Status.Active, new Status[] { Status.Active }, GameStatus.Maintenance, new GameStatus[] { GameStatus.Active })]
        [TestCase(Status.Active, new Status[] { Status.Active, Status.Maintenance }, GameStatus.Maintenance, new GameStatus[] { GameStatus.Active })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameUnderMaintenanceException(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<GameUnderMaintenanceException>();
        }

        [TestCase(Status.Active, new Status[] { Status.Active }, GameStatus.Inactive, new GameStatus[] { GameStatus.Active, GameStatus.Maintenance })]
        [TestCase(Status.Active, new Status[] { Status.Active, Status.Maintenance }, GameStatus.Inactive, new GameStatus[] { GameStatus.Active, GameStatus.Maintenance })]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_GameStatus_Is_Not_One_Of(Status? status, Status[] expectedStatuses, GameStatus? gameStatus, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<Guid>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(status));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>()
                .WithMessage($"Game '{externalGameId}' doesn't exist or is not active");
        }
    }
}