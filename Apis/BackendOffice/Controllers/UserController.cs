using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/users")]
    [ApiController]
    public class UserController : BaseContextController
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly IBackendOfficeQueries<User> _userQueries;
        private readonly IBackendOfficeQueries<Department> _departmentQueries;
        private readonly IBackendOfficeQueries<Role> _roleQueries;
        private readonly IBackendOfficeCommands<User> _userCommands;

        public UserController(
            IPasswordHandler passwordHandler,
            IBackendOfficeQueries<User> userQueries,
            IBackendOfficeQueries<Department> departmentQueries,
            IBackendOfficeQueries<Role> roleQueries,
            IBackendOfficeCommands<User> userCommands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _passwordHandler = passwordHandler;
            _userQueries = userQueries;
            _departmentQueries = departmentQueries;
            _roleQueries = roleQueries;
            _userCommands = userCommands;
        }

        [Route("")]
        [HttpGet, Description("Get users")]
        public Task<PagedResponse<UserModel>> GetListAsync([FromQuery] GetUsersRequest request)
        {
            return _userQueries.GetQueries()
                .Where(request.Username, p => p.Username.Contains(request.Username))
                .Select(user => new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    RoleId = user.RoleId,
                    DepartmentId = user.DepartmentId,
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
        public async Task<MessageResponse> CreateAsync([FromBody] CreateUserRequest request)
        {
            if (await _userQueries.ExistsAsync(p => p.Username == request.Username))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.UsernameExistedAlready);
            }

            if (await _roleQueries.GetSingleAsync(p => p.Id == request.RoleId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.NotFound);
            }

            if (await _departmentQueries.GetSingleAsync(p => p.Id == request.DepartmentId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);
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

            await _userCommands.CreateAsync(user);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{userId}"), Description("Update a user")]
        public async Task<MessageResponse> UpdateAsync(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var user = await _userQueries.GetSingleAsync(p => p.Id == userId);

            if (user == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.NotFound);
            }

            if (await _roleQueries.GetSingleAsync(r => r.Id == request.RoleId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Role.NotFound);
            }
            
            if (await _departmentQueries.GetSingleAsync(p => p.Id == request.DepartmentId) == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            user.Name = request.Name;
            user.RoleId = request.RoleId;
            user.DepartmentId = request.DepartmentId;
            user.Status = request.Status;
            user.UpdatedByUsername = Username;
            user.UpdatedOn = DateTimeOffset.Now;

            await _userCommands.UpdateAsync(user);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{userId}/password"), Description("Update user password")]
        public async Task<MessageResponse> UpdatePasswordAsync(Guid userId, [FromBody] UpdatePasswordRequest request)
        {
            var user = await _userQueries.GetSingleAsync(p => p.Id == userId);

            if (user == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.User.NotFound);
            }

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);
            user.Password = password;
            user.Salt = salt;
            user.UpdatedByUsername = Username;
            user.UpdatedOn = DateTimeOffset.Now;

            await _userCommands.UpdateAsync(user);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
