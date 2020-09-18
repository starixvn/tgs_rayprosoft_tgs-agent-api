using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.StationLetter;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using VerifyRequest = TGS.Apis.BackendOffice.Controllers.Messages.StationLetter.VerifyRequest;

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
        public Task<PagedResponse<StationLetterModel>> GetListAsync([FromQuery] PageableRequest request)
        {
            return _queries.GetQueries()
                .Select(a => new StationLetterModel
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
                stationLetter.Status = StationLetterStatus.InVerify;
            }
            else if (type == types[1])
            {
                stationLetter.Status = StationLetterStatus.Rejected;
            }
            stationLetter.Remarks = request.Remarks;
            stationLetter.UpdatedByUsername = Username;
            stationLetter.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}"), Description("Update a station letter")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] StationLetterRequest request)
        {
            var stationLetter = await _queries.GetSingleAsync(p => p.Id == id);
            if (stationLetter == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.NotFound);
            }

            stationLetter.Subject = request.Content;
            stationLetter.Content = request.Subject;
            stationLetter.UpdatedByUsername = Username;
            stationLetter.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost, Description("Create a station letter")]
        public async Task<MessageResponse> CreateAsync([FromBody] StationLetterRequest request)
        {
            var stationLetter = new StationLetter
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

            await _commands.CreateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost, Description("Upload player file")]
        public async Task<MessageResponse> UpdateAsync(IFormFile file)
        {
            var users = new List<UserModel>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        users.Add(new UserModel { Name = reader.GetValue(0).ToString(), Username = reader.GetValue(1).ToString() });
                    }
                }
            }
            return CreateValidResponse<MessageResponse>();
        }
    }
}
