using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers
{
    public class BaseContextController : ControllerBase
    {
        protected string Username => GetUserMap()[Consts.UserInfo.UsernameKey];
        protected string RoleName => GetUserMap()[Consts.UserInfo.RolenameKey];
        protected RoleType? UserRoleType => GetUserMap().GetTypeByKey<RoleType>(Consts.UserInfo.RoleTypeKey);

        protected Guid? ResellerId => GetUserMap().GetIdByKey(Consts.UserInfo.ResellerKey);

        protected Guid? LicenseeId => GetUserMap().GetIdByKey(Consts.UserInfo.LicenseeKey);
        protected Guid? BrandId => GetUserMap().GetIdByKey(Consts.UserInfo.BrandKey);

        protected static TResponse CreateValidResponse<TResponse>() where TResponse : MessageResponse, new() =>
            new()
            {
                Valid = true
            };

        protected static TResponse CreateInvalidResponse<TResponse>(string message)
            where TResponse : MessageResponse, new() =>
            new()
            {
                Valid = false,
                Message = message
            };

        private Dictionary<string, string> GetUserMap()
        {
            return HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }
    }
}