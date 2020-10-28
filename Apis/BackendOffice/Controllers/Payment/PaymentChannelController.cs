using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentChannelController : BaseContextController
    {
        private readonly ITgsCommands<PaymentChannel> _commands;
        private readonly ITgsQueries<PaymentChannel> _queries;

        public PaymentChannelController(
            ITgsQueries<PaymentChannel> queries,
            ITgsCommands<PaymentChannel> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }


        [HttpGet]
        [Description("Get PaymentChannels")]
        public Task<PagedResponse<PaymentChannelModel>> GetPaymentChannelsAsync(
            [FromQuery] PaymentChannelsRequest request)
        {
            return _queries.GetQueries()
                .Include(pc => pc.PaymentVendor)
                .Include(pc => pc.PaymentMethod)
                .Where(request.Code, q => q.Code == request.Code)
                .Select(q => new PaymentChannelModel
                {
                    Id = q.Id,
                    Code = q.Code,
                    Name = q.Name,
                    PaymentVendorCode = q.PaymentVendor.Code,
                    PaymentMethodCode = q.PaymentMethod.Code,
                    Description = q.Description,
                    CreatedByUsername = q.CreatedByUsername,
                    UpdatedByUsername = q.UpdatedByUsername,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a PaymentChannel")]
        public async Task<GetPaymentChannelResponse> GetPaymentChannelAsync(Guid id)
        {
            var channel = await _queries.GetSingleAsync(q => q.Id == id,
                p => p
                    .Include(pc => pc.PaymentVendor)
                    .Include(pc => pc.PaymentMethod));

            return new GetPaymentChannelResponse
            {
                Valid = true,
                PaymentChannel = new PaymentChannelModel
                {
                    Id = channel.Id,
                    Code = channel.Code,
                    Name = channel.Name,
                    PaymentVendorCode = channel.PaymentVendor.Code,
                    PaymentMethodCode = channel.PaymentMethod.Code,
                    Description = channel.Description,
                    CreatedOn = channel.CreatedOn,
                    UpdatedOn = channel.UpdatedOn,
                    CreatedByUsername = channel.CreatedByUsername,
                    UpdatedByUsername = channel.UpdatedByUsername
                }
            };
        }

        [HttpPost]
        [Description("Create a PaymentChannel")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateUpdatePaymentChannelRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Code == request.Code))
                return CreateInvalidResponse<MessageResponse>(Resources.PaymentChannel.CodeExists);
            await _commands.CreateAsync(new PaymentChannel
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                PaymentVendorId = request.PaymentVendorId,
                PaymentMethodId = request.PaymentMethodId,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut]
        [Description("Update a PaymentChannel")]
        public async Task<MessageResponse> UpdateAsync([FromBody] CreateUpdatePaymentChannelRequest request)
        {
            var channel = await _queries.GetSingleAsync(p => p.Id == request.Id);

            if (channel == null) return CreateInvalidResponse<MessageResponse>(Resources.PaymentChannel.NotFound);

            channel.PaymentVendorId = request.PaymentVendorId;
            channel.PaymentMethodId = request.PaymentMethodId;
            channel.Description = request.Description;
            channel.UpdatedByUsername = Username;
            channel.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(channel);

            return CreateValidResponse<MessageResponse>();
        }
    }
}