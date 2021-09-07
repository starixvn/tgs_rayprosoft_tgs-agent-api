using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Filters;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Tokens;
using TGS.Services.Agent.Services.Agent;
using TGS.ServiceUtility;

namespace Agent.UnitTests
{
    [TestFixture]
    public class AgentStatusCheckTests
    {
        [Test]
        public async Task When_Api_Has_No_AuthorizeAgentTokenAttribute_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task
            When_Api_Has_AuthorizeAgentTokenAttribute_And_AllowNullOrEmpty_Is_True_And_NeedAuthorize_Is_True_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = true,
                    NeedAuthorize = true
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task
            When_Api_Has_AuthorizeAgentTokenAttribute_And_AllowNullOrEmpty_Is_True_And_NeedAuthorize_Is_False__Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = true,
                    NeedAuthorize = false
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task
            When_Api_Has_AuthorizeAgentTokenAttribute_And_AllowNullOrEmpty_Is_False_And_NeedAuthorize_Is_True_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();

            fakeService
                .GetAsync(new GetAgentStatusQuery(Guid.NewGuid()))
                .Returns(AgentStatus.Disabled);
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task
            When_Api_Has_AuthorizeAgentTokenAttribute_And_AllowNullOrEmpty_Is_False_And_NeedAuthorize_Is_False_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    NeedAuthorize = false,
                    AllowNullOrEmpty = false
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task
            When_Api_Has_AuthorizeAgentTokenAttribute_And_NeedAuthorize_Is_True_And_AllowNullOrEmpty_Is_False_And_Agent_Status_Blacklist_Should_Get_StatusCodes_403()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var agentId = Guid.NewGuid();

            fakeService
                .GetAsync(new GetAgentStatusQuery(agentId))
                .Returns(AgentStatus.InBlacklist);
            var sut = new CheckAgentEnabledFilter(fakeService);
            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = agentId});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNotNull(context.Result);

            var result = (ObjectResult)context.Result;

            Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
            Assert.IsInstanceOf<TgsResponse>(result.Value);
            Assert.AreEqual("AccessDenied", ((TgsResponse)result.Value).ErrorMessageKey);
        }

        [Test]
        public async Task When_Agent_Status_Enabled_Api_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            fakeService
                .GetAsync(new GetAgentStatusQuery(Guid.NewGuid()))
                .Returns(AgentStatus.Enabled);

            var sut = new CheckAgentEnabledFilter(fakeService);

            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                }
            };

            await sut.OnAuthorizationAsync(context);
            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task When_Agent_Status_Disabled_And_Api_Has_CheckEnabledAttribute_Should_Get_StatusCodes_403()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var agentId = Guid.NewGuid();

            fakeService
                .GetAsync(new GetAgentStatusQuery(agentId))
                .Returns(AgentStatus.Disabled);

            var sut = new CheckAgentEnabledFilter(fakeService);

            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = agentId});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                },
                new CheckEnabledAttribute()
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNotNull(context.Result);

            var result = (ObjectResult)context.Result;

            Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
            Assert.IsInstanceOf<TgsResponse>(result.Value);
            Assert.AreEqual("UserBlocked", ((TgsResponse)result.Value).ErrorMessageKey);
        }

        [Test]
        public async Task When_Agent_Status_Disabled_And_Api_Has_No_CheckEnabledAttribute_Should_Execute()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            fakeService
                .GetAsync(new GetAgentStatusQuery(Guid.NewGuid()))
                .Returns(AgentStatus.Disabled);

            var sut = new CheckAgentEnabledFilter(fakeService);

            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = Guid.NewGuid()});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                }
            };
            await sut.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [Test]
        public async Task When_Agent_Status_BlackList_Should_Get_StatusCodes_403()
        {
            var fakeService = Substitute.For<IGetService<GetAgentStatusQuery, AgentStatus>>();
            var agentId = Guid.NewGuid();

            fakeService
                .GetAsync(new GetAgentStatusQuery(agentId))
                .Returns(AgentStatus.InBlacklist);
            var sut = new CheckAgentEnabledFilter(fakeService);

            var fakeHttpContext = new DefaultHttpContext();

            fakeHttpContext.Items.Add("agentToken", new AgentTokenData {AgentId = agentId});
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(fakeHttpContext, routeData, actionDescriptor);
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            context.ActionDescriptor.EndpointMetadata = new List<object>
            {
                new AuthorizeAgentTokenAttribute
                {
                    AllowNullOrEmpty = false,
                    NeedAuthorize = true
                }
            };

            await sut.OnAuthorizationAsync(context);

            Assert.IsNotNull(context.Result);

            var result = (ObjectResult)context.Result;

            Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);
            Assert.IsInstanceOf<TgsResponse>(result.Value);
            Assert.AreEqual("AccessDenied", ((TgsResponse)result.Value).ErrorMessageKey);
        }
    }
}