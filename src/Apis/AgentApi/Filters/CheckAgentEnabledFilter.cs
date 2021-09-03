using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;

namespace TGS.Apis.AgentApi.Filters
{
    public class CheckAgentEnabledFilter : IAsyncAuthorizationFilter
    {
        public CheckAgentEnabledFilter()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authAttr = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAgentTokenAttribute>().LastOrDefault();

            if (authAttr == null || !authAttr.NeedAuthorize && authAttr.AllowNullOrEmpty)
            {
                return;
            }
            var ignoreAttr = context.ActionDescriptor.EndpointMetadata.OfType<CheckEnabledAttribute>().LastOrDefault();

            var agentTokenData = context.HttpContext.Items.GetAgentTokenData();

            var agentId = agentTokenData.AgentId;

            //TODO:補查agent status
            // await 
            
            //TODO:補封鎖和黑單時需要的回傳內容

            context.Result = new ObjectResult(
               new object())//TODO:補回傳格式
            {
                StatusCode = 403
            };
        }
    }
}
