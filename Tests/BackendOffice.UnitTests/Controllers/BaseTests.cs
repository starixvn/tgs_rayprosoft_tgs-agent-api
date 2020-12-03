using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Domain;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [Category(Named.Unit)]
    public class BaseTests
    {
        protected readonly Mock<IHttpContextAccessor> HttpContextAccessorMock = new Mock<IHttpContextAccessor>();
        protected string Username = "admin.enabled";

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

            HttpContextAccessorMock.Setup(p => p.HttpContext)
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
                })
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