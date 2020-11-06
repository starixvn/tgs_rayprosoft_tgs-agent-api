using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Announcement;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : BaseContextController
    {
        private readonly ITgsCommands<Announcement> _commands;
        private readonly ITgsQueries<Announcement> _queries;

        private readonly Dictionary<string, dynamic> _typeDictionary = new Dictionary<string, dynamic>
        {
            {
                "pass", new
                {
                    AllowedStatus = AnnounceStatus.InVerify,
                    VerifiedStatus = AnnounceStatus.Effective,
                    Message = Resources.Announcement.OnlyInVerifyAnnouncementCanBePassed,
                    CanRemarks = true
                }
            },
            {
                "reject", new
                {
                    AllowedStatus = AnnounceStatus.InVerify,
                    VerifiedStatus = AnnounceStatus.Rejected,
                    Message = Resources.Announcement.OnlyInVerifyAnnouncementCanBeRejected,
                    CanRemarks = true
                }
            },
            {
                "invalid", new
                {
                    AllowedStatus = AnnounceStatus.Effective,
                    VerifiedStatus = AnnounceStatus.Invalid,
                    Message = Resources.Announcement.OnlyEffectiveAnnouncementCanBeInvalid,
                    CanRemarks = false
                }
            }
        };

        public AnnouncementController(
            ITgsQueries<Announcement> queries,
            ITgsCommands<Announcement> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get announcements")]
        public Task<PagedResponse<AnnouncementModel>> GetListAsync([FromQuery] GetAnnouncementsRequest request)
        {
            return _queries.GetQueries()
                .Where(p => request.Status == null || request.Status.Contains(p.Status))
                .Select(a => new AnnouncementModel
                {
                    Id = a.Id,
                    Subject = a.Subject,
                    Content = a.Content,
                    Remarks = a.Remarks,
                    CreatedByUsername = a.CreatedByUsername,
                    VerifiedByUsername = a.VerifiedByUsername,
                    From = a.From,
                    To = a.To,
                    UpdatedOn = a.UpdatedOn,
                    Status = DateTimeOffset.Now > a.To ? AnnounceStatus.Expired.ToString() : a.Status.ToString()
                })
                .CreatePagedResponseAsync(request);
        }

        [HttpPut("{id}/{type}")]
        [Description("Verify a announcement")]
        public async Task<MessageResponse> VerifyAsync(Guid id, string type, [FromBody] VerifyRequest request)
        {
            if (!_typeDictionary.ContainsKey(type))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Announcement.InvalidUrl);
            }

            var announcement = await _queries.GetSingleAsync(p => p.Id == id);
            if (announcement == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Announcement.NotFound);
            }

            foreach (var types in _typeDictionary.Where(types => types.Key == type))
            {
                if (types.Value.AllowedStatus != announcement.Status)
                {
                    return CreateInvalidResponse<MessageResponse>(types.Value.Message);
                }

                if (types.Value.CanRemarks)
                {
                    announcement.Remarks = request.Remarks;
                }

                announcement.Status = types.Value.VerifiedStatus;
            }

            announcement.UpdatedByUsername = Username;
            announcement.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(announcement);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a announcement")]
        public async Task<MessageResponse> UpdateAsync(Guid id, [FromBody] AnnouncementRequest request)
        {
            var announcement = await _queries.GetSingleAsync(p => p.Id == id);
            if (announcement == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Announcement.NotFound);
            }

            if (announcement.Status != AnnounceStatus.Effective && announcement.Status != AnnounceStatus.Invalid)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Announcement.CannotUpdateDueToInvalidStatus);
            }


            announcement.Subject = request.Subject;
            announcement.Content = request.Content;
            announcement.From = request.From;
            announcement.To = request.To;
            announcement.UpdatedByUsername = Username;
            announcement.UpdatedOn = DateTimeOffset.Now;
            announcement.Status = AnnounceStatus.InVerify;

            await _commands.UpdateAsync(announcement);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPost]
        [Description("Create a announcement")]
        public async Task<MessageResponse> CreateAsync([FromBody] AnnouncementRequest request)
        {
            var announcement = new Announcement
            {
                Id = GuidExtensions.NewSequentialGuid(),
                DisplayOrder = 99,
                Subject = request.Subject,
                Content = request.Content,
                From = request.From,
                To = request.To,
                Status = AnnounceStatus.InVerify,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            await _commands.CreateAsync(announcement);
            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("ChangeOrders")]
        [Description("Change Announcement Order")]
        public async Task<MessageResponse> ChangeOrder(ChangeOrderRequest request)
        {
            var ids = request.Orders.Select(q => q.Id);
            var payments = await _queries.GetQueries().Where(q => ids.Contains(q.Id)).ToListAsync();
            payments.ForEach(p => p.DisplayOrder = request.Orders.FirstOrDefault(x => x.Id == p.Id).DisplayOrder);

            await _commands.BatchInsertOrUpdateAsync(payments.ToArray());

            return CreateValidResponse<MessageResponse>();
        }
    }
}