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

namespace TGS.UnitTests.Operations
{
    [TestFixture]
    [Category(Named.Unit)]
    [Parallelizable]
    public sealed class StatusValidatorTests : NUnitTestsBase
    {
        private IStatusValidator _statusValidator;
        private Mock<IBrandOperations> _brandOperationsMock;
        private Mock<IGameProviderOperations> _gameProviderOperationsMock;

        protected override void InitializeBeforeEachTest()
        {
            _brandOperationsMock = new Mock<IBrandOperations>();
            _gameProviderOperationsMock = new Mock<IGameProviderOperations>();
            _statusValidator = new StatusValidator(_brandOperationsMock.Object, _gameProviderOperationsMock.Object);
        }

        [Test]
        public void ThrowIfBrandStatusIsNotOneOfAsync_Ensures_Status()
        {
            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(Guid.NewGuid());
            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Null()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?) null));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void
            ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_InvalidStatusException_When_Licensee_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?) Status.Inactive));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Throws_LicenseeUnderMaintenanceException()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?) Status.Maintenance));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
                licenseeId,
                Status.Active);

            action.Should().Throw<BrandUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfLicenseeStatusIsNotOneOfAsync_Not_Throws_When_Ok()
        {
            var fixture = new Fixture();
            var licenseeId = fixture.Create<Guid>();
            _brandOperationsMock.Setup(x => x.GetBrandStatusAsync(licenseeId))
                .Returns(Task.FromResult((Status?) Status.Active));

            Func<Task> action = async () => await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(
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
            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid.NewGuid(),
                    Guid.NewGuid());
            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [TestCase(null, Status.Active)]
        [TestCase(Status.Active, null)]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(
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
            action.Should().Throw<InvalidStatusException>();
        }

        [Test]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of()
        {
            var fixture = new Fixture();
            var gpId = fixture.Create<Guid>();
            var gpcId = fixture.Create<Guid>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult((Status?) Status.Inactive));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult((Status?) Status.Inactive));

            Func<Task> action = async () =>
                await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId,
                    Status.Active);
            action.Should().Throw<InvalidStatusException>();
        }

        [TestCase(Status.Maintenance, Status.Active)]
        [TestCase(Status.Active, Status.Maintenance)]
        public void ThrowIfGameProviderConfigurationStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(
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

            action.Should().Throw<GameProviderUnderMaintenanceException>();
        }

        [Test]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureStatus()
        {
            var gameStatus = new[] {GameStatus.Active};
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<string>(), Array.Empty<Status>(), gameStatus);

            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        [Test]
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_EnsureGameStatus()
        {
            var status = new[] {Status.Active};
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<string>(), status, Array.Empty<GameStatus>());

            action.Should().Throw<ArgumentException>().WithMessage("No status provided");
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Maintenance, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Maintenance,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Maintenance,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Maintenance, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Maintenance,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Maintenance,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Maintenance, GameStatus.Maintenance,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Not_Throws_When_Ok(Status? gameProviderStatus,
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

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().NotThrow();
        }

        // @formatter:off
        [TestCase(null, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Active, Status.Active,
            null, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Active, null,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
       
        [TestCase(Status.Active, Status.Active,
            GameStatus.Active, null,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Null(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>();
        }

        // @formatter:off
        [TestCase(Status.Maintenance, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Active, Status.Maintenance,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameProviderUnderMaintenanceException(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus, Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<GameProviderUnderMaintenanceException>();
        }

        // @formatter:off
        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Inactive, Status.Active,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active})]
        
        [TestCase(Status.Active, Status.Inactive,
            GameStatus.Active, GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active})]
        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_Status_Is_Not_One_Of(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus, Status[] expectedStatuses,
            GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>()
                .WithMessage("GameProvider doesn't exist or is not active");
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Maintenance,GameStatus.Active, 
            new[] {Status.Active},  new[] {GameStatus.Active})]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active,GameStatus.Maintenance, 
            new[] {Status.Active},  new[] {GameStatus.Active})]
        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_GameUnderMaintenanceException(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus,
            Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));

            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));
            
            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<GameUnderMaintenanceException>();
        }

        // @formatter:off
        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,GameStatus.Active,
            new[] {Status.Active}, new[] {GameStatus.Active, GameStatus.Maintenance})]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active,GameStatus.Inactive,
            new[] {Status.Active}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,GameStatus.Active,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]

        [TestCase(Status.Active, Status.Active,
            GameStatus.Active,GameStatus.Inactive,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]
        
        [TestCase(Status.Active, Status.Active,
            GameStatus.Inactive,GameStatus.Inactive,
            new[] {Status.Active, Status.Maintenance}, new[] {GameStatus.Active, GameStatus.Maintenance})]

        // @formatter:on
        public void ThrowIfGameOrGameProviderStatusIsNotOneOfAsync_Throws_Exception_When_GameStatus_Is_Not_One_Of(
            Status? gameProviderStatus, Status? brandGameProviderStatus, GameStatus? gameStatus,
            GameStatus? brandGameStatus, Status[] expectedStatuses, GameStatus[] expectedGameStatuses)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var gpId = fixture.Create<Guid>();
            var gameId = fixture.Create<int>();
            var externalGameId = fixture.Create<string>();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
                .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameStatusAsync(gpId, gameId))
                .Returns(Task.FromResult(gameStatus));
           
            _brandOperationsMock.Setup(x => x.GetBrandGameProviderStatusAsync(brandId, gpId))
                .Returns(Task.FromResult(brandGameProviderStatus));
            _brandOperationsMock.Setup(x => x.GetBrandGameStatusAsync(brandId, gpId, gameId))
                .Returns(Task.FromResult(brandGameStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                brandId, gpId, gameId, externalGameId, expectedStatuses, expectedGameStatuses);

            action.Should().Throw<InvalidStatusException>()
                .WithMessage($"Game '{externalGameId}' doesn't exist or is not active");
        }
    }
}