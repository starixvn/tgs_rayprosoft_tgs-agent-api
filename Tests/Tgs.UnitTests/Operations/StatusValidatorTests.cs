using System;
using System.Threading.Tasks;
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
            var licenseeId = Guid.NewGuid();
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
            var licenseeId = Guid.NewGuid();
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
            var licenseeId = Guid.NewGuid();
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
            var licenseeId = Guid.NewGuid();
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
            var gpId = Guid.NewGuid();
            var gpcId = Guid.NewGuid();

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
            var gpId = Guid.NewGuid();
            var gpcId = Guid.NewGuid();

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
            var gpId = Guid.NewGuid();
            var gpcId = Guid.NewGuid();

            _gameProviderOperationsMock.Setup(x => x.GetGameProviderStatusAsync(gpId))
               .Returns(Task.FromResult(gameProviderStatus));
            _gameProviderOperationsMock.Setup(x => x.GetGameProviderConfigurationStatusAsync(gpId, gpcId))
                .Returns(Task.FromResult(gameProviderConfigurationStatus));

            Func<Task> action = async () => await _statusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(gpId, gpcId, Status.Active);

            action.Should().Throw<GameProviderUnderMaintenanceException>();
        }
    }
}