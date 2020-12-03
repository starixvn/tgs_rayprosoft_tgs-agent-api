using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application
{
    public abstract class Handler<T, TOut> : IRequestHandler<T, TOut> where T : IRequest<TOut>
    {
        private readonly Dictionary<string, string> _userInfoMap;

        protected Handler(IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext != null)
            {
                _userInfoMap = accessor.HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
            }
        }

        protected string Username => _userInfoMap[Consts.UserInfo.UsernameKey];

        protected string RoleType => _userInfoMap[Consts.UserInfo.RoleTypeKey];
        protected string RoleName => _userInfoMap[Consts.UserInfo.RolenameKey];

        protected Guid? ResellerId => GetIdByKey(Consts.UserInfo.ResellerKey);

        protected Guid? LicenseeId => GetIdByKey(Consts.UserInfo.LicenseeKey);
        protected Guid? BrandId => GetIdByKey(Consts.UserInfo.BrandKey);
        public abstract Task<TOut> Handle(T request, CancellationToken cancellationToken);

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