using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Department;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Queries;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BackendOfficeControllerBase
    {
        private readonly IDepartmentQueries _queries;
        private readonly IGenericCommands<Department> _commands;

        public DepartmentController(
            IDepartmentQueries queries,
            IGenericCommands<Department> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Description("Get departments")]
        public Task<PagedResponse<DepartmentModel>> GetDepartmentsAsync([FromQuery] PageableRequest request)
        {
            return _queries.GetDepartmentQueries()
                .Select(d => new DepartmentModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .CreatePagedResponseAsync(request);
        }


        [HttpPut("{departmentId}"), Description("Update a department")]
        public async Task<MessageResponse> UpdateDepartmentAsync(Guid departmentId, [FromBody] DepartmentRequest request)
        {
            var department = await _queries.GetDepartmentByIdAsync(departmentId);
            if (department == null)
            {
                return CreateInvalidResponse<MessageResponse>($"{Resources.Department.NotFound}");
            }

            if (request.Name != department.Name && await _queries.NameExistsAsync(request.Name))
            {
                return CreateInvalidResponse<MessageResponse>($"{Resources.Department.NameExists}");
            }

            department.Name = request.Name;
            department.UpdatedByUsername = Username;
            department.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(department);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpDelete("{departmentId}"), Description("Delete a department")]
        public async Task<MessageResponse> DeleteDepartmentAsync(Guid departmentId)
        {
            var department = await _queries.GetDepartmentByIdAsync(departmentId);
            if (department == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            if (department.Users.Any())
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.CannotDeleteRole);
            }

            await _commands.DeleteAsync(department);
            return CreateValidResponse<MessageResponse>();
        }


        [HttpPost, Description("Create a department")]
        public async Task<GetRoleResponse> CreateRoleAsync([FromBody] DepartmentRequest request)
        {
            if (await _queries.NameExistsAsync(request.Name))
            {
                return CreateInvalidResponse<GetRoleResponse>(Resources.Role.NameExistd);
            }

            var department = new Department
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Name = request.Name,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _commands.CreateAsync(department);
            return CreateValidResponse<GetRoleResponse>();
        }
    }
}
