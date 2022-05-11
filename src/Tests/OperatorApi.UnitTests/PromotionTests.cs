using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.RouletteServices.Clients;
using TGS.Services.Operator.ApplicationServices.Promotions;

namespace TGS.Tests.OperatorApi.UnitTests;

public class PromotionTests
{
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
}
