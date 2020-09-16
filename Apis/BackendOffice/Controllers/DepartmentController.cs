using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Department;
using TGS.Apis.BackendOffice.DataService;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseUserContextController
    {
        private readonly IGenericQueries<Department> _queries;
        private readonly IGenericCommands<Department> _commands;

        public DepartmentController(
            IGenericQueries<Department> queries,
            IGenericCommands<Department> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Description("Get departments")]
        public Task<PagedResponse<DepartmentModel>> GetListAsync([FromQuery] PageableRequest request)
        {
            return _queries.GetQueries()
                .Select(d => new DepartmentModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpPut("{id}"), Description("Update a department")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] DepartmentRequest request)
        {
            var department = await _queries.GetSingleAsync(p => p.Id == id);
            if (department == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            if (request.Name != department.Name && await _queries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NameExists);
            }

            department.Name = request.Name;
            department.UpdatedByUsername = Username;
            department.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(department);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpDelete("{id}"), Description("Delete a department")]
        public async Task<MessageResponse> DeleteAsync(Guid id)
        {
            var department = await _queries.GetSingleAsync(p => p.Id == id, d => d.Include(x => x.Users));
            if (department == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NotFound);
            }

            if (department.Users.Any())
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.CannotBeDeleted);
            }

            await _commands.DeleteAsync(department);
            return CreateValidResponse<MessageResponse>();
        }


        [HttpPost, Description("Create a department")]
        public async Task<MessageResponse> CreateAsync([FromBody] DepartmentRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NameExists);
            }

            var department = new Department
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Name = request.Name,
                Description = request.Name,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _commands.CreateAsync(department);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
