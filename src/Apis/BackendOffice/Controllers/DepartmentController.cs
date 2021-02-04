using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Department;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseContextController
    {
        private readonly ITgsCommands<Department> _commands;
        private readonly ITgsQueries<Department> _queries;

        public DepartmentController(
            ITgsQueries<Department> queries,
            ITgsCommands<Department> commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get departments")]
        public Task<PagedResponse<DepartmentModel>> GetListAsync([FromQuery] GetDepartmentsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.ResellerId, r => r.ResellerId == request.ResellerId)
                .Where(request.LicenseeId, l => l.LicenseeId == request.LicenseeId)
                .Where(request.BrandId, l => l.BrandId == request.BrandId)
                .Where(ResellerId != null, r => r.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                .Where(BrandId != null, l => l.BrandId == BrandId)
                .Select(d => new DepartmentModel
                {
                    Id = d.Id,
                    ResellerId = d.ResellerId,
                    LicenseeId = d.LicenseeId,
                    BrandId = d.BrandId,
                    Name = d.Name
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpPut("{id}")]
        [Description("Update a department")]
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

        [HttpDelete("{id}")]
        [Description("Delete a department")]
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


        [HttpPost]
        [Description("Create a department")]
        public async Task<MessageResponse> CreateAsync([FromBody] DepartmentRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Department.NameExists);
            }

            var department = new Department
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ResellerId = request.ResellerId,
                LicenseeId = request.LicenseeId,
                BrandId = request.BrandId,
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