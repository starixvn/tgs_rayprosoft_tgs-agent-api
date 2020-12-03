using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers
{
    public class BaseContextController : ControllerBase
    {
        private readonly Dictionary<string, string> _userInfoMap;

        public BaseContextController(IHttpContextAccessor httpContextAccessor)
        {
            _userInfoMap = httpContextAccessor.HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }

        protected string Username => _userInfoMap[Consts.UserInfo.UsernameKey];

        protected string RoleName => _userInfoMap[Consts.UserInfo.RolenameKey];

        protected Guid? ResellerId => GetIdByKey(Consts.UserInfo.ResellerKey);

        protected Guid? LicenseeKey => GetIdByKey(Consts.UserInfo.LicenseeKey);

        protected static TResponse CreateValidResponse<TResponse>() where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = true
            };
        }

        protected static TResponse CreateInvalidResponse<TResponse>(string message)
            where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = false,
                Message = message
            };
        }

        private Guid? GetIdByKey(string key)
        {
            return !_userInfoMap.TryGetValue(key, out var value) || value.IsNullOrEmpty() ||
                   !Guid.TryParse(value, out var id)
                ? (Guid?) null
                : id;
        }
    }
}