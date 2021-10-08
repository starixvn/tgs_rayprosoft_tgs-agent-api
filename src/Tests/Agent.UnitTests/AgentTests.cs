using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Agent;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace Agent.UnitTests
{
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
                    null,
                    1,
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    0M,
                    0M));

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
                    null,
                    1,
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    Array.Empty<byte>(),
                    0M,
                    0M));

            var actual = await sut.GetAsync(command);

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task Create_Agent_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();

            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.NewGuid();

            var brand = new CoreModels.Brand
            {
                Id = brandId,
                Currencies = new List<CoreModels.BrandCurrency>
                {
                    new CoreModels.BrandCurrency
                    {
                        BrandId = brandId,
                        CurrencyCode = "CNY"
                    }
                }
            };

            await repository.Brands.AddAsync(brand);
            var parentAgentId = Guid.NewGuid();

            var parentAgent = new CoreModels.Agent
            {
                Id = parentAgentId,
                AgentDividendConditions = new[]
                {
                    new CoreModels.AgentDividendCondition
                    {
                        Rank = 1,
                        NetProfit = 1,
                        ActiveNumber = 1,
                        DividendPercentage = 50
                    }
                }
            };
            await repository.Agents.AddAsync(parentAgent);
            await repository.SaveChangesAsync();

            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new GetCreateAgentQueryService(repository, fakePasswordHandler);

            var query = new GetCreateAgentQuery
            {
                ParentAgentId = parentAgentId,
                BrandId = brandId,
                Username = "UserName",
                Password = "Password",
                Remarks = "Remarks",
                AgentDividendConditions = new[]
                {
                    new AgentDividendConditionModel
                    {
                        Rank = 1,
                        NetProfit = 1,
                        ActiveNumber = 1,
                        DividendPercentage = 50
                    }
                }
            };

            var info = await sut.GetAsync(query);

            Assert.IsTrue(info.IsNew);
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
    }
}