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
    public sealed class ExpirationValidatorTests : NUnitTestsBase
    {
        private Mock<IPlayerOperations> _playerOperationsMock;
        private IExpirationValidator _expirationValidator;

        protected override void InitializeBeforeEachTest()
        {
            _playerOperationsMock = new();
            _expirationValidator = new ExpirationValidator(_playerOperationsMock.Object);
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_No_Throws_When_Ok(PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlayerCode = playerCode,
                PlatformType = platform ?? PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            _playerOperationsMock.Setup(x => x.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform))
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().NotThrow();
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_No_Enabled_LoginRecord(
            PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();

            _playerOperationsMock.Setup(x => x.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform))
                .Returns(Task.FromResult((CachedPlayerLoginRecord) null));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().Throw<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login record is expired");
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired(
            PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlayerCode = playerCode,
                PlatformType = platform ?? PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
            };

            _playerOperationsMock.Setup(x => x.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform))
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().Throw<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login record is expired");
        }

        [Test]
        public void ThrowIfPlayerLoginExpiredAsync_With_Id_No_Throws_When_Ok()
        {
            var fixture = new Fixture();
            var loginRecordId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlayerCode = playerCode,
                PlatformType = PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            _playerOperationsMock.Setup(x => x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            action.Should().NotThrow();
        }

        [Test]
        public void
            ThrowIfPlayerLoginExpiredAsync_With_Id_Throws_PlayerLoginExpiredException_When_No_Enabled_LoginRecord()
        {
            var fixture = new Fixture();
            var loginRecordId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();

            _playerOperationsMock.Setup(x => x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult((CachedPlayerLoginRecord) null));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            action.Should().Throw<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login record is expired");
        }

        [Test]
        public void
            ThrowIfPlayerLoginExpiredAsync_With_Id_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired()
        {
            var fixture = new Fixture();
            var loginRecordId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlayerCode = playerCode,
                PlatformType = PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
            };

            _playerOperationsMock.Setup(x => x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            action.Should().Throw<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login record is expired");
        }

        [Test]
        public void
            ThrowIfPlayerLoginExpiredAsync_With_Id_Throws_PlayerLoginExpiredException_When_LoginRecord_PlayerCode_Is_Not_Matched()
        {
            var fixture = new Fixture();
            var loginRecordId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlayerCode = fixture.Create<string>(),
                PlatformType = PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
            };

            _playerOperationsMock.Setup(x => x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            action.Should().Throw<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login record is expired");
        }
    }
}