using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Player;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;

namespace OperatorApi.UnitTests
{
    public class PlayerPasswordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Change_Player_Password_Should_Success()
        {
            var playerCode = "testPlayerCode";
            var fakeHttpContext = new DefaultHttpContext();
            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData()
            {
                PlayerCode = playerCode
            });

            var fakePlayerQueries = Substitute.For<IPlayerQueries>();

            fakePlayerQueries.GetSaltAndPasswordByPlayerCodeAsync(Arg.Any<string>()).Returns((salt: new byte[] { 1, 2, 3 }, password: new byte[] { 4, 5, 6 }));

            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var fakePlayerOperations = Substitute.For<IPlayerOperations>();

            var sut = new ChangePasswordHandler(
                new HttpContextAccessor()
                {
                    HttpContext = fakeHttpContext
                },
                fakePlayerQueries,
                fakePlayerOperations,
                fakePasswordHandler,
                Substitute.For<IStatusValidator>());

            var request = new ChangePasswordRequest()
            {
                OldPassword = "password",
                NewPassword = "newpass"
            };

            fakePasswordHandler
                .Verify(Arg.Any<byte[]>(), Arg.Any<byte[]>(), Arg.Is(request.OldPassword))
                .Returns(true);
            fakePasswordHandler
                .GenerateCredential(Arg.Is(request.NewPassword))
                .Returns((Salt: new byte[] { 1, 2, 3 }, HashedPassword: new byte[] { 4, 5, 6 }));

            var expected = playerCode;

            var actual = await sut.Handle(request, default);

            Assert.AreEqual(expected, actual.PlayerCode);
            _ = fakePlayerOperations.Received(1).ChangePlayerPasswordAsync(
                Arg.Is<ChangePlayerPasswordData>(
                    data => data.PlayerCode == expected
                        && data.Salt != null
                        && data.Password != null));
        }

        [Test]
        public void Forgot_Password_Not_Found_Player_By_Phone_Will_Throw_Exception()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var fakeTgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var sut = new ForgotPasswordCommandService(
                fakeTgsRepository,
                Substitute.For<IValidateCodeGenerator>(),
                Substitute.For<IValidateCodeOperator>());

            var command = new ForgotPasswordCommand(CultureInfo.GetCultureInfo("en-US"), "0911123456");

            Assert.ThrowsAsync<PlayerNotFoundException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Forgot_Password_Will_Sent_ShortMessage()
        {
            var playerPhone = "0911123456";

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var fakeTgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var player = new Player
            {
                PlayerCode = "abcdef123456",
                Username = "Tester",
                BrandId = Guid.NewGuid(),
                Phone = playerPhone,
                PhoneAuthentication = true
            };

            await fakeTgsRepository.Players.AddAsync(player);
            await fakeTgsRepository.SaveChangesAsync();

            var fakeValidateCodeGenerator = Substitute.For<IValidateCodeGenerator>();
            var fakeValidateCodeOperator = Substitute.For<IValidateCodeOperator>();

            var sut = new ForgotPasswordCommandService(
                fakeTgsRepository,
                fakeValidateCodeGenerator,
                fakeValidateCodeOperator);

            var command = new ForgotPasswordCommand(CultureInfo.GetCultureInfo("en-US"), "0911123456");
            var validateCode = "123456";
            var token = "abc123";

            fakeValidateCodeGenerator.GenerateCode().Returns(validateCode);
            fakeValidateCodeGenerator.GenerateToken().Returns(token);

            var expected = $"Your ID verification code is : {validateCode}";

            await sut.ExecuteAsync(command);

            _ = fakeValidateCodeOperator.Received(1).SendMessageAsync(Arg.Is(playerPhone), Arg.Is(expected));
        }
    }
}
