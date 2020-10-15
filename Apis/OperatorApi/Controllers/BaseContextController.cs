using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.OperatorApi.Controllers
{
    public class BaseContextController : ControllerBase
    {
        private readonly Dictionary<string, string> _userInfoMap;

        public BaseContextController(IHttpContextAccessor httpContextAccessor)
        {
            _userInfoMap = httpContextAccessor.HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }

        protected string Username => _userInfoMap[Consts.UserInfo.UsernameKey];
    }
}
