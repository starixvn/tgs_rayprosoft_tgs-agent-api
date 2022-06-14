using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandCertificate.Clients;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.Payment.Clients;
using Tgs.PlayerService.Client;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operations.Queries;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Withdraw;
using TGS.Services.Operator.BrandPayment;
using TGS.Services.PlayerBucket;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public void Create_Withdraw_Form_Not_Found_Player_Will_Throw_Exception()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerService = Substitute.For<IPlayerServiceClient>();
            var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

            var sut = new CreateWithdrawFormCommandService(
                fakePaymentServiceClient,
                fakeCashServiceClient,
                fakePlayerService,
                fakePlayerBucketRepository);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "ThisIsPlayerNote");

            Assert.ThrowsAsync<PlayerNotFoundException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public void Player_Balance_Not_Enough_Withdraw_Will_Throw_Exception()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerService = Substitute.For<IPlayerServiceClient>();
            var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

            var sut = new CreateWithdrawFormCommandService(
                fakePaymentServiceClient,
                fakeCashServiceClient,
                fakePlayerService,
                fakePlayerBucketRepository);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "ThisIsPlayerNote");

            var walletId = Guid.NewGuid();

            fakePlayerService
                .GetPlayerInfoAsync(Arg.Is(command.PlayerCode))
                .Returns(new Tgs.PlayerService.Client.PlayerInfo
                {
                    PlayerCode = command.PlayerCode,
                    BrandId = command.BrandId,
                    IdentityAuthentication = false,
                    IsSetPayPassword = false,
                    PhoneAuthentication = false,
                    Status = Tgs.PlayerService.Client.PlayerStatus.Enabled,
                    UserName = "Tester",
                    VipLevelId = null
                });

            fakePlayerService
                .GetPlayerPropertiesAsync(Arg.Is(command.PlayerCode))
                .Returns(new Dictionary<string, string>
                {
                    ["DefaultWalletId"] = walletId.ToString()
                });

            fakeCashServiceClient
                .GetBalanceAsync(Arg.Any<Guid>())
                .Returns(800M);

            fakePlayerBucketRepository
                .IsBucketFullAsync(Arg.Any<string>())
                .Returns(true);

            var fakeBankInfo = new PlayerBankInfo
            {
                BankType = "Bank",
                BrandId = command.BrandId,
                Id = command.BankInfoId,
                PaymentChannelId = Guid.NewGuid(),
                Detail = JsonDocument.Parse("{}")
            };

            Assert.ThrowsAsync<InsufficientFundsException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public void Player_Bucket_Not_Full_Withdraw_Will_Throw_Exception()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerService = Substitute.For<IPlayerServiceClient>();
            var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

            var sut = new CreateWithdrawFormCommandService(
                fakePaymentServiceClient,
                fakeCashServiceClient,
                fakePlayerService,
                fakePlayerBucketRepository);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "ThisIsPlayerNote");

            fakePlayerService
                .GetPlayerInfoAsync(Arg.Is(command.PlayerCode))
                .Returns(new Tgs.PlayerService.Client.PlayerInfo
                {
                    PlayerCode = command.PlayerCode,
                    BrandId = command.BrandId,
                    IdentityAuthentication = false,
                    IsSetPayPassword = false,
                    PhoneAuthentication = false,
                    Status = Tgs.PlayerService.Client.PlayerStatus.Enabled,
                    UserName = "Tester",
                    VipLevelId = null
                });

            fakeCashServiceClient
                .GetBalanceAsync(Arg.Any<Guid>())
                .Returns(800M);

            fakePlayerBucketRepository
                .IsBucketFullAsync(Arg.Any<string>())
                .Returns(false);

            var fakeBankInfo = new PlayerBankInfo
            {
                BankType = "Bank",
                BrandId = command.BrandId,
                Id = command.BankInfoId,
                PaymentChannelId = Guid.NewGuid(),
                Detail = JsonDocument.Parse("{}")
            };

            Assert.ThrowsAsync<InsufficientBucketException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Player_Can_Create_Withdraw_Form()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerService = Substitute.For<IPlayerServiceClient>();
            var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

            var sut = new CreateWithdrawFormCommandService(
                fakePaymentServiceClient,
                fakeCashServiceClient,
                fakePlayerService,
                fakePlayerBucketRepository);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "ThisIsPlayerNote");

            var walletId = Guid.NewGuid();

            fakePlayerService
                .GetPlayerInfoAsync(Arg.Is(command.PlayerCode))
                .Returns(new Tgs.PlayerService.Client.PlayerInfo
                {
                    PlayerCode = command.PlayerCode,
                    BrandId = command.BrandId,
                    IdentityAuthentication = false,
                    IsSetPayPassword = false,
                    PhoneAuthentication = false,
                    Status = Tgs.PlayerService.Client.PlayerStatus.Enabled,
                    UserName = "ThisIsPlayerName",
                    VipLevelId = null
                });

            fakePlayerService
                .GetPlayerPropertiesAsync(Arg.Is(command.PlayerCode))
                .Returns(new Dictionary<string, string>
                {
                    ["DefaultWalletId"] = walletId.ToString()
                });

            fakeCashServiceClient
                .GetBalanceAsync(Arg.Any<Guid>())
                .Returns(2000M);

            fakeCashServiceClient
                .LockWithdrawAsync(
                    Arg.Is(walletId),
                    Arg.Is(command.Amount),
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .Returns(new TransactionResponse
                {
                    TransactionId = Guid.NewGuid(),
                    Balance = 12345M
                });

            fakePlayerBucketRepository
                .IsBucketFullAsync(Arg.Any<string>())
                .Returns(true);

            var fakeBankInfo = new PlayerBankInfo
            {
                BankType = "Bank",
                BrandId = command.BrandId,
                Id = command.BankInfoId,
                PaymentChannelId = Guid.NewGuid(),
                Detail = JsonDocument.Parse("{}")
            };

            var expected = new WithdrawFormCreateInfo
            {
                Amount = command.Amount,
                BankInfoId = command.BankInfoId,
                BrandId = command.BrandId,
                UserId = command.PlayerCode,
                UserName = "ThisIsPlayerName",
                UserType = UserType.Player,
                Note = command.Note,
            }.ToExpectedObject();

            fakePaymentServiceClient
                .When(fake => fake.CreateWithdrawFormAsync(Arg.Any<WithdrawFormCreateInfo>()))
                .Do(callInfo =>
                {
                    var info = callInfo.Arg<WithdrawFormCreateInfo>();

                    expected.ShouldMatch(info);
                });

            await sut.ExecuteAsync(command);

            var actual = await command.ReturnFormIdSource.Task;

            _ = fakePaymentServiceClient.Received(1)
                .CreateWithdrawFormAsync(Arg.Any<WithdrawFormCreateInfo>());

            _ = fakePaymentServiceClient.Received(1)
                .SubmitWithdrawFormStatusAsync(
                    Arg.Is(actual),
                    Arg.Is(PaymentFlowState.WaitApprove),
                    Arg.Any<string>(),
                    Arg.Any<decimal?>());
        }

        [Test]
        public async Task Player_Can_Get_Withdraw_Required_Amount()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var playerCode = "4ucmqplxb029";

            repository.PlayerBuckets.AddRange(
                new CoreModels.PlayerBucket
                {
                    PlayerCode = playerCode,
                    BucketAmount = 100,
                    CurrentBucketAmount = 100
                },
                new CoreModels.PlayerBucket
                {
                    PlayerCode = playerCode,
                    BucketAmount = 200,
                    CurrentBucketAmount = 100
                });

            await repository.SaveChangesAsync();

            var sut = new GetRequireAmountQueryService(repository);
            var actual = await sut.GetAsync(new GetRequireAmountQuery(playerCode));

            Assert.AreEqual(100, actual);
        }

        [Test]
        public async Task Player_Can_Not_Withdraw_When_Brand_Enable_Player_Identity_Is_True_And_Player_Identity_Authentication_Is_False()
        {
            var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var brandId = Guid.NewGuid();
            var playerCode = "playerCode";

            _ = fakeBrandServiceClient
                .GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
                {
                    Id = brandId,
                    Code = "GPI",
                    Currency = "CNY",
                    PlayerIdentityEnabled = true
                });

            _ = fakePlayerRepository
                .GetPlayerInfoAsync(playerCode)
                .Returns(new Services.Operator.PlayerInfo(
                    playerCode,
                    brandId,
                    0m,
                    IdentityAuthentication: false,
                    false,
                    null,
                    null,
                    null,
                    Core.Models.Enums.PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            var sut = new CheckPlayerIdentityCommandService(fakeBrandServiceClient, fakePlayerRepository);

            var actual = await sut.GetAsync(new CheckPlayerIdentityCommand(brandId, playerCode));

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task Player_Can_Withdraw_When_Brand_Enable_Player_Identity_Is_False()
        {
            var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var brandId = Guid.NewGuid();
            var playerCode = "playerCode";

            _ = fakeBrandServiceClient
                .GetBrandInfoAsync(brandId)
                .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
                {
                    Id = brandId,
                    Code = "GPI",
                    Currency = "CNY"
                });

            var sut = new CheckPlayerIdentityCommandService(fakeBrandServiceClient, fakePlayerRepository);

            var acutal = await sut.GetAsync(new CheckPlayerIdentityCommand(brandId, playerCode));

            _ = fakePlayerRepository
                .Received(0)
                .GetPlayerInfoAsync(playerCode);

            Assert.IsTrue(acutal);
        }

        [Test]
        public async Task Player_Can_Withdraw_When_Brand_Enable_Player_Identity_Is_True_And_Player_Identity_Authentication_Is_True()
        {
            var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var brandId = Guid.NewGuid();
            var playerCode = "playerCode";

            _ = fakeBrandServiceClient
                .GetBrandInfoAsync(brandId)
                .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
                {
                    Id = brandId,
                    Code = "GPI",
                    Currency = "CNY",
                    PlayerIdentityEnabled = true
                });

            _ = fakePlayerRepository
                .GetPlayerInfoAsync(playerCode)
                .Returns(new Services.Operator.PlayerInfo(
                    playerCode,
                    brandId,
                    0m,
                    IdentityAuthentication: true,
                    false,
                    null,
                    null,
                    null,
                    Core.Models.Enums.PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            var sut = new CheckPlayerIdentityCommandService(fakeBrandServiceClient, fakePlayerRepository);

            var actual = await sut.GetAsync(new CheckPlayerIdentityCommand(brandId, playerCode));

            Assert.IsTrue(actual);
        }

        [Test]
        public void Player_Withdraw_But_Balance_Not_Enough_Will_Throw_Excpetion()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerService = Substitute.For<IPlayerServiceClient>();
            var fakePlayerBucketRepository = Substitute.For<IPlayerBucketRepository>();

            var sut = new CreateWithdrawFormCommandService(
                fakePaymentServiceClient,
                fakeCashServiceClient,
                fakePlayerService,
                fakePlayerBucketRepository);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "ThisIsPlayerNote");

            var walletId = Guid.NewGuid();

            fakePlayerService
                .GetPlayerInfoAsync(Arg.Is(command.PlayerCode))
                .Returns(new Tgs.PlayerService.Client.PlayerInfo
                {
                    PlayerCode = command.PlayerCode,
                    BrandId = command.BrandId,
                    IdentityAuthentication = false,
                    IsSetPayPassword = false,
                    PhoneAuthentication = false,
                    Status = Tgs.PlayerService.Client.PlayerStatus.Enabled,
                    UserName = "Tester",
                    VipLevelId = null
                });

            fakePlayerService
                .GetPlayerPropertiesAsync(Arg.Is(command.PlayerCode))
                .Returns(new Dictionary<string, string>
                {
                    ["DefaultWalletId"] = walletId.ToString()
                });

            fakeCashServiceClient
                .GetBalanceAsync(Arg.Any<Guid>())
                .Returns(1000M);

            fakeCashServiceClient
                .LockWithdrawAsync(
                    Arg.Is(walletId),
                    Arg.Any<decimal>(),
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .Returns(new TransactionResponse
                {
                    Balance = -200M,
                    TransactionId = Guid.NewGuid()
                });

            fakePlayerBucketRepository
                .IsBucketFullAsync(Arg.Any<string>())
                .Returns(true);

            var fakeBankInfo = new PlayerBankInfo
            {
                BankType = "Bank",
                BrandId = command.BrandId,
                Id = command.BankInfoId,
                PaymentChannelId = Guid.NewGuid(),
                Detail = JsonDocument.Parse("{}")
            };

            Assert.ThrowsAsync<InsufficientFundsException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Player_Withdraw_Should_Input_Pay_Password()
        {
            var fakePlayerQueries = Substitute.For<IPlayerQueries>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new CheckWithdrawPayPasswordCommandService(
                fakePlayerQueries,
                fakePasswordHandler);

            var command = new CheckWithdrawPayPasswordCommand("ThisIsPlayerCode", "1qaz2wsx");

            fakePasswordHandler
                .Verify(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Is(command.PayPassword))
                .Returns(true);

            await sut.ExecuteAsync(command);

            var actual = await command.ReturnSource.Task.ConfigureTimeout(TimeSpan.FromMilliseconds(500));

            Assert.IsTrue(actual);
        }
    }
}
