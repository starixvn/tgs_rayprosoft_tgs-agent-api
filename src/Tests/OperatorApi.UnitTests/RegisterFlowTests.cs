using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandCertificate;
using Tgs.BrandCertificate.Clients;
using Tgs.Payment.Clients;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Brand;
using TGS.Services.Operator.ApplicationServices.Player;
using TGS.Services.Operator.ApplicationServices.Withdraw;
using TGS.Services.Operator.PlayerRegisterFlow;
using static TGS.Services.Operator.PlayerRegisterFlow.SetEmailRegisterFlowStep;
using static TGS.Services.Operator.PlayerRegisterFlow.SetPhoneRegisterFlowStep;
using static TGS.Services.Operator.PlayerRegisterFlow.SetWithdrawBankInfoRegisterFlowStep;

namespace TGS.Tests.OperatorApi.UnitTests;

public class RegisterFlowTests
{
    [Test]
    public void Add_Step_To_RegisterFlowDefinition()
    {
        var sut = new RegisterFlowDefinition();

        var privateList = typeof(RegisterFlowDefinition)
            .GetField("_stepDefinitions", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(sut) as List<IRegisterFlowStep>;

        Assert.AreEqual(0, privateList.Count);

        sut.AddStep(new StubRegisterFlowStep());

        Assert.AreEqual(1, privateList.Count);
    }

    [Test]
    public async Task After_Call_ConfirmCompleted_Will_Remain_Undone_Steps()
    {
        var playerCode = "player1";

        var step1 = new StubRegisterFlowStep();
        step1.SetConfirmCompletedDelegate(_ => true);
        var step2 = new StubRegisterFlowStep();
        var step3 = new StubRegisterFlowStep();

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        var privateStack = typeof(RegisterFlow)
            .GetField("_flowSteps", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(sut) as Queue<IRegisterFlowStep>;

        Assert.IsFalse(actual);
        Assert.AreEqual(2, privateStack.Count);
    }

    [Test]
    public void Build_RegisterFlow()
    {
        var sut = new RegisterFlowDefinition();

        sut.AddStep(new StubRegisterFlowStep());

        var actual = sut.BuildRegisterFlow();

        var privateStack = typeof(RegisterFlow)
            .GetField("_flowSteps", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(actual) as Queue<IRegisterFlowStep>;

        Assert.AreEqual(1, privateStack.Count);
    }

    [Test]
    public void Create_RegisterFlowDefinition_From_RegisterFlowBuilder()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>())
            .AddSingleton(Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>())
            .AddSingleton(Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>())
            .AddSingleton(Substitute.For<IPlayerServiceClient>())
            .AddSingleton(Substitute.For<IBrandServiceClient>())
            .BuildServiceProvider();
        var sut = new RegisterFlowBuilder(
            serviceProvider,
            new Dictionary<string, Type>
            {
                ["Test1"] = typeof(StubRegisterFlowStep)
            }.ToImmutableDictionary());

        var actual = sut.LoadSettings(JsonDocument.Parse(@"[
            {
                ""type"": ""Test1""
            }]"));

        var privateList = typeof(RegisterFlowDefinition)
            .GetField("_stepDefinitions", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(actual) as List<IRegisterFlowStep>;

        Assert.IsNotNull(privateList);
        Assert.AreEqual(2, privateList.Count);
    }

    [Test]
    public async Task CreatePlayerRegisterStep_If_Player_Exists_ConfirmCompleted_Should_Return_True()
    {
        var fakeCreatePlayerService = Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>();
        var fakeCreateAgentInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>();
        var fakeCreatePlayerInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();

        var sut = new CreatePlayerRegisterFlowStep(
            fakeCreatePlayerService,
            fakeCreateAgentInvitationPlayerService,
            fakeCreatePlayerInvitationPlayerService,
            fakePlayerServiceClient,
            fakeBrandServiceClient);

        var playerCode = "player1";

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(playerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo());

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task CreatePlayerRegisterStep_ProcessInput_With_InvitationCode()
    {
        var fakeCreatePlayerService = Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>();
        var fakeCreateAgentInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>();
        var fakeCreatePlayerInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();

        var sut = new CreatePlayerRegisterFlowStep(
            fakeCreatePlayerService,
            fakeCreateAgentInvitationPlayerService,
            fakeCreatePlayerInvitationPlayerService,
            fakePlayerServiceClient,
            fakeBrandServiceClient);

        var brandId = Guid.NewGuid();

        var input = new CreatePlayerRegisterFlowStep.CreatePlayerInputData
        {
            IsTestPlayer = false,
            UserName = "abc",
            Password = "1234",
            AgentInvitationCode = "aabbcc"
        };

        var locationInfo = new PlayerLocationInfo(
            null,
            "127.0.0.1",
            null,
            null);

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandId))
            .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
            {
                Id = brandId,
                Code = "AAA",
                Currency = "CNY"
            });

        fakeCreateAgentInvitationPlayerService.GetAsync(Arg.Is<CreatePlayerWithAgentInvitationCodeCommand>(
            x => x.BrandId == brandId
                && x.IsTestPlayer == input.IsTestPlayer
                && x.IpAddress == locationInfo.Ip
                && x.HostName == locationInfo.HostName
                && x.UserName == input.UserName
                && x.Password == input.Password
                && x.AgentInvitationCode == input.AgentInvitationCode))
            .Returns(new CreatePlayerInfo("test", true));

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(input),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task SetWithdrawBankInfoRegisterStep_ProcssInput()
    {
        var fakeQueryPlayerBankInfoService = Substitute.For<IQueryService<GetPlayerBankInfosQuery, PlayerBankInfo>>();
        var fakeCreateBankInfoService = Substitute.For<ICommandService<CreateInitBankInfoCommand>>();

        var sut = new SetWithdrawBankInfoRegisterFlowStep(
            fakeQueryPlayerBankInfoService,
            fakeCreateBankInfoService);

        var brandId = Guid.NewGuid();
        var inputData = new BankInfoData
        {
            BankType = "Bank",
            Detail = JsonDocument.Parse("{}"),
            PaymentChannelId = Guid.NewGuid()
        };

        var locationInfo = new PlayerLocationInfo(
            "player1",
            "127.0.0.1",
            null,
            null);

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(inputData),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task CreatePlayerRegisterStep_ProcessInput_With_InvitationCode_Header()
    {
        var fakeCreatePlayerService = Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>();
        var fakeCreateAgentInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>();
        var fakeCreatePlayerInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();

        var sut = new CreatePlayerRegisterFlowStep(
            fakeCreatePlayerService,
            fakeCreateAgentInvitationPlayerService,
            fakeCreatePlayerInvitationPlayerService,
            fakePlayerServiceClient,
            fakeBrandServiceClient);

        var brandId = Guid.NewGuid();

        var input = new CreatePlayerRegisterFlowStep.CreatePlayerInputData
        {
            IsTestPlayer = false,
            UserName = "abc",
            Password = "1234"
        };

        var locationInfo = new PlayerLocationInfo(
            null,
            "127.0.0.1",
            null,
            "aabbcc");

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandId))
            .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
            {
                Id = brandId,
                Code = "AAA",
                Currency = "CNY"
            });

        fakeCreateAgentInvitationPlayerService.GetAsync(Arg.Is<CreatePlayerWithAgentInvitationCodeCommand>(
            x => x.BrandId == brandId
                && x.IsTestPlayer == input.IsTestPlayer
                && x.IpAddress == locationInfo.Ip
                && x.HostName == locationInfo.HostName
                && x.UserName == input.UserName
                && x.Password == input.Password
                && x.AgentInvitationCode == locationInfo.AgentInvitationCode))
            .Returns(new CreatePlayerInfo("test", true));

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(input),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task CreatePlayerRegisterStep_ProcessInput_With_UpperPlayer()
    {
        var fakeCreatePlayerService = Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>();
        var fakeCreateAgentInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>();
        var fakeCreatePlayerInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();

        var sut = new CreatePlayerRegisterFlowStep(
            fakeCreatePlayerService,
            fakeCreateAgentInvitationPlayerService,
            fakeCreatePlayerInvitationPlayerService,
            fakePlayerServiceClient,
            fakeBrandServiceClient);

        var brandId = Guid.NewGuid();

        var input = new CreatePlayerRegisterFlowStep.CreatePlayerInputData
        {
            IsTestPlayer = false,
            UserName = "abc",
            Password = "1234",
            UpperPlayerCode = "player2"
        };

        var locationInfo = new PlayerLocationInfo(
            null,
            "127.0.0.1",
            null,
            null);

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandId))
            .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
            {
                Id = brandId,
                Code = "AAA",
                Currency = "CNY"
            });

        fakeCreatePlayerInvitationPlayerService.GetAsync(Arg.Is<CreatePlayerWithPlayerInvitationCommand>(
            x => x.BrandId == brandId
                && x.IsTestPlayer == input.IsTestPlayer
                && x.IpAddress == locationInfo.Ip
                && x.HostName == locationInfo.HostName
                && x.UserName == input.UserName
                && x.Password == input.Password
                && x.UpperPlayerCode == input.UpperPlayerCode))
            .Returns(new CreatePlayerInfo("test", true));

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(input),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task CreatePlayerRegisterStep_ProcessInput_Without_InvitationCode()
    {
        var fakeCreatePlayerService = Substitute.For<IGetService<CreatePlayerCommand, CreatePlayerInfo>>();
        var fakeCreateAgentInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithAgentInvitationCodeCommand, CreatePlayerInfo>>();
        var fakeCreatePlayerInvitationPlayerService = Substitute.For<IGetService<CreatePlayerWithPlayerInvitationCommand, CreatePlayerInfo>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();

        var sut = new CreatePlayerRegisterFlowStep(
            fakeCreatePlayerService,
            fakeCreateAgentInvitationPlayerService,
            fakeCreatePlayerInvitationPlayerService,
            fakePlayerServiceClient,
            fakeBrandServiceClient);

        var brandId = Guid.NewGuid();

        var input = new CreatePlayerRegisterFlowStep.CreatePlayerInputData
        {
            IsTestPlayer = false,
            UserName = "abc",
            Password = "1234"
        };

        var locationInfo = new PlayerLocationInfo(
            null,
            "127.0.0.1",
            null,
            null);

        fakeBrandServiceClient.GetBrandInfoAsync(Arg.Is(brandId))
            .Returns(new Tgs.BrandCertificate.Clients.BrandInfo
            {
                Id = brandId,
                Code = "AAA",
                Currency = "CNY"
            });

        fakeCreatePlayerService.GetAsync(Arg.Is<CreatePlayerCommand>(
            x => x.BrandId == brandId
                && x.IsTestPlayer == input.IsTestPlayer
                && x.IpAddress == locationInfo.Ip
                && x.HostName == locationInfo.HostName
                && x.UserName == input.UserName
                && x.Password == input.Password))
            .Returns(new CreatePlayerInfo("test", true));

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(input),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task ExecutePlayerRegisterFlow()
    {
        var fakeBrandRegisterFlowDefinitionManager = Substitute.For<IBrandRegisterFlowDefinitionManager>();

        var sut = new ExecutePlayerRegisterFlowCommandService(fakeBrandRegisterFlowDefinitionManager);

        var command = new ExecutePlayerRegisterFlowCommand(
            Guid.NewGuid(),
            "CreatePlayer",
            JsonDocument.Parse("{}"),
            new PlayerLocationInfo(null, "127.0.0.1", null, null));

        var flowDefinition = new RegisterFlowDefinition();

        var fakeCreatePlayerStep = new StubRegisterFlowStep()
        {
            Type = "CreatePlayer"
        };

        fakeCreatePlayerStep.SetProcessInputDelegate((_, _, _) => Task.FromResult(
            new RegisterFlowProcessResult(true, null, null)));

        flowDefinition.AddStep(fakeCreatePlayerStep);

        fakeBrandRegisterFlowDefinitionManager.GetRegisterFlowDefinitionAsync(Arg.Is(command.BrandId))
            .Returns(flowDefinition);

        var actual = await sut.GetAsync(command);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task If_Has_Any_Undone_Step_ConfirmCompleted_Should_Return_True()
    {
        var sut = new RegisterFlow(Array.Empty<IRegisterFlowStep>());

        var actual = await sut.ConfirmCompletedAsync(null);

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Process_Input_If_Step_Undone_Will_Return_CurrentStepInfo()
    {
        var brandId = Guid.NewGuid();
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        var step1 = new StubRegisterFlowStep()
        {
            Type = "Step1"
        };
        var stepResponse = JsonDocument.Parse("{}");
        step1.SetProcessInputDelegate((_, _, _) => Task.FromResult(
            new RegisterFlowProcessResult(false, null, stepResponse)));
        var step2 = new StubRegisterFlowStep()
        {
            Type = "Step2"
        };
        var step3 = new StubRegisterFlowStep()
        {
            Type = "Step3"
        };

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ProcessInputAsync(brandId, JsonDocument.Parse("{}"), locationInfo);

        Assert.IsFalse(actual.IsCompleted);
        Assert.AreEqual("Step1", actual.NextStepType);
        Assert.AreEqual(stepResponse, actual.StepResponse);
    }

    [Test]
    public async Task Process_Input_Will_Return_NextStepInfo()
    {
        var brandId = Guid.NewGuid();
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        var step1 = new StubRegisterFlowStep()
        {
            Type = "Step1"
        };
        step1.SetProcessInputDelegate((_, _, _) => Task.FromResult(
            new RegisterFlowProcessResult(true, null, null)));
        var step2 = new StubRegisterFlowStep()
        {
            Type = "Step2"
        };
        var step3 = new StubRegisterFlowStep()
        {
            Type = "Step3"
        };

        var sut = new RegisterFlow(new[] {
            step1,
            step2,
            step3
        });

        var actual = await sut.ProcessInputAsync(brandId, JsonDocument.Parse("{}"), locationInfo);

        Assert.IsFalse(actual.IsCompleted);
        Assert.AreEqual("Step2", actual.NextStepType);
    }

    [Test]
    public async Task SetEmailRegisterFlowStep_ConfirmCompleted_With_Player_Already_Authenticate_Email_Should_Return_True()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
        var fakeStartSetMailService = Substitute.For<IGetService<StartSetPlayerEmailCommand, string>>();
        var fakeCommitMailService = Substitute.For<ICommandService<CommitInitPlayerEmailCommand>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        var sut = new SetEmailRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandEmailService,
            fakeStartSetMailService,
            fakeCommitMailService,
            fakePlayerServiceClient);

        var playerCode = "player1";

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(playerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = "tester",
                EmailAuthentication = true
            });

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task SetEmailRegisterFlowStep_ProcessInitEmail()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
        var fakeStartSetMailService = Substitute.For<IGetService<StartSetPlayerEmailCommand, string>>();
        var fakeCommitMailService = Substitute.For<ICommandService<CommitInitPlayerEmailCommand>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        var sut = new SetEmailRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandEmailService,
            fakeStartSetMailService,
            fakeCommitMailService,
            fakePlayerServiceClient);

        var brandId = Guid.NewGuid();
        var emailData = new EmailData
        {
            Email = "tester@test.com"
        };
        var inputData = new InitEmailStepInputData
        {
            InputType = "Init",
            Data = JsonSerializer.SerializeToDocument(emailData)
        };
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        fakeGetBrandInfoService.GetAsync(Arg.Is<BrandQuery>(query => query.BrandId == brandId))
            .Returns(new Services.Operator.BrandInfo(
                brandId,
                "Test",
                null,
                null,
                null,
                BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                true,
                false,
                null));

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(locationInfo.PlayerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = locationInfo.PlayerCode
            });

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(inputData),
            locationInfo);

        Assert.IsFalse(actual.IsCompleted);
        Assert.NotNull(actual.StepResponse);
    }

    [Test]
    public async Task SetEmailRegisterFlowStep_ProcessValidEmail()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandEmailService = Substitute.For<IGetService<CheckBrandEmailQuery, bool>>();
        var fakeStartSetMailService = Substitute.For<IGetService<StartSetPlayerEmailCommand, string>>();
        var fakeCommitMailService = Substitute.For<ICommandService<CommitInitPlayerEmailCommand>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

        var sut = new SetEmailRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandEmailService,
            fakeStartSetMailService,
            fakeCommitMailService,
            fakePlayerServiceClient);

        var brandId = Guid.NewGuid();
        var confirmData = new ConfirmEmailData
        {
            SecurityKey = Guid.NewGuid().ToString(),
            ValidateCode = "123456"
        };
        var inputData = new InitEmailStepInputData
        {
            InputType = "Valid",
            Data = JsonSerializer.SerializeToDocument(confirmData)
        };
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(locationInfo.PlayerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = locationInfo.PlayerCode
            });

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(inputData),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task SetPhoneRegisterFlowStep_ConfirmCompleted_With_Player_Already_Authenticate_Phone_Should_Return_True()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandPhoneService = Substitute.For<IGetService<CheckBrandPhoneQuery, bool>>();
        var fakeStartSetPhoneService = Substitute.For<IGetService<StartSetPlayerPhoneCommand, string>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeConfirmSetPhoneService = Substitute.For<ICommandService<CommitInitPlayerPhoneCommand>>();

        var sut = new SetPhoneRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandPhoneService,
            fakeStartSetPhoneService,
            fakeConfirmSetPhoneService,
            fakePlayerServiceClient);

        var playerCode = "player1";

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(playerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = "tester",
                PhoneAuthentication = true
            });

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task SetPhoneRegisterFlowStep_ProcessInitPhone()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandPhoneService = Substitute.For<IGetService<CheckBrandPhoneQuery, bool>>();
        var fakeStartSetPhoneService = Substitute.For<IGetService<StartSetPlayerPhoneCommand, string>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeConfirmSetPhoneService = Substitute.For<ICommandService<CommitInitPlayerPhoneCommand>>();

        var sut = new SetPhoneRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandPhoneService,
            fakeStartSetPhoneService,
            fakeConfirmSetPhoneService,
            fakePlayerServiceClient);

        var brandId = Guid.NewGuid();
        var phoneData = new PhoneData
        {
            AreaCode = "123",
            Phone = "1345678"
        };
        var inputData = new InitPhoneStepInputData
        {
            InputType = "Init",
            Data = JsonSerializer.SerializeToDocument(phoneData)
        };
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        fakeGetBrandInfoService.GetAsync(Arg.Is<BrandQuery>(query => query.BrandId == brandId))
            .Returns(new Services.Operator.BrandInfo(
                brandId,
                "Test",
                null,
                null,
                null,
                BrandStatus.Active,
                "CNY",
                false,
                true,
                false,
                false,
                false,
                null));

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(locationInfo.PlayerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = locationInfo.PlayerCode
            });

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(inputData),
            locationInfo);

        Assert.IsFalse(actual.IsCompleted);
        Assert.NotNull(actual.StepResponse);
    }

    [Test]
    public async Task SetPhoneRegisterFlowStep_ProcessValidPhone()
    {
        var fakeGetBrandInfoService = Substitute.For<IGetService<BrandQuery, Services.Operator.BrandInfo>>();
        var fakeCheckBrandPhoneService = Substitute.For<IGetService<CheckBrandPhoneQuery, bool>>();
        var fakeStartSetPhoneService = Substitute.For<IGetService<StartSetPlayerPhoneCommand, string>>();
        var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();
        var fakeConfirmSetPhoneService = Substitute.For<ICommandService<CommitInitPlayerPhoneCommand>>();

        var sut = new SetPhoneRegisterFlowStep(
            fakeGetBrandInfoService,
            fakeCheckBrandPhoneService,
            fakeStartSetPhoneService,
            fakeConfirmSetPhoneService,
            fakePlayerServiceClient);

        var brandId = Guid.NewGuid();
        var confirmData = new ConfirmPhoneData
        {
            SecurityKey = Guid.NewGuid().ToString(),
            ValidateCode = "123456"
        };
        var inputData = new InitPhoneStepInputData
        {
            InputType = "Valid",
            Data = JsonSerializer.SerializeToDocument(confirmData)
        };
        var locationInfo = new PlayerLocationInfo("player1", "127.0.0.1", null, null);

        fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Is(locationInfo.PlayerCode))
            .Returns(new Tgs.PlayerService.Client.PlayerInfo
            {
                PlayerCode = locationInfo.PlayerCode
            });

        var actual = await sut.ProcessInputAsync(
            brandId,
            JsonSerializer.SerializeToDocument(inputData),
            locationInfo);

        Assert.IsTrue(actual.IsCompleted);
    }

    [Test]
    public async Task SetWithdrawBankInfoRegisterStep_If_Has_Withdraw_BankInfo_ConfirmCompleted_Should_Return_True()
    {
        var fakeQueryPlayerBankInfoService = Substitute.For<IQueryService<GetPlayerBankInfosQuery, PlayerBankInfo>>();
        var fakeCreateBankInfoService = Substitute.For<ICommandService<CreateInitBankInfoCommand>>();

        var sut = new SetWithdrawBankInfoRegisterFlowStep(
            fakeQueryPlayerBankInfoService,
            fakeCreateBankInfoService);

        var playerCode = "player1";
        var brandId = Guid.NewGuid();

        fakeQueryPlayerBankInfoService.QueryAsync(Arg.Any<GetPlayerBankInfosQuery>())
            .Returns(new[] {
                new PlayerBankInfo
                {
                    BankType = "Bank",
                    BrandId = brandId,
                    Id = Guid.NewGuid(),
                    PaymentChannelId = Guid.NewGuid(),
                    PlayerCode = playerCode,
                    IsDefault=true
                }
            }.ToAsyncEnumerable());

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        Assert.IsTrue(actual);
    }

    [Test]
    public async Task Undone_Flow_Call_ConfirmCompleted_Should_Return_False()
    {
        var playerCode = "player1";

        var sut = new RegisterFlow(new[] {
            new StubRegisterFlowStep()
        });

        var actual = await sut.ConfirmCompletedAsync(playerCode);

        Assert.IsFalse(actual);
    }
}
