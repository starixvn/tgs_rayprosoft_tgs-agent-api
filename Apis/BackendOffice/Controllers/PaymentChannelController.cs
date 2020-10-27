using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor;
using TGS.Apis.BackendOffice.DataService.Commands;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentChannelController : BaseContextController
    {
        private readonly IBackendOfficeCommands<PaymentChannel> _commands;
        private readonly IPaymentVendorCommands _paymentVendorCommands;
        private readonly IBackendOfficeQueries<PaymentChannel> _queries;

        public PaymentChannelController(
            IBackendOfficeQueries<PaymentChannel> queries,
            IBackendOfficeCommands<PaymentChannel> commands,
            IPaymentVendorCommands paymentVendorCommands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
            _paymentVendorCommands = paymentVendorCommands;
        }


        [HttpGet]
        [Description("Get PaymentChannels")]
        public Task<PagedResponse<PaymentChannelModel>> GetPaymentChannelsAsync(
            [FromQuery] PaymentChannelsRequest request)
        {
            return _queries.GetQueries().Select(q => new PaymentChannelModel
            {
                Id = q.Id,
                Code = q.Code,
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
            var channel = await _queries.GetSingleAsync(q => q.Id == id);

            return new GetPaymentChannelResponse
            {
                Valid = true,
                PaymentChannel = new PaymentChannelModel
                {
                    Id = channel.Id,
                    Code = channel.Code,
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