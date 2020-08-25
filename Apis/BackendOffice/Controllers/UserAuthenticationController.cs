using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TGS.Apis.BackendOffice.Controllers.Messages.UserAuthentication;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.TGS.Apis.BackendOffice.Extensions;
using TGS.Infrastructures.Security;

namespace TGS.Apis.TGS.Apis.BackendOffice.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthenticationController : BackendOfficeControllerBase
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _tgsHashConfig;
        private readonly IUserQueries _userQueries;
        protected UserAuthenticationController(
            IHashProvider hashProvider,
            ITgsHashConfig tgsHashConfig,
            IUserQueries userQueries)
        {
            _hashProvider = hashProvider;
            _tgsHashConfig = tgsHashConfig;
            _userQueries = userQueries;
        }

        [AllowAnonymous]
        [Route("auth")]
        [HttpPost, Description("Authenticate user login")]
        public async Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest request)
        {
            var user = await _userQueries.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                return CreateInvalidResponse<AuthenticateUserResponse>("User not found.");
            }

            if (PasswordMismatch(user, request.Password))
            {
                return CreateInvalidResponse<AuthenticateUserResponse>("Invalid password.");
            }


            var mainMenus = user.Role.Menus.Where(p => p.ParentId == null && p.SubMenus
                    .Any(sm => sm.IsDisplay && sm.Permissions
                        .Any(smp => smp.Permission.Name.Equals("View") && smp.IsDisplay)))
                .OrderBy(r => r.DisplayOrder);


            return new AuthenticateUserResponse
            {
                Valid = true,
                Role = user.Role.Name,
                Username = user.Username,
                UserId = user.Id,
                TimeZone = user.TimeZone,
                Menus = mainMenus.CreateViewMenus().ToArray()
            };
        }


        protected (byte[], byte[]) GenerateCredential(string password)

        {
            var salt = _hashProvider.GenerateRandomSalt(32);
            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                _hashProvider.Hash(salt),
                _tgsHashConfig.HashAlgorithm);
            return (salt, hashedPassword);
        }

        protected bool PasswordMismatch(User user, string password)
        {
            var salt = _hashProvider.Hash(user.Salt);

            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                salt,
                _tgsHashConfig.HashAlgorithm);

            return !user.Password.SequenceEqual(hashedPassword);
        }
    }
}
