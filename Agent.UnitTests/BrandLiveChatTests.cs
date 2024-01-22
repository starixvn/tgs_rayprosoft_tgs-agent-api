using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using CoreModels = TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Agent.ApplicationServices.BrandLiveChat;

namespace Agent.UnitTests;

[TestFixture]
public class BrandLiveChatTests
{
    [Test]
    public async Task Get_Brand_Live_Chat_Should_Success()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
        dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
        var brandId = Guid.NewGuid();

        repository.BrandLiveChats.Add(new CoreModels.BrandLiveChat()
        {
            BrandId = brandId,
            ChatName = "LiveChat",
            ChatSetting = "{\"url\":\"https://www.google.com\"}"
        });
        await repository.SaveChangesAsync();

        var sut = new GetBrandLiveChatQueryService(repository);

        var query = new BrandLiveChatQuery(BrandId: brandId);

        var info = await sut.GetAsync(query);

        Assert.That(info!.Setting, Is.Not.Null);
    }
}
