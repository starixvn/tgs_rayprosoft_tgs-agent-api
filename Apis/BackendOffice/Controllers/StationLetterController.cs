using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.StationLetter;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.File;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationLetterController : BaseContextController
    {
        private readonly ITgsCommands<StationLetter> _commands;
        private readonly IFileService _fileService;
        private readonly ITgsCommands<StationLetterPlayer> _playerCommands;
        private readonly ITgsQueries<Player> _playerQueries;
        private readonly ITgsQueries<StationLetter> _queries;

        private readonly Dictionary<string, StationLetterStatus> _typeDictionary =
            new Dictionary<string, StationLetterStatus>
            {
                {"pass", StationLetterStatus.Published},
                {"reject", StationLetterStatus.Rejected}
            };

        private readonly IUploadService _uploadService;

        public StationLetterController(
            ITgsQueries<StationLetter> queries,
            ITgsQueries<Player> playerQueries,
            ITgsCommands<StationLetter> commands,
            ITgsCommands<StationLetterPlayer> playerCommands,
            IUploadService uploadService,
            IFileService fileService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _playerQueries = playerQueries;
            _commands = commands;
            _playerCommands = playerCommands;
            _uploadService = uploadService;
            _fileService = fileService;
        }

        [HttpGet]
        [Description("Get station letters")]
        public Task<PagedResponse<StationLetterModel>> GetListAsync([FromQuery] GetStationLettersRequest request)
        {
            return _queries.GetQueries()
                .Where(p => p.Status == request.Status)
                .Select(a => new StationLetterModel
                {
                    Id = a.Id,
                    Subject = a.Subject,
                    Content = a.Content,
                    Remarks = a.Remarks,
                    AssignedAllPlayers = a.AssignedAllPlayers,
                    CreatedByUsername = a.CreatedByUsername,
                    VerifiedByUsername = a.VerifiedByUsername,
                    Status = a.Status.ToString(),
                    CreatedOn = a.CreatedOn
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpPut("{id}/{type}")]
        [Description("Verify a station letter")]
        public async Task<MessageResponse> VerifyAsync(Guid id, string type, [FromBody] VerifyRequest request)
        {
            if (!_typeDictionary.ContainsKey(type))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.InvalidUrl);
            }

            var stationLetter = await _queries.GetSingleAsync(p => p.Id == id);
            if (stationLetter == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.NotFound);
            }

            if (stationLetter.Status != StationLetterStatus.InVerify)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.InvalidStatus);
            }

            var targetStatus = _typeDictionary
                .FirstOrDefault(t => t.Key == type);

            stationLetter.Status = targetStatus.Value;
            stationLetter.VerifiedByUsername = Username;
            stationLetter.Remarks = request.Remarks;
            stationLetter.UpdatedByUsername = Username;
            stationLetter.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a station letter")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] StationLetterRequest request)
        {
            var stationLetter = await _queries.GetSingleAsync(p => p.Id == id);
            if (stationLetter == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.StationLetter.NotFound);
            }

            stationLetter.Subject = request.Subject;
            stationLetter.Content = request.Content;
            stationLetter.UpdatedByUsername = Username;
            stationLetter.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(stationLetter);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost]
        [Description("Create a station letter")]
        public async Task<MessageResponse> CreateAsync([FromBody] StationLetterRequest request)
        {
            var stationLetter = new StationLetter
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Subject = request.Subject,
                Content = request.Content,
                AssignedAllPlayers = request.AssignedAllPlayers,
                Status = StationLetterStatus.InVerify,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var allPlayers = await _playerQueries.GetQueries()
                .Where(p => p.Status == PlayerStatus.Enabled)
                .ToArrayAsync();

            if (!request.AssignedAllPlayers)
            {
                allPlayers = allPlayers.Where(p => request.PlayerUsernames.Contains(p.Username)).ToArray();
            }

            var players = allPlayers.Select(p => new StationLetterPlayer
            {
                PlayerCode = p.PlayerCode,
                StationLetterId = stationLetter.Id
            }).ToArray();

            await _commands.CreateAsync(stationLetter);
            await _playerCommands.BatchInsertOrUpdateAsync(players);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost("upload")]
        [Description("Upload player file")]
        public async Task<GetPlayerFromFileResponse> UploadAsync(IFormFile file)
        {
            var uploadedResult = await _uploadService.GetUploadedExcelAsync(file);
            var users = uploadedResult.Select(p => p.FirstOrDefault());

            var allPlayers = await _playerQueries.GetQueries()
                .Where(p => p.Status == PlayerStatus.Enabled && users.Contains(p.Username))
                .ToArrayAsync();

            var players = from u in users
                join p in allPlayers on u equals p.Username
                    into tmpU
                from p in tmpU.DefaultIfEmpty()
                select new GetPlayerFromFileResponse.PlayerModel
                {
                    Username = u,
                    Exists = p != null
                };

            return new GetPlayerFromFileResponse
            {
                Valid = true,
                Players = players.ToArray()
            };
        }

        [HttpPost("export")]
        [Description("Export player file")]
        public async Task<HttpResponseMessage> ExportAsync()
        {
            var wb = await Task.Run(() =>
            {
                var workbook = new XLWorkbook();
                var ws = workbook.AddWorksheet("Player");
                ws.Cell("A1").Value = "Player1";
                ws.Cell("A2").Value = "Player2";

                return workbook;
            });
            return wb.Deliver("Player.xlsx");
        }

        [HttpPost("upload/image")]
        [Description("Upload image")]
        public async Task<ImageUploadResponse> UploadImage([FromForm] ImageUploadRequest request)
        {
            if (request.FormFile == null)
            {
                return CreateInvalidResponse<ImageUploadResponse>("");
                // This is the point where we should persist the image bytes to a persistent store.
                // This could be anything from storing the image in a database (MySql, SqlServer), disk,
                // or object store (Amazon S3, Azure Blog Storage, Digital Ocean Spaces)

                // Once the image is stored, we'll want to return the url of where the image is stored
                // in the response. For the time being, this image url will be hardcoded so that the
                // TinyMCE editor functions.
            }

            var path = await _fileService.UploadFileAsync(request.FormFile);
            var imageUrl = $"{Request.Scheme}://{Request.Host}/{path}";

            return new ImageUploadResponse
            {
                Valid = true,
                Url = imageUrl
            };
        }
    }
}