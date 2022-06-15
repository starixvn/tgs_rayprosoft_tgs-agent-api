using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NSubstitute;
using NUnit.Framework;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Validators;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Tests.OperatorApi.UnitTests
{
    public class PlayerCreateTests
    {
        [Test]
        public async Task Create_Player_If_Brand_Not_Use_Any_RegisterFlow_Must_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(1)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(1)
                .PlayerLoginIpAsync(
                Arg.Any<string>(),
                Arg.Is(command.IpAddress),
                Arg.Is(command.HostName));
        }

        [Test]
        public async Task Create_Player_If_Brand_Not_Use_Any_RegisterFlow_PlayerStatus_Should_Be_Enable()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.Enabled));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_BankInfoRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
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
                        true)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                 .PlayerLoginIpAsync(
                     Arg.Any<string>(),
                     Arg.Is(command.IpAddress),
                     Arg.Is(command.HostName));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_BankInfoRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(
                    new BrandInfo(
                        brandId,
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
                        true));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_EmailRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        true,
                        false,
                        false,
                        true,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                 .PlayerLoginIpAsync(
                     Arg.Any<string>(),
                     Arg.Is(command.IpAddress),
                     Arg.Is(command.HostName));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_EmailRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        true,
                        false,
                        false,
                        true,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_SmsRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        true,
                        true,
                        false,
                        true,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                 .PlayerLoginIpAsync(
                     Arg.Any<string>(),
                     Arg.Is(command.IpAddress),
                     Arg.Is(command.HostName));
        }

        [Test]
        public async Task Create_Player_If_Brand_Use_SmsRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        true,
                        true,
                        false,
                        false,
                        false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            fakePlayerOperations.GetPlayerByCodeAsync(Arg.Any<string>())
                .Returns(new Player
                {
                    RegisteredOn = DateTimeOffset.UtcNow
                });

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerCommand(
                brandId,
                "username",
                "password",
                "CNY",
                false,
                "192.168.50.123",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Not_Use_Any_RegisterFlow_Must_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(1)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(1)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Not_Use_Any_RegisterFlow_PlayerStatus_Should_Be_Enable()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.Enabled));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_BankInfoRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                true);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_BankInfoRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                true);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_EmailRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                true,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_EmailRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                true,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_SmsRegisterFlow_Not_Record_Ip_Address()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                true,
                false,
                false,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_InvitationCode_If_Brand_Use_SmsRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                true,
                false,
                false,
                false);

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(new Agent()));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Not_Use_Any_RegisterFlow_Must_Record_Ip_Address()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(1)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(1)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Not_Use_Any_RegisterFlow_PlayerStatus_Should_Be_Enable()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                false,
                false,
                false,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.Enabled));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_BankInfoRegisterFlow_Not_Record_Ip_Address()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",

                false,
                false,
                false,
                false,
                true);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_BankInfoRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",

                false,
                false,
                false,
                false,
                true);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_EmailRegisterFlow_Not_Record_Ip_Address()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",

                false,
                false,
                false,
                true,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_EmailRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",

                false,
                false,
                false,
                true,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_SmsRegisterFlow_Not_Record_Ip_Address()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                true,
                false,
                false,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = fakePlayerRepository.Received(0)
                .PlayerRegisterIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName),
                    Arg.Any<DateTimeOffset>());

            _ = fakePlayerRepository.Received(0)
                .PlayerLoginIpAsync(
                    Arg.Any<string>(),
                    Arg.Is(command.IpAddress),
                    Arg.Is(command.HostName));
        }

        [Test]
        public async Task Register_Player_With_Existent_Upper_PlayerCode_If_Brand_Use_SmsRegisterFlow_PlayerStatus_Should_Be_InBlackList()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var brandInfo = new BrandInfo(
                brandId,
                "code",
                new Uri("http://image.com/img01"),
                new Uri("http://image.com/img02"),
                new Uri("http://image.com/img03"),
                Core.Models.Enums.BrandStatus.Active,
                "CNY",
                false,
                true,
                false,
                false,
                false);

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(brandInfo);

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    brandId,
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            fakePlayerOperations.CreatePlayerAsync(Arg.Any<CreatePlayerData>())
                .Returns("TestPlayerCode");

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            var actual = await sut.GetAsync(command);

            // assert
            _ = _ = fakePlayerOperations
                .Received(1)
                .CreatePlayerAsync(
                    Arg.Is<CreatePlayerData>(x => x.PlayerStatus == PlayerStatus.InBlacklist));
        }

        [Test]
        public async Task Register_Player_With_No_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();

            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            var sut = new CreatePlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var password = "password";

            var query = new CreatePlayerCommand(
                brandId,
                "username",
                password,
                "CNY",
                false,
                "::1",
                "localhost");

            // act
            var actual = await sut.GetAsync(query);
            // assert
            Assert.IsTrue(actual.IsNew);
        }

        [Test]
        public void Register_Player_With_Non_Existent_InvitationCode()
        {
            // arrange
            var brandId = Guid.NewGuid();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var password = "password";
            var invitationCode = "NonExistentCode";

            var query = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                password,
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            Assert.ThrowsAsync<InvalidInvitationCodeException>(async () => await sut.GetAsync(query));
        }

        [Test]
        public void Register_Player_With_Non_Existent_Upper_PlayerCode()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var command = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            Assert.ThrowsAsync<InvalidInvitationCodeException>(async () => await sut.GetAsync(command));
        }

        [Test]
        public void Register_Player_With_Other_Brand_InvitationCode_Should_Throw_Error()
        {
            var brandId = Guid.NewGuid();
            var invitationCode = "6rd4mq";

            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeAgentOperations = Substitute.For<IAgentOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeAgentOperations
                .GetAgentByInvitationCodeAsync(
                    Arg.Is(brandId),
                    Arg.Is(invitationCode))
                .Returns(Task.FromResult(default(Agent)));

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        Core.Models.Enums.BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            var sut = new CreateAgentInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakeAgentOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var query = new CreatePlayerWithAgentInvitationCodeCommand(
                brandId,
                "username",
                "password",
                "CNY",
                invitationCode,
                false,
                "::1",
                "localhost");

            // act
            Assert.ThrowsAsync<InvalidInvitationCodeException>(
                async () => await sut.GetAsync(query));
        }

        [Test]
        public void Register_Player_With_Other_Brand_Upper_PlayerCode_Should_Throw_Error()
        {
            var brandId = Guid.NewGuid();
            var upperPlayerCode = "upper";

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakePasswordHandler = Substitute.For<IPasswordHandler>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            fakeBrandRepository.GetBrandInfoAsync(Arg.Is(brandId))
                .Returns(ValueTask.FromResult(
                    new BrandInfo(
                        brandId,
                        "code",
                        new Uri("http://image.com/img01"),
                        new Uri("http://image.com/img02"),
                        new Uri("http://image.com/img03"),
                        BrandStatus.Active,
                        "CNY",
                        false,
                        false,
                        false,
                        false,
                        false)));

            fakePlayerRepository.GetPlayerInfoAsync(Arg.Is(upperPlayerCode))
                .Returns(new PlayerInfo(
                    upperPlayerCode,
                    Guid.NewGuid(),
                    100000M,
                    false,
                    false,
                    null,
                    null,
                    null,
                    PlayerStatus.Enabled,
                    "username",
                    null,
                    null,
                    1,
                    null,
                    null,
                    false));

            var sut = new CreatePlayerInvitationPlayerCommandService(
                fakePlayerOperations,
                fakePasswordHandler,
                fakeBrandOperations,
                fakePlayerRepository,
                fakeBrandRepository);

            var query = new CreatePlayerWithPlayerInvitationCommand(
                brandId,
                "username",
                "password",
                "CNY",
                upperPlayerCode,
                false,
                "::1",
                "localhost");

            // act
            Assert.ThrowsAsync<InvalidInvitationCodeException>(
                async () => await sut.GetAsync(query));
        }

        [Test]
        public void Verify_Player_Account_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();

            var request = new CreatePlayerRequest
            {
                Password = "123456",
                UserName = "123456789012345678901234567890123",
                AgentInvitationCode = string.Empty,
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.UserName);
        }

        [Test]
        public void Verify_Player_Account_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();

            var request = new CreatePlayerRequest
            {
                Password = "123456",
                UserName = "user",
                AgentInvitationCode = string.Empty,
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);

            // assert
            actual.ShouldHaveValidationErrorFor(request => request.UserName);
        }

        [Test]
        public void Verify_Player_Account_With_Special_Character()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();

            var request = new CreatePlayerRequest
            {
                Password = "123456",
                UserName = "abc123_",
                AgentInvitationCode = string.Empty,
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.UserName);
        }

        [Test]
        public void Verify_Player_Password_Input_Max_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();

            var request = new CreatePlayerRequest
            {
                Password = "123456789012345678901",
                UserName = "username",
                AgentInvitationCode = string.Empty,
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }

        [Test]
        public void Verify_Player_Password_Input_Min_Length()
        {
            // arrange
            var sut = new CreatePlayerRequestValidator();

            var request = new CreatePlayerRequest
            {
                Password = "1234",
                UserName = "username",
                AgentInvitationCode = string.Empty,
                IsTestPlayer = false
            };

            // act
            var actual = sut.TestValidate(request);
            // assert
            actual.ShouldHaveValidationErrorFor(request => request.Password);
        }
    }
}
