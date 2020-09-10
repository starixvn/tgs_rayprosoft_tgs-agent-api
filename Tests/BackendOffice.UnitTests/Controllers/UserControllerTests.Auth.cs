using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.Users.Authentication;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Infrastructures.Security;
using Resources = TGS.Apis.BackendOffice.Resources;

namespace BackendOffice.UnitTests.Controllers
{
    public partial class UserControllerTests
    {
        [Test]
        public async Task Cannot_Authenticate_User_Not_Found()
        {
            var request = new AuthenticateUserRequest
            {
                Username = Generator.RandomString(6),
                Password = Generator.RandomString(6)
            };

            _userQueriesMock.Setup(u => u.GetUserByUsernameAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult<User>(null));

            var response = await _controller.AuthenticateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.User.NotFound);
        }

        [Test]
        public async Task Cannot_Authenticate_User_With_Password_Not_Matched()
        {
            var request = new AuthenticateUserRequest
            {
                Username = Generator.RandomString(6),
                Password = Generator.RandomString(6)
            };

            var user = CreateUserFixture().Create<User>();

            _userQueriesMock.Setup(u => u.GetUserByUsernameAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(user));

            _hashProviderMock.Setup(p => p.Hash(It.IsAny<byte[]>()))
                .Returns(new Fixture().Create<byte[]>());

            _hashProviderMock.Setup(p => p.Hash(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<HashAlgorithmKind>()))
                .Returns(new Fixture().Create<byte[]>());


            var response = await _controller.AuthenticateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Resources.User.InvalidPassword);
        }

        [Test]
        public async Task Can_Authenticate_User()
        {
            var request = new AuthenticateUserRequest
            {
                Username = Generator.RandomString(6),
                Password = Generator.RandomString(6)
            };

            var user = CreateUserFixture().Create<User>();

            _userQueriesMock.Setup(u => u.GetUserByUsernameAsync(It.Is<string>(p => p == request.Username)))
                .Returns(Task.FromResult(user));

            _hashProviderMock.Setup(p => p.Hash(It.IsAny<byte[]>()))
                .Returns(new Fixture().Create<byte[]>());

            _hashProviderMock.Setup(p => p.Hash(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<HashAlgorithmKind>()))
                .Returns(user.Password);

            _antiForgeryMock.Setup(p => p.GetAndStoreTokens(It.IsAny<HttpContext>()))
                .Returns(new Fixture().Create<AntiforgeryTokenSet>());

            SetupHttpContext();

            var response = await _controller.AuthenticateAsync(request);
            response.Valid.Should().BeTrue();
            response.UserInfo.Should().NotBeNull();
            _userCommandsMock.Verify(p => p.UpdateUserAsync(It.Is<User>(u => u.Username == user.Username)));
        }

        private Mock<IAuthenticationService> _authServiceMock;

        [Test]
        public async Task Can_Logout()
        {
            SetupHttpContext();
            await _controller.LogoutAsync();
            _authServiceMock.Verify(p => p.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()));
        }

        private void SetupHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(p => p.User)
                .Returns(GetUserClaimsPrincipal());

            _authServiceMock = new Mock<IAuthenticationService>();
            _authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult(default(object)));

            _authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult(default(object)));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(_authServiceMock.Object);

            mockHttpContext.Setup(p => p.RequestServices)
                .Returns(serviceProviderMock.Object);

            mockHttpContext.Setup(p => p.Connection)
                .Returns(new Mock<ConnectionInfo>().Object);

            var httpResponse = new Mock<HttpResponse>();
            httpResponse.Setup(p => p.Cookies)
                .Returns(new Mock<IResponseCookies>().Object);

            mockHttpContext.Setup(p => p.Response)
                .Returns(httpResponse.Object);

            _httpContextAccessorMock.Setup(p => p.HttpContext)
                .Returns(mockHttpContext.Object);
        }
    }
}
