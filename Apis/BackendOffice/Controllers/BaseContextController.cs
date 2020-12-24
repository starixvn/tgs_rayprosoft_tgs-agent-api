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
        protected RoleType? UserRoleType => GetTypeByKey<RoleType>(Consts.UserInfo.RoleTypeKey);

        protected Guid? ResellerId => GetIdByKey(Consts.UserInfo.ResellerKey);

        protected Guid? LicenseeId => GetIdByKey(Consts.UserInfo.LicenseeKey);
        protected Guid? BrandId => GetIdByKey(Consts.UserInfo.BrandKey);

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

        private Guid? GetIdByKey(string key) =>
            !GetUserMap().TryGetValue(key, out var value) || value.IsNullOrEmpty() ||
            !Guid.TryParse(value, out var id)
                ? (Guid?) null
                : id;

        private TEnum? GetTypeByKey<TEnum>(string key) where TEnum : struct =>
            !GetUserMap().TryGetValue(key, out var value) || value.IsNullOrEmpty() ||
            !Enum.TryParse(value, true, out TEnum resultInputType)
                ? (TEnum?) null
                : resultInputType;

        private Dictionary<string, string> GetUserMap()
        {
            return HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }
    }
}