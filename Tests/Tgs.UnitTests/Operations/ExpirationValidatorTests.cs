using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Tests.TestsCommon;
using UGS.Core.Models.Exceptions;

namespace Tgs.UnitTests.Operations
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public sealed class ExpirationValidatorTests : NUnitTestsBase
    {
        private IPlayerOperations _playerOperations;
        private IExpirationValidator _expirationValidator;

        protected override void InitializeBeforeEachTest()
        {
            _playerOperations = Substitute.For<IPlayerOperations>();
            _expirationValidator = new ExpirationValidator(_playerOperations);
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_No_Throws_When_Ok(PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlatformType = platform ?? PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform)
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () => await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().NotThrow();
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_No_Enabled_LoginRecord(PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();

            _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform)
                .Returns(Task.FromResult((CachedPlayerLoginRecord)null));

            Func<Task> action = async () => await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().Throw<PlayerLoginExpiredException>().WithMessage($"Player({playerCode}) login record is expired");
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public void ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired(PlatformType? platform)
        {
            var fixture = new Fixture();
            var playerCode = fixture.Create<string>();
            var loginRecord = new CachedPlayerLoginRecord
            {
                PlatformType = platform ?? PlatformType.Desktop,
                ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
            };

            _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform)
                .Returns(Task.FromResult(loginRecord));

            Func<Task> action = async () => await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            action.Should().Throw<PlayerLoginExpiredException>().WithMessage($"Player({playerCode}) login record is expired");
        }
    }
}
