using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Announcement;
using TGS.Apis.BackendOffice.Controllers.Messages.StationLetter;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using VerifyRequest = TGS.Apis.BackendOffice.Controllers.Messages.Announcement.VerifyRequest;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationLetterController : BaseUserContextController
    {
        private readonly ITgsQueries<StationLetter> _queries;
        private readonly ITgsCommands<StationLetter> _commands;

        public StationLetterController(
            ITgsQueries<StationLetter> queries,
            ITgsCommands<StationLetter> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Description("Get station letters")]
        public Task<PagedResponse<AnnouncementModel>> GetListAsync([FromQuery] PageableRequest request)
        {
            return _queries.GetQueries()
                .Select(a => new AnnouncementModel
                {
                    Id = a.Id,
                    Subject = a.Subject,
                    Content = a.Content,
                    Remarks = a.Remarks,
                    CreatedByUsername = a.CreatedByUsername,
                    VerifiedByUsername = a.VerifiedByUsername,
                    Status = a.Status.ToString()
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpPut("{id}/{type}"), Description("Update a station letter")]
        public async Task<MessageResponse> VerifyAsync(Guid id, string type, [FromBody] VerifyRequest request)
        {
            var types = new[] { "pass", "reject" };
            var stationLetter = await _queries.GetSingleAsync(p => p.Id == id);
            if (stationLetter == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.NotFound);
            }

            if (!types.Contains(type))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.InvalidUrl);
            }

            if (type == types[0])
            {
                stationLetter.Remarks = request.Remarks;
                stationLetter.Status = StationLetterStatus.InVerify;
            }
            else if (type == types[1])
            {
                stationLetter.Remarks = request.Remarks;
                stationLetter.Status = StationLetterStatus.Rejected;
            }

            stationLetter.UpdatedByUsername = Username;
            stationLetter.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}"), Description("Update a station letter")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] StationLetterRequest request)
        {
            var announcement = await _queries.GetSingleAsync(p => p.Id == id);
            if (announcement == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.NotFound);
            }

            announcement.Subject = request.Content;
            announcement.Content = request.Subject;
            announcement.UpdatedByUsername = Username;
            announcement.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(announcement);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost, Description("Create a station letter")]
        public async Task<MessageResponse> CreateAsync([FromBody] StationLetterRequest request)
        {
            var announcement = new StationLetter
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Subject = request.Subject,
                Content = request.Content,
                Status = StationLetterStatus.InVerify,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _commands.CreateAsync(announcement);
            return CreateValidResponse<MessageResponse>();
        }
    }
}
