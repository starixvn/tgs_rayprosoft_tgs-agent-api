#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NSubstitute;
using NUnit.Framework;
using Tgs.PlayerService.Client;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;
using CoreEnums = TGS.Core.Models.Enums;
using PlayerInfo = TGS.Services.Operator.PlayerInfo;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerInfoTests
    {
        [Test]
        public async Task Check_Player_Status_InBlacklist_Should_Return_True()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var playerCode = "playerCode";
            var player = new Player
            {
                PlayerCode = playerCode,
                Status = CoreEnums.PlayerStatus.InBlacklist
            };

            fakePlayerOperations
                .GetPlayerCodeByUsernameAsync(Arg.Any<Guid>(), "UserName")
                .Returns(playerCode);

            fakePlayerOperations
                .GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(player);

            var sut = new CheckPlayerBannedQueryService(fakePlayerOperations);

            var command = new CheckPlayerBannedQuery(
                Guid.NewGuid(),
                "UserName");
            Assert.IsTrue(await sut.GetAsync(command));
        }

        [Test]
        public async Task Get_Player_Status_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var brandId = Guid.NewGuid();
            var playerCode = "playerCode";

            repository.Players.Add(new Player
            {
                PlayerCode = playerCode,
                Status = CoreEnums.PlayerStatus.Disabled,
                DefaultWalletCurrencyCode = "CNY",
                Password = Array.Empty<byte>(),
                Salt = Array.Empty<byte>(),
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                Username = "tester"
            });

            await repository.SaveChangesAsync();
            var sut = new GetPlayerStatusQueryService(repository);

            var query = new GetPlayerStatusQuery(playerCode);
            var actual = await sut.GetAsync(query);
            var expected = CoreEnums.PlayerStatus.Disabled;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Query_Player_By_UserName_Should_Success()
        {
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var sut = new GetPlayerInfoByUserNameQueryService(fakePlayerRepository);

            var playerInfo = new PlayerInfo(
                "playerCode",
                Guid.NewGuid(),
                100,
                false,
                false,
                null,
                null,
                Guid.NewGuid(),
                CoreEnums.PlayerStatus.Enabled,
                "username",
                null,
                null,
                1,
                null,
                null,
                false);

            fakePlayerRepository
                .GetPlayerInfoByUserNameAsync(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(playerInfo);

            var brandId = Guid.NewGuid();
            var username = "username";
            var query = new GetPlayerInfoByUserNameQuery(brandId, username);

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(playerInfo, actual);
        }

        [Test]
        public async Task Query_Player_Info_Should_Call_Get_Player_Main_Data()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new GetPlayerMainDataQueryService(moqPlayerServiceClient.Object);

            var playerCode = "abcdef123456";

            var query = new GetPlayerMainDataQuery(playerCode);

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            moqPlayerServiceClient.Verify(
                fake => fake.GetPlayerInfoAsync(
                    It.Is<string>(x => x == playerCode),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Query_Player_Info_Should_Call_Get_Player_Properties()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new GetPlayerPropertiesQueryService(moqPlayerServiceClient.Object);

            var playerCode = "abcdef123456";

            var query = new GetPlayerPropertiesQuery(playerCode);

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            moqPlayerServiceClient.Verify(
                fake => fake.GetPlayerPropertiesAsync(
                    It.Is<string>(x => x == playerCode),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Query_Player_Info_Should_Get_Player_VipLevel_When_VipLevelId_Is_Not_Null()
        {
            var moqVipLevelRepository = new Mock<IVipLevelRepository>();

            var sut = new GetPlayerVipLevelQueryService(moqVipLevelRepository.Object);

            var vipLevelId = Guid.NewGuid();

            var query = new GetPlayerVipLevelQuery(vipLevelId);

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            moqVipLevelRepository
                .Verify(fake =>
                        fake.GetVipLevelAsync(
                            It.Is<Guid>(x => x == vipLevelId)),
                    Times.Exactly(1));
        }

        [Test]
        public async Task Query_Player_Info_Should_Not_Get_Player_VipLevel_When_VipLevelId_Is_Null()
        {
            var moqVipLevelRepository = new Mock<IVipLevelRepository>();

            var sut = new GetPlayerVipLevelQueryService(moqVipLevelRepository.Object);

            var query = new GetPlayerVipLevelQuery(null);

            _ = await sut.GetAsync(query).ConfigureAwait(false);

            moqVipLevelRepository
                .Verify(fake =>
                        fake.GetVipLevelAsync(
                            It.IsAny<Guid>()),
                    Times.Never);
        }

        [Test]
        public async Task Set_Player_Birthdate_Should_SuccessAsync()
        {
            var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
            var sut = new SetBirthdateCommandService(fakePlayerServiceClient);

            var command = new SetBirthdateCommand("player1", DateOnly.Parse("2022/4/26"));

            await sut.ExecuteAsync(command);

            _ = fakePlayerServiceClient.Received(1)
                .SetPlayerPropertiesAsync(
                    Arg.Is(command.PlayerCode),
                    Arg.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        props => props[PlayerPropertyName.Birthdate] == "2022/04/26"));
        }

        [Test]
        public async Task Set_Player_Facebook_Messenger_Id_Should_Success()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new SetFacebookMessengerCommandService(moqPlayerServiceClient.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetFacebookMessengerCommand(player.PlayerCode, "this_is_messenger_id");

            await sut.ExecuteAsync(command);

            moqPlayerServiceClient.Verify(
                fake => fake.SetPlayerPropertiesAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        x => x.Count == 1
                            && x.ContainsKey(PlayerPropertyName.FacebookMessenger)
                            && x[PlayerPropertyName.FacebookMessenger] == command.FacebookMessengerId),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_Line_Id_Should_Success()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new SetLineCommandService(moqPlayerServiceClient.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetLineCommand(player.PlayerCode, "this_is_line_id");

            await sut.ExecuteAsync(command);

            moqPlayerServiceClient.Verify(
                fake => fake.SetPlayerPropertiesAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        x => x.Count == 1
                            && x.ContainsKey(PlayerPropertyName.Line)
                            && x[PlayerPropertyName.Line] == command.LineId),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_QQ_Number_Should_Success()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new SetQqCommandService(moqPlayerOperations.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetQqCommand(player.PlayerCode, "this_is_qq_number");

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(
                    It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            await sut.ExecuteAsync(command);

            moqPlayerOperations
                .Verify(
                    fake => fake.SetQqAsync(
                        It.Is<string>(x => x == player.PlayerCode),
                        It.Is<string>(x => x == command.QqId),
                        It.IsAny<DateTimeOffset>()),
                    Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_Telegram_Number_Should_Success()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new SetTelegramCommandService(moqPlayerServiceClient.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetTelegramCommand(player.PlayerCode, "this_is_telegram_number");

            await sut.ExecuteAsync(command);

            moqPlayerServiceClient.Verify(
                fake => fake.SetPlayerPropertiesAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        x => x.Count == 1
                            && x.ContainsKey(PlayerPropertyName.Telegram)
                            && x[PlayerPropertyName.Telegram] == command.TelegramId),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_Viber_Id_Should_Success()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new SetViberCommandService(moqPlayerServiceClient.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetViberCommand(player.PlayerCode, "this_is_viber_id");

            await sut.ExecuteAsync(command);

            moqPlayerServiceClient.Verify(
                fake => fake.SetPlayerPropertiesAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        x => x.Count == 1
                            && x.ContainsKey(PlayerPropertyName.Viber)
                            && x[PlayerPropertyName.Viber] == command.ViberId),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_WeChat_Number_Should_Success()
        {
            var moqPlayerOperations = new Mock<IPlayerOperations>();

            var sut = new SetWeChatCommandService(moqPlayerOperations.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetWeChatCommand(player.PlayerCode, "this_is_wechat_number");

            moqPlayerOperations
                .Setup(fake => fake.GetPlayerByCodeAsync(
                    It.Is<string>(x => x == player.PlayerCode)))
                .Returns(Task.FromResult(player));

            await sut.ExecuteAsync(command);

            moqPlayerOperations
                .Verify(
                    fake => fake.SetWeChatAsync(
                        It.Is<string>(x => x == player.PlayerCode),
                        It.Is<string>(x => x == command.WeChatId),
                        It.IsAny<DateTimeOffset>()),
                    Times.Exactly(1));
        }

        [Test]
        public async Task Set_Player_WhatsApp_Id_Should_Success()
        {
            var moqPlayerServiceClient = new Mock<IPlayerServiceClient>();

            var sut = new SetWhatsAppCommandService(moqPlayerServiceClient.Object);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                DefaultWalletCurrencyCode = "THB"
            };

            var command = new SetWhatsAppCommand(player.PlayerCode, "this_is_whatsapp_id");

            await sut.ExecuteAsync(command);

            moqPlayerServiceClient.Verify(
                fake => fake.SetPlayerPropertiesAsync(
                    It.Is<string>(x => x == player.PlayerCode),
                    It.Is<IReadOnlyDictionary<PlayerPropertyName, string?>>(
                        x => x.Count == 1
                            && x.ContainsKey(PlayerPropertyName.WhatsApp)
                            && x[PlayerPropertyName.WhatsApp] == command.WhatsAppId),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }
    }
}
