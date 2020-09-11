using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using TGS.Apis.BackendOffice;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.DataService.Enums;

namespace BackendOffice.UnitTests.Controllers
{
    public class BaseTests
    {
        protected readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        public virtual void Setup()
        {
            Login();
        }

        private void Login(
            string username = "admin.enabled",
            string rolename = "Admin",
            RoleType roleType = RoleType.Admin)
        {
            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext.Setup(p => p.User)
                .Returns(GetUserClaimsPrincipal(username, rolename, roleType));

            _httpContextAccessorMock.Setup(p => p.HttpContext)
                .Returns(mockHttpContext.Object);
        }

        protected ClaimsPrincipal GetUserClaimsPrincipal(
            string username = "admin.enabled",
            string rolename = "Admin",
            RoleType roleType = RoleType.Admin)
        {
            return new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new[]
                {
                    new Claim(Consts.UserInfo.UsernameKey, username),
                    new Claim(Consts.UserInfo.RolenameKey, rolename),
                    new Claim(Consts.UserInfo.PortalKey, roleType.ToString())
                }),
            });
        }

        protected (int, int) GetPage(PageableRequest request)
        {
            var page = request.Page <= 0 ? 0 : request.Page - 1;
            var pageSize = request.Rows <= 0 ? 10 : request.Rows;
            return (page * pageSize, pageSize);
        }
    }
}
