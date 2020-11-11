using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.VipLevel;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipLevelController : BaseContextController
    {
        private readonly ITgsCommands<VipLevel> _commands;
        private readonly ITgsQueries<VipLevel> _queries;

        public VipLevelController(
            ITgsQueries<VipLevel> queries,
            ITgsCommands<VipLevel> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get vip levels")]
        public async Task<VipLevelsResponse> GetListAsync()
        {
            var vipLevels = await _queries.GetQueries()
                .Select(d => new VipLevelModel
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name
                }).ToArrayAsync();
            return new VipLevelsResponse
            {
                Valid = true,
                VipLevels = vipLevels
            };
        }

        [HttpPut("{id}")]
        [Description("Update a vip level")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] VipLevelRequest request)
        {
            var vipLevel = await _queries.GetSingleAsync(p => p.Id == id);
            if (vipLevel == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.NotFound);
            }

            if (request.Code != vipLevel.Code && await _queries.ExistsAsync(p => p.Code == request.Code))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.CodeExists);
            }

            if (request.Name != vipLevel.Name && await _queries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.NameExists);
            }

            vipLevel.Code = request.Code;
            vipLevel.Name = request.Name;
            vipLevel.UpdatedByUsername = Username;
            vipLevel.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(vipLevel);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpDelete("{id}")]
        [Description("Delete a vip level")]
        public async Task<MessageResponse> DeleteAsync(Guid id)
        {
            var vipLevel = await _queries.GetSingleAsync(p => p.Id == id, d => d.Include(x => x.Players));
            if (vipLevel == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.NotFound);
            }

            if (vipLevel.Players.Any())
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.CannotBeDeleted);
            }

            await _commands.DeleteAsync(vipLevel);
            return CreateValidResponse<MessageResponse>();
        }


        [HttpPost]
        [Description("Create a vip level")]
        public async Task<MessageResponse> CreateAsync([FromBody] VipLevelRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.VipLevel.NameExists);
            }

            var vipLevel = new VipLevel
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Code = request.Code,
                Name = request.Name,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _commands.CreateAsync(vipLevel);
            return CreateValidResponse<MessageResponse>();
        }
    }
}