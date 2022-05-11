using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TGS.Services.Operator.ApplicationServices.Promotions;

namespace TGS.Tests.OperatorApi.UnitTests;

public class PromotionTests
{
    [Test]
    public async Task Roulette_Should_Get_Result()
    {
        var sut = new SignInRouletteCommandService();

        var command = new SignInRouletteCommand("player1", Guid.NewGuid());

        await sut.GetAsync(command);
    }
}
