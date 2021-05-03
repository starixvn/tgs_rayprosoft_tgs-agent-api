using System;
using System.Text.Json;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Services.Operator;
using TGS.Services.Operator.Withdraw;
using TGS.Services.Payment;
using TGS.Services.WalletService;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class WithdrawTests
    {
        [Test]
        public async Task Player_Can_Create_Withdraw_Form()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var optionsAccessor = new OptionsWrapper<JsonOptions>(
                new JsonOptions());

            var sut = new CreateWithdrawFormCommandService(
                fakeWalletRepository,
                fakePaymentRepository,
                fakePlayerOperations,
                jsonSerializationProvider,
                optionsAccessor);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            fakePlayerOperations
                .GetCachedPlayerAsync(Arg.Is(command.PlayerCode))
                .Returns(new CachedPlayer
                {
                    PlayerCode = command.PlayerCode,
                    DefaultWalletId = Guid.NewGuid()
                });

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.PlayerCode))
                .Returns(2000M);

            var fakeBankInfo = new BankInfo(
                command.BankInfoId,
                command.PlayerCode,
                command.BrandId,
                Guid.NewGuid(),
                "Bank",
                "detail");

            var expected = new WithdrawFormCreateInfo(
                command.PlayerCode,
                command.BrandId,
                command.Amount,
                1000,
                null,
                command.BankInfoId,
                BankInfo: jsonSerializationProvider.SerializeToString(
                    fakeBankInfo,
                    optionsAccessor.Value.JsonSerializerOptions)).ToExpectedObject();

            fakePaymentRepository
                .GetBankInfoByIdAsync(Arg.Any<Guid>())
                .Returns(fakeBankInfo);

            fakePaymentRepository
                .When(fake => fake.CreateWithdrawFormAsync(Arg.Any<Guid>(), Arg.Any<WithdrawFormCreateInfo>()))
                .Do(callInfo =>
                {
                    var info = callInfo.Arg<WithdrawFormCreateInfo>();

                    expected.ShouldMatch(info);
                });

            await sut.ExecuteAsync(command);

            _ = fakePaymentRepository
                .Received(1)
                .CreateWithdrawFormAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<WithdrawFormCreateInfo>());
        }

        [Test]
        public void Player_Balance_Not_Enough_Withdraw_Will_Throw_Exception()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var optionsAccessor = new OptionsWrapper<JsonOptions>(new JsonOptions());

            var sut = new CreateWithdrawFormCommandService(
                fakeWalletRepository,
                fakePaymentRepository,
                fakePlayerOperations,
                jsonSerializationProvider,
                optionsAccessor);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            fakePlayerOperations
                .GetCachedPlayerAsync(Arg.Is(command.PlayerCode))
                .Returns(new CachedPlayer
                {
                    PlayerCode = command.PlayerCode,
                    DefaultWalletId = Guid.NewGuid()
                });

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.PlayerCode))
                .Returns(800M);

            var fakeBankInfo = new BankInfo(
                command.BankInfoId,
                command.PlayerCode,
                command.BrandId,
                Guid.NewGuid(),
                "Bank",
                "detail");

            Assert.ThrowsAsync<InsufficientFundsException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public void Player_Withdraw_But_Balance_Not_Enough_Will_Throw_Excpetion()
        {
            var fakeWalletRepository = Substitute.For<IWalletRepository>();
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var jsonSerializationProvider = new JsonSerializationProvider();
            var optionsAccessor = new OptionsWrapper<JsonOptions>(new JsonOptions());

            var sut = new CreateWithdrawFormCommandService(
                fakeWalletRepository,
                fakePaymentRepository,
                fakePlayerOperations,
                jsonSerializationProvider,
                optionsAccessor);

            var command = new CreateWithdrawFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M);

            fakePlayerOperations
                .GetCachedPlayerAsync(Arg.Is(command.PlayerCode))
                .Returns(new CachedPlayer
                {
                    PlayerCode = command.PlayerCode,
                    DefaultWalletId = Guid.NewGuid()
                });

            fakeWalletRepository
                .GetBalanceAsync(Arg.Any<Guid>(), Arg.Is(command.PlayerCode))
                .Returns(1000M);

            fakeWalletRepository
                .ExchangeAsync(
                    Arg.Any<Guid>(),
                    Arg.Any<Guid>(),
                    Arg.Is(command.PlayerCode),
                    Arg.Is(TransactionTypes.Withdrawal),
                    Arg.Any<decimal>())
                .Returns((Guid.NewGuid(), -200M));

            var fakeBankInfo = new BankInfo(
                command.BankInfoId,
                command.PlayerCode,
                command.BrandId,
                Guid.NewGuid(),
                "Bank",
                "detail");

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
