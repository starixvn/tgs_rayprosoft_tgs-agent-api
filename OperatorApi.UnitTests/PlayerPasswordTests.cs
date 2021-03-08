using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Player;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace OperatorApi.UnitTests
{
    public class PlayerPasswordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Change_Player_Password_Should_SuccessAsync()
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
    }
}
