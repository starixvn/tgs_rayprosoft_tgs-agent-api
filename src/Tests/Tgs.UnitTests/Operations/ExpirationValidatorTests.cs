using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
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
        private Mock<IPlayerQueries> _playerQueriesMock;

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public async Task ThrowIfPlayerLoginExpiredAsync_No_Throws_When_Ok(PlatformType? platform)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();

            var results = new GetPlayerLoginRecordResult[]
            {
                new()
                {
                    BrandId = brandId,
                    CreatedOn = DateTimeOffset.UtcNow.AddHours(-23),
                    PlatformType = platform ?? PlatformType.Desktop,
                    Status = LoginStatus.Enabled,
                    ExpiredOn = DateTimeOffset.UtcNow.AddHours(1)
                }
            };

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled))
                .ReturnsAsync(results);

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            _ = await action.Should().NotThrowAsync();
        }

        [TestCase(null)]
        [TestCase(PlatformType.Desktop)]
        public async Task ThrowIfPlayerLoginExpiredAsync_Throws_PlayerLoginExpiredException_When_LoginRecord_Is_Expired(
            PlatformType? platform)
        {
            var fixture = new Fixture();
            var brandId = fixture.Create<Guid>();
            var playerCode = fixture.Create<string>();

            var results = new GetPlayerLoginRecordResult[]
            {
                new()
                {
                    BrandId = brandId,
                    CreatedOn = DateTimeOffset.UtcNow.AddHours(-23),
                    PlatformType = platform ?? PlatformType.Desktop,
                    Status = LoginStatus.Enabled,
                    ExpiredOn = DateTimeOffset.UtcNow.AddHours(-1)
                }
            };

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled))
                .ReturnsAsync(results);

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            _ = await action.Should()
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

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled))
                .ReturnsAsync(Array.Empty<GetPlayerLoginRecordResult>());

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(playerCode, platform);

            _ = await action.Should()
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

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginRecordAsync(loginRecordId))
                .ReturnsAsync(loginRecord);

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            _ = await action.Should().NotThrowAsync();
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

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginRecordAsync(loginRecordId))
                .Returns(Task.FromResult(loginRecord));

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            _ = await action.Should()
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

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginRecordAsync(loginRecordId))
                .ReturnsAsync(loginRecord);

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            _ = await action.Should()
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

            _ = _playerQueriesMock.Setup(x =>
                    x.GetPlayerLoginRecordAsync(loginRecordId))
                 .ReturnsAsync((CachedPlayerLoginRecord)null);

            var sut = new ExpirationValidator(_playerQueriesMock.Object, _agentOperationsMock.Object);

            Func<Task> action = async () =>
                await sut.ThrowIfPlayerLoginExpiredAsync(loginRecordId, playerCode);

            _ = await action.Should()
                .ThrowAsync<PlayerLoginExpiredException>()
                .WithMessage($"Player({playerCode}) login token is expired");
        }

        protected override void InitializeBeforeEachTest()
        {
            _playerQueriesMock = new Mock<IPlayerQueries>();
            _agentOperationsMock = new Mock<IAgentOperations>();
        }
    }
}
