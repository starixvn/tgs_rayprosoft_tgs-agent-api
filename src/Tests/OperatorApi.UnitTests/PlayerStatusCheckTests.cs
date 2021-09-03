using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Filters;
using TGS.Apis.OperatorApi.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class PlayerStatusCheckTests
    {
        [Test]
        public async Task When_Player_Status_Disabled_Should_Get_StatusCodes_403()
        {
            var fakeService = Substitute.For<IGetService<GetPlayerStatusQuery, PlayerStatus>>();
            fakeService
                .GetAsync(new GetPlayerStatusQuery("playerCode"))
                .Returns(PlayerStatus.Disabled);

            var sut = new CheckPlayerEnabledFilter(fakeService);

            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData()
            {
                PlayerCode = "playerCode"
            });
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata> { });
            context.ActionDescriptor.EndpointMetadata = new List<object> {
                new AuthorizePlayerTokenAttribute(),
                new CheckEnabledAttribute()
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNotNull(context.Result);

            var result = (ObjectResult)context.Result;

            Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
            Assert.IsInstanceOf<FailResponse>(result.Value);
            Assert.AreEqual("UserBlocked", ((FailResponse)result.Value).ErrorMessageKey);
        }

        [Test]
        public async Task When_Player_Status_BlackList_Should_Get_StatusCodes_403()
        {
            var fakeService = Substitute.For<IGetService<GetPlayerStatusQuery, PlayerStatus>>();

            fakeService
                .GetAsync(new GetPlayerStatusQuery("playerCode"))
                .Returns(PlayerStatus.InBlacklist);
            var sut = new CheckPlayerEnabledFilter(fakeService);
            
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("playerToken", new PlayerTokenData()
            {
                PlayerCode = "playerCode"
            });
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata> { });
            context.ActionDescriptor.EndpointMetadata = new List<object> {
                new AuthorizePlayerTokenAttribute()
            };

            await sut.OnAuthorizationAsync(context);

            Assert.IsNotNull(context.Result);

            var result = (ObjectResult)context.Result;

            Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
            Assert.IsInstanceOf<FailResponse>(result.Value);
            Assert.AreEqual("AccessDenied", ((FailResponse)result.Value).ErrorMessageKey);
        }
    }
}