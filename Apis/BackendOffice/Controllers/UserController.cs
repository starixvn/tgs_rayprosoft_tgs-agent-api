using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.Controllers
{
    [AllowAnonymous]
    [Route("api/users")]
    [ApiController]
    public partial class UserController : BackendOfficeControllerBase
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _tgsHashConfig;
        private readonly IAntiforgery _antiForgery;
        private readonly IUserQueries _userQueries;
        private readonly IUserCommands _userCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public UserController(
            IHashProvider hashProvider,
            ITgsHashConfig tgsHashConfig,
            IAntiforgery antiForgery,
            IUserQueries userQueries,
            IUserCommands userCommands,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _hashProvider = hashProvider;
            _tgsHashConfig = tgsHashConfig;
            _antiForgery = antiForgery;
            _userQueries = userQueries;
            _userCommands = userCommands;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        [Route("")]
        [HttpPost, Description("Get user list")]
        public Task<PagedResponse<UserModel>> GetUserAsync(GetUsersRequest request)
        {
            return _userQueries.GetUseQueries()
                .Where(request.Username, p => p.Username.Contains(request.Username))
                .Select(p => new UserModel())
                .CreatePagedResponseAsync(request);

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
    }
}
