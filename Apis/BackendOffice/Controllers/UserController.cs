using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/users")]
    [ApiController]
    public partial class UserController : BackendOfficeControllerBase
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _tgsHashConfig;
        private readonly IAntiforgery _antiForgery;
        private readonly IUserQueries _userQueries;
        private readonly IDepartmentQueries _departmentQueries;
        private readonly IRoleQueries _roleQueries;
        private readonly IUserCommands _userCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            IHashProvider hashProvider,
            ITgsHashConfig tgsHashConfig,
            IAntiforgery antiForgery,
            IUserQueries userQueries,
            IDepartmentQueries departmentQueries,
            IRoleQueries roleQueries,
            IUserCommands userCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _tgsHashConfig = tgsHashConfig;
            _antiForgery = antiForgery;
            _userQueries = userQueries;
            _departmentQueries = departmentQueries;
            _roleQueries = roleQueries;
            _userCommands = userCommands;
            _jsonSerializationProvider = jsonSerializationProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("")]
        [HttpGet, Description("Get users")]
        public Task<PagedResponse<UserModel>> GetUserAsync([FromQuery] GetUsersRequest request)
        {
            return _userQueries.GetUseQueries()
                .Where(request.Username, p => p.Username.Contains(request.Username))
                .Select(user => new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    Role = user.Role.Name,
                    DepartmentName = user.Department.Name,
                    LastLoginIp = user.LastLoginIp,
                    LastLoginTime = user.LastLoginTime,
                    Status = user.Status.ToString(),
                    CreatedOn = user.CreatedOn

                })
                .CreatePagedResponseAsync(request);
        }

        [Route("")]
        [HttpPost, Description("Create a user")]
        public async Task<MessageResponse> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            if (await _userQueries.UsernameExistsAsync(request.Username))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.UsernameExistedAlready);
            }

            if (await _roleQueries.GetRoleByIdAsync(request.RoleId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.NotFound);
            }

            if (await _departmentQueries.GetDepartmentByIdAsync(request.DepartmentId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            var (salt, password) = GenerateCredential(request.Password);
            var user = new User
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Username = request.Username,
                Name = request.Name,
                Password = password,
                Salt = salt,
                RoleId = request.RoleId,
                DepartmentId = request.DepartmentId,
                TimeZone = request.TimeZone,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _userCommands.CreateUserAsync(user);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{userId}"), Description("Update a user")]
        public async Task<MessageResponse> UpdateUserAsync(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var user = await _userQueries.GetUserByIdAsync(userId);

            if (user == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.NotFound);
            }
            if (await _roleQueries.GetRoleByIdAsync(request.RoleId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.NotFound);
            }
            if (await _departmentQueries.GetDepartmentByIdAsync(request.DepartmentId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            user.Name = request.Name;
            user.RoleId = request.RoleId;
            user.DepartmentId = request.DepartmentId;
            user.UpdatedByUsername = Username;
            user.UpdatedOn = DateTimeOffset.Now;

            await _userCommands.UpdateUserAsync(user);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{userId}/password"), Description("Update user password")]
        public async Task<MessageResponse> UpdateUserPasswordAsync(Guid userId, [FromBody] UpdatePasswordRequest request)
        {
            var user = await _userQueries.GetUserByIdAsync(userId);

            if (user == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.NotFound);
            }

            var (salt, password) = GenerateCredential(request.Password);
            user.Password = password;
            user.Salt = salt;
            user.UpdatedByUsername = Username;
            user.UpdatedOn = DateTimeOffset.Now;

            await _userCommands.UpdateUserAsync(user);
            return CreateValidResponse<MessageResponse>();
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
