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
        private Mock<IAgentOperations> _agentOperationsMock;
        private IExpirationValidator _expirationValidator;
        private Mock<IPlayerOperations> _playerOperationsMock;

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public async Task ThrowIfPlayerLoginExpiredAsync_No_Throws_When_Ok(PlatformType? platform)
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

            await action.Should().NotThrowAsync();
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public async Task ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired(
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

            await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public async Task ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_No_Enabled_LoginRecord(
            PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();

            _playerOperationsMock.Setup(x => x.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform))
                .Returns(Task.FromResult((CachedPlayerLoginRecord)null));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        [Test]
        public async Task ThrowIfPlayerLoginExpiredAsync_With_Id_No_Throws_When_Ok()
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

            await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task
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

            await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        [Test]
        public async Task
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

            await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        [Test]
        public async Task
            ThrowIfPlayerLoginExpiredAsync_With_Id_Throws_PlayerLoginExpiredException_When_No_Enabled_LoginRecord()
        {
            var fixture = new Fixture();
            var loginRecordId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();

            _playerOperationsMock.Setup(x => x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult((CachedPlayerLoginRecord)null));

            Func<Task> action = async () =>
                await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        protected override void InitializeBeforeEachTest()
        {
            _playerOperationsMock = new();
            _agentOperationsMock = new();
            _expirationValidator = new ExpirationValidator(_playerOperationsMock.Object, _agentOperationsMock.Object);
        }
    }
}
