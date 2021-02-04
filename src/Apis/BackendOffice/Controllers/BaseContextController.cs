using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    public class BaseContextController : ControllerBase
    {
        protected string Username => GetUserMap()[Consts.BackendOffice.UserInfo.UsernameKey];
        protected string RoleName => GetUserMap()[Consts.BackendOffice.UserInfo.RolenameKey];

        protected RoleType? UserRoleType =>
            GetUserMap().GetTypeByKey<RoleType>(Consts.BackendOffice.UserInfo.RoleTypeKey);

        protected Guid? ResellerId => GetUserMap().GetIdByKey(Consts.BackendOffice.UserInfo.ResellerKey);

        protected Guid? LicenseeId => GetUserMap().GetIdByKey(Consts.BackendOffice.UserInfo.LicenseeKey);
        protected Guid? BrandId => GetUserMap().GetIdByKey(Consts.BackendOffice.UserInfo.BrandKey);

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