using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using Tgs.Payment.Clients;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Withdraw;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankInfoTests
    {
        [Test]
        public async Task Player_Can_Create_BankInfo()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new CreateBankInfoCommandService(fakePaymentServiceClient);

            var brandId = Guid.NewGuid();
            var playerCode = "ThisIsPlayerCode";
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var command = new CreateBankInfoCommand(
                brandId,
                playerCode,
                paymentChannelId,
                bankType,
                JsonSerializer.SerializeToDocument(detail));

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreatePlayerBankInfoAsync(
                    Arg.Is(brandId),
                    Arg.Is(playerCode),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));
        }

        [Test]
        public async Task Player_Can_Delete_BankInfo()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new DeleteBankInfoCommandService(fakePaymentServiceClient);

            var playerCode = "ThisIsPlayerCode";
            var bankInfoId = Guid.NewGuid();
            var command = new DeleteBankInfoCommand(playerCode, bankInfoId);

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .DeletePlayerBankInfoAsync(Arg.Is(playerCode), Arg.Is(bankInfoId));
        }

        [Test]
        public void Player_Can_List_Self_BankInfos()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new GetPlayerBankInfosQueryService(fakePaymentServiceClient);

            var playerCode = "ThisIsPlayerCode";

            sut.QueryAsync(new GetPlayerBankInfosQuery(playerCode));

            _ = fakePaymentServiceClient.Received(1)
                .GetPlayerBankInfosAsync(Arg.Is(playerCode));
        }

        [Test]
        public async Task Player_Init_BankInfo_If_Brand_Not_Use_BankInfoRegisterFlow_Should_Not_Process()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();

            var sut = new CreateInitBankInfoCommandService(
                fakeBrandRepository,
                fakePlayerRepository,
                fakePaymentServiceClient,
                fakeRedisCacheManager);

            var playerCode = "ThisIsPlayerCode";
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var bankInfoId = Guid.NewGuid();

            var brandInfo = new BrandInfo(
                Guid.NewGuid(),
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                true,
                false,
                false,
                false,
                false);

            _ = fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandInfo.BrandId))
                .Returns(brandInfo);

            _ = fakePaymentServiceClient.CreatePlayerBankInfoAsync(
                Arg.Is(brandInfo.BrandId),
                Arg.Is(playerCode),
                Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId))
                .Returns(bankInfoId);

            var command = new CreateInitBankInfoCommand(
                brandInfo.BrandId,
                playerCode,
                paymentChannelId,
                bankType,
                JsonSerializer.SerializeToDocument(detail),
                "127.0.0.1",
                "localhost");

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .DidNotReceive()
                .CreatePlayerBankInfoAsync(
                    Arg.Is(brandInfo.BrandId),
                    Arg.Is(playerCode),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));

            _ = fakePaymentServiceClient
                .DidNotReceive()
                .SetDefaultPlayerBankInfoAsync(
                    Arg.Is(playerCode),
                    Arg.Is(bankInfoId),
                    Arg.Is(playerCode));

            _ = fakePlayerRepository
                .DidNotReceive()
                .UpdatePlayerStatusAsync(
                    Arg.Is(playerCode),
                    Arg.Is(PlayerStatus.Enabled));

            _ = fakeRedisCacheManager
                .DidNotReceive()
                .ClearCacheAsync(
                    Arg.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                    Arg.Is<string>(x => x == playerCode),
                    Arg.Any<bool>());

            _ = fakePlayerRepository
                .DidNotReceive()
                .PlayerRegisterIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository
                .DidNotReceive()
                .PlayerLoginIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Player_Init_BankInfo_If_Brand_Use_BankInfoRegisterFlow_Should_Record_Ip()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();

            var sut = new CreateInitBankInfoCommandService(
                fakeBrandRepository,
                fakePlayerRepository,
                fakePaymentServiceClient,
                fakeRedisCacheManager);

            var playerCode = "ThisIsPlayerCode";
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var bankInfoId = Guid.NewGuid();

            var brandInfo = new BrandInfo(
                Guid.NewGuid(),
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                true,
                false,
                false,
                false,
                true);

            _ = fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandInfo.BrandId))
                .Returns(brandInfo);

            _ = fakePaymentServiceClient.CreatePlayerBankInfoAsync(
                Arg.Is(brandInfo.BrandId),
                Arg.Is(playerCode),
                Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId))
                .Returns(bankInfoId);

            var command = new CreateInitBankInfoCommand(
                brandInfo.BrandId,
                playerCode,
                paymentChannelId,
                bankType,
                JsonSerializer.SerializeToDocument(detail),
                "127.0.0.1",
                "localhost");

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreatePlayerBankInfoAsync(
                    Arg.Is(brandInfo.BrandId),
                    Arg.Is(playerCode),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));

            _ = fakePaymentServiceClient
                .Received(1)
                .SetDefaultPlayerBankInfoAsync(
                    Arg.Is(playerCode),
                    Arg.Is(bankInfoId),
                    Arg.Is(playerCode));

            _ = fakePlayerRepository
                .Received(1)
                .PlayerRegisterIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository
                .Received(1)
                .PlayerLoginIpAsync(
                    Arg.Is(playerCode),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Player_Init_BankInfo_If_Brand_Use_BankInfoRegisterFlow_Should_Success()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();

            var sut = new CreateInitBankInfoCommandService(
                fakeBrandRepository,
                fakePlayerRepository,
                fakePaymentServiceClient,
                fakeRedisCacheManager);

            var playerCode = "ThisIsPlayerCode";
            var paymentChannelId = Guid.NewGuid();
            var bankType = "Transfer";
            var detail = new
            {
                bankAccount = "0987654321"
            };

            var bankInfoId = Guid.NewGuid();

            var brandInfo = new BrandInfo(
                Guid.NewGuid(),
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                true,
                false,
                false,
                false,
                true);

            _ = fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandInfo.BrandId))
                .Returns(brandInfo);

            _ = fakePaymentServiceClient.CreatePlayerBankInfoAsync(
                Arg.Is(brandInfo.BrandId),
                Arg.Is(playerCode),
                Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId))
                .Returns(bankInfoId);

            var command = new CreateInitBankInfoCommand(
                brandInfo.BrandId,
                playerCode,
                paymentChannelId,
                bankType,
                JsonSerializer.SerializeToDocument(detail),
                "127.0.0.1",
                "localhost");

            await sut.ExecuteAsync(command);

            _ = fakePaymentServiceClient
                .Received(1)
                .CreatePlayerBankInfoAsync(
                    Arg.Is(brandInfo.BrandId),
                    Arg.Is(playerCode),
                    Arg.Is<BankCreateInfo>(info => info.PaymentChannelId == paymentChannelId));

            _ = fakePaymentServiceClient
                .Received(1)
                .SetDefaultPlayerBankInfoAsync(
                    Arg.Is(playerCode),
                    Arg.Is(bankInfoId),
                    Arg.Is(playerCode));

            _ = fakePlayerRepository
                .Received(1)
                .UpdatePlayerStatusAsync(
                    Arg.Is(playerCode),
                    Arg.Is(PlayerStatus.Enabled));

            _ = fakeRedisCacheManager
                .Received(1)
                .ClearCacheAsync(
                    Arg.Is<object>(x => x.ToString() == nameof(RedisCacheType.Player)),
                    Arg.Is<string>(x => x == playerCode),
                    Arg.Any<bool>());
        }
    }
}
