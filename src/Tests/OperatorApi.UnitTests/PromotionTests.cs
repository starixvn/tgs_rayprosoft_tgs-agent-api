using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.DepositAccumulateRoulette.Clients;
using Tgs.RouletteServices.Clients;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Promotions;
using RouletteResult = Tgs.DepositAccumulateRoulette.Clients.RouletteResult;

namespace TGS.Tests.OperatorApi.UnitTests;

public class PromotionTests
{
    [Test]
    public async Task Deposit_Accumulate_Roulette_Should_Get_Result()
    {
        var fakeDepositAccumulateRouletteClient = Substitute.For<IDepositAccumulateRouletteClient>();

        var command = new DepositAccumulateRouletteCommand(Guid.NewGuid(), "player1");

        var expected = new RouletteResultWithCount(Guid.NewGuid(), 3, 5);

        _ = fakeDepositAccumulateRouletteClient
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<CancellationToken>())
            .Returns(new RouletteResult(expected.Id, expected.ItemIndex, expected.RouletteCount));

        var sut = new DepositAccumulateRouletteCommandService(fakeDepositAccumulateRouletteClient);

        var actual = await sut.GetAsync(command);

        Assert.AreEqual(expected, actual);

        _ = fakeDepositAccumulateRouletteClient.Received(1)
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Get_Deposit_Accumulate_Roulette_Settings()
    {
        var fakeDepositAccumulateRouletteClient = Substitute.For<IDepositAccumulateRouletteClient>();
        var sut = new DepositAccumulateRouletteSettingQueryService(fakeDepositAccumulateRouletteClient);

        var command = new DepositAccumulateRouletteSettingQuery(Guid.NewGuid());

        await sut.QueryAsync(command).ToArrayAsync();

        _ = fakeDepositAccumulateRouletteClient.Received(1)
            .GetRouletteSettingAsync(
                Arg.Is(command.PromotionId),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Get_Roulette_Record()
    {
        var fakeRouletteSignInClient = Substitute.For<IRouletteSignInClient>();
        var sut = new GetSignInRouletteRecordQueryService(fakeRouletteSignInClient);

        var query = new GetSignInRouletteRecordQuery("player1", Guid.NewGuid());

        await sut.GetAsync(query);

        _ = fakeRouletteSignInClient.Received(1)
            .GetRouletteRecordAsync(
                Arg.Is(query.PromotionId),
                Arg.Is(query.PlayerCode),
                Arg.Any<DateOnly>());
    }

    [Test]
    public async Task Roulette_Should_Get_Result()
    {
        var fakeRouletteSignInClient = Substitute.For<IRouletteSignInClient>();
        var sut = new SignInRouletteCommandService(fakeRouletteSignInClient);

        var command = new SignInRouletteCommand("player1", Guid.NewGuid());

        await sut.GetAsync(command);

        _ = fakeRouletteSignInClient.Received(1)
            .RouletteAsync(
                Arg.Is(command.PromotionId),
                Arg.Is(command.PlayerCode),
                Arg.Any<DateOnly>());
    }
}
