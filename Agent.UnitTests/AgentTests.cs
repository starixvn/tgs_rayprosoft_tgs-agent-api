using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.CashService.Client;
using TGS.Core.DataServices.Tgs;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Agent;
using TGS.Services.Agent.Exceptions;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace Agent.UnitTests;

[TestFixture]
public class AgentTests
{
    [Test]
    public async Task Agent_Can_Set_Password()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var sut = new SetAgentPasswordCommandService(
            fakePasswordHandler,
            fakeAgentRepository);

        var command = new SetAgentPasswordCommand(
            Guid.NewGuid(),
            "NewPassword");

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository.Received(1)
            .SetPasswordAsync(
                Arg.Is(command.AgentId),
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Any<DateTimeOffset>());
    }

    [Test]
    public async Task Agent_Can_Valid_Password()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var sut = new ValidateAgentPasswordCommandService(fakeAgentRepository, fakePasswordHandler);

        var command = new ValidateAgentPasswordCommand(
            Guid.NewGuid(),
            "Password");

        fakeAgentRepository.GetAgentByIdAsync(Arg.Any<Guid>())
            .Returns(new AgentInfo(
                command.AgentId,
                "AgentName",
                "AgentName",
                null,
                Guid.NewGuid(),
                null,
                null,
                null,
                "Invitation",
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                null,
                DateTimeOffset.Now,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                0,
                0,
                0));

        await sut.GetAsync(command);

        fakePasswordHandler.Received(1)
            .Verify(
                Arg.Any<byte[]>(),
                Arg.Any<byte[]>(),
                Arg.Is(command.Password));
    }

    [Test]
    public async Task Agent_Validate_Password_If_Not_Found_Agent_Will_Return_False()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var sut = new ValidateAgentPasswordCommandService(fakeAgentRepository, fakePasswordHandler);

        var command = new ValidateAgentPasswordCommand(
            Guid.NewGuid(),
            "Password");

        fakeAgentRepository.GetAgentByIdAsync(Arg.Any<Guid>())
            .Returns(new AgentInfo(
                command.AgentId,
                "AgentName",
                "AgentName",
                null,
                Guid.NewGuid(),
                null,
                null,
                null,
                "Invitation",
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                Array.Empty<byte>(),
                null,
                DateTimeOffset.Now,
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Agent,
                0M,
                0,
                0));

        var actual = await sut.GetAsync(command);

        Assert.That(!actual);
    }

    [Test]
    public async Task Create_Agent_Should_Failed_With_Same_Username()
    {
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();
        var fakeAgentClient = Substitute.For<IAgentClient>();

        fakePasswordHandler.GenerateCredential(Arg.Any<string>())
            .Returns((Array.Empty<byte>(), Array.Empty<byte>()));

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

        dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

        var brandId = Guid.NewGuid();

        var brand = new CoreModels.Brand
        {
            Id = brandId,
            Currencies =
                new List<CoreModels.BrandCurrency>
                {
                    new() { BrandId = brandId, CurrencyCode = "CNY", BrandCurrencyCode = "CNY" }
                },
            ApiKey = string.Empty,
            ClientId = string.Empty,
            Code = "Test",
            CreatedByUsername = "System",
            Name = "Test",
            UpdatedByUsername = "System",
            SecretKey = string.Empty
        };

        _ = await repository.Brands.AddAsync(brand);

        var parentAgentId = Guid.NewGuid();

        var parentAgent = new CoreModels.Agent
        {
            BrandId = brandId,
            Id = parentAgentId,
            AgentDividendConditions =
                new[]
                {
                    new CoreModels.AgentDividendCondition
                    {
                        Rank = 1, NetProfit = 1, ActiveNumber = 1, DividendPercentage = 50
                    }
                },
            InvitationCode = "Test",
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "agent",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        };

        _ = await repository.Agents.AddAsync(parentAgent);

        _ = await repository.SaveChangesAsync();

        var sut = new CreateAgentCommandService(repository, fakeAgentClient);

        var query = new CreateAgentCommand
        {
            ParentAgentId = parentAgentId,
            BrandId = brandId,
            Username = "Agent",
            Password = "Password",
            Remarks = "Remarks"
        };

        _ = Assert.ThrowsAsync<UsernameException>(async () => await sut.GetAsync(query));
    }

    [Test]
    public async Task Create_Agent_Should_Success()
    {
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();
        var fakeAgentClient = Substitute.For<IAgentClient>();

        fakePasswordHandler.GenerateCredential(Arg.Any<string>())
            .Returns((Array.Empty<byte>(), Array.Empty<byte>()));

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

        dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

        var brandId = Guid.NewGuid();

        var brand = new CoreModels.Brand
        {
            Id = brandId,
            Currencies =
                new List<CoreModels.BrandCurrency>
                {
                    new() { BrandId = brandId, CurrencyCode = "CNY", BrandCurrencyCode = "CNY" }
                },
            ApiKey = string.Empty,
            ClientId = string.Empty,
            Code = "Test",
            CreatedByUsername = "System",
            Name = "Test",
            UpdatedByUsername = "System",
            SecretKey = string.Empty
        };

        _ = await repository.Brands.AddAsync(brand);

        var parentAgentId = Guid.NewGuid();

        var parentAgent = new CoreModels.Agent
        {
            Id = parentAgentId,
            AgentDividendConditions =
                new[]
                {
                    new CoreModels.AgentDividendCondition
                    {
                        Rank = 1, NetProfit = 1, ActiveNumber = 1, DividendPercentage = 50
                    }
                },
            InvitationCode = "Test",
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "agent",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        };

        _ = await repository.Agents.AddAsync(parentAgent);

        _ = await repository.SaveChangesAsync();

        var sut = new CreateAgentCommandService(repository, fakeAgentClient);

        var query = new CreateAgentCommand
        {
            ParentAgentId = parentAgentId,
            BrandId = brandId,
            Username = "UserName",
            Password = "Password",
            Remarks = "Remarks"
        };

        var info = await sut.GetAsync(query);

        Assert.That(info.IsNew);

        _ = fakeAgentClient.ReceivedWithAnyArgs(1).CreateAgentAsync(default!);
        _ = fakeAgentClient.Received(1)
            .CreateAgentAsync(Arg.Is<CreateAgentRequest>(x => x.ParentAgentId == parentAgent.Id));
    }

    [Test]
    public async Task Get_Agent_Balance()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

        var sut = new GetAgentBalanceQueryService(fakeCashServiceClient);

        var query = new GetAgentBalanceQuery(Guid.NewGuid(), Guid.NewGuid());

        _ = fakeCashServiceClient.GetBalanceAsync(Arg.Any<Guid>()).Returns(121);

        var response = await sut.GetAsync(query);

        Assert.That(response, Is.EqualTo(121));
    }

    [Test]
    public async Task Get_Agent_Domains_By_Agent_Id()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentDomainsQueryService(fakeAgentRepository);

        var query = new GetAgentDomainsQuery(Guid.NewGuid());

        await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeAgentRepository.Received(1)
            .GetAgentDomainsByIdAsync(Arg.Is(query.AgentId));
    }

    [Test]
    public async Task Get_Agent_Info_By_Agent_Id()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentInfoQueryService(fakeAgentRepository);

        var agentId = Guid.NewGuid();

        await sut.GetAsync(new GetAgentInfoQuery(agentId));

        _ = fakeAgentRepository.Received(1)
            .GetAgentByIdAsync(Arg.Is(agentId));
    }

    [Test]
    public async Task Get_Agent_List_Should_Success()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

        dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

        var brandId = Guid.NewGuid();
        var parentAgentId = Guid.NewGuid();

        repository.Agents.AddRange(
            new CoreModels.Agent
            {
                BrandId = brandId,
                ParentAgentId = parentAgentId,
                Id = Guid.NewGuid(),
                InvitationCode = "Test1",
                Password = Array.Empty<byte>(),
                Salt = Array.Empty<byte>(),
                Username = "agent1",
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                WalletId = Guid.NewGuid()
            },
            new CoreModels.Agent
            {
                BrandId = brandId,
                ParentAgentId = parentAgentId,
                Id = Guid.NewGuid(),
                InvitationCode = "Test2",
                Password = Array.Empty<byte>(),
                Salt = Array.Empty<byte>(),
                Username = "agent2",
                WithdrawPassword = Array.Empty<byte>(),
                WithdrawSalt = Array.Empty<byte>(),
                WalletId = Guid.NewGuid()
            });

        await repository.SaveChangesAsync();

        var sut = new GetAgentInfosQueryService(repository);

        var query = new GetAgentInfosQuery(
            brandId,
            parentAgentId,
            null,
            null);

        var actual = await sut.QueryAsync(query).ToArrayAsync();

        Assert.That(actual.Length, Is.EqualTo(2));
    }
}