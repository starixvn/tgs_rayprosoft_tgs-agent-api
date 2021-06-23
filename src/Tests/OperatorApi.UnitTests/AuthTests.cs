using System;
using System.Security.Claims;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator.Services.Auth;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class AuthTests
    {
        [Test]
        public async Task Get_Token_Should_Success()
        {
            //arrange
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeJwtTokenHandler = Substitute.For<IJwtTokenHandler>();

            var apiKey = "I7nKiimrZQOqnJ6ryjUw74c0Hi64KC6Myunhob4cHh8VOUIuZh4p63czefxyyzbZ";
            var claimNameCurrency = "curr";
            var currency = "CNY";
            var site = "operator-api";

            var query = new GetAuthQuery(apiKey, claimNameCurrency, currency, site);

            var sut = new GetAuthQueryService(
                fakeBrandOperations,
                fakeStatusValidator,
                fakeJwtTokenHandler);

            //act
            await sut.GetAsync(query);

            //assert
            _ = fakeJwtTokenHandler
                .Received(1)
                .GenerateToken(Arg.Any<ClaimsIdentity>(), Arg.Any<byte[]>(),
                query.Site, Arg.Any<DateTime>());
        }
    }
}