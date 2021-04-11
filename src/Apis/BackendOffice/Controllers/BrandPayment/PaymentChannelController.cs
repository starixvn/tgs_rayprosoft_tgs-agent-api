using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentChannelController : BaseContextController
    {
        private readonly ITgsCommands<PaymentChannel> _commands;
        private readonly ITgsQueries<PaymentChannel> _queries;

        public PaymentChannelController(
            ITgsQueries<PaymentChannel> queries,
            ITgsCommands<PaymentChannel> commands)
        {
            _queries = queries;
            _commands = commands;
        }


        [HttpGet]
        [Description("Get PaymentChannels")]
        public IAsyncEnumerable<PaymentChannelModel> GetAllPaymentChannelsAsync()
        {
            return _queries.GetQueries()
                .Select(q => new PaymentChannelModel
                {
                    Id = q.Id,
                    Code = q.Code,
                })
                .AsNoTracking()
                .ToAsyncEnumerable();
        }

        //[HttpGet("{id}")]
        //[Description("Get a PaymentChannel")]
        //public async Task<GetPaymentChannelResponse> GetPaymentChannelAsync(Guid id)
        //{
        //    var channel = await _queries.GetSingleAsync(q => q.Id == id,
        //        p => p
        //            .Include(pc => pc.Payment)
        //            .Include(pc => pc.PaymentMethod)
        //            .AsNoTracking());

        //    return new GetPaymentChannelResponse
        //    {
        //        Valid = true,
        //        PaymentChannel = new PaymentChannelModel
        //        {
        //            Id = channel.Id,
        //            Code = channel.Code,
        //            Name = channel.Name,
        //            PaymentName = channel.Payment.Name,
        //            PaymentMethodName = channel.PaymentMethod.Name,
        //            Description = channel.Description,
        //            CreatedOn = channel.CreatedOn,
        //            UpdatedOn = channel.UpdatedOn,
        //            CreatedByUsername = channel.CreatedByUsername,
        //            UpdatedByUsername = channel.UpdatedByUsername
        //        }
        //    };
        //}

        //[HttpPost]
        //[Description("Create a PaymentChannel")]
        //public async Task<MessageResponse> CreateAsync(CreatePaymentChannelRequest request)
        //{
        //    if (await _queries.ExistsAsync(p => p.Code == request.Code))
        //    {
        //        return CreateInvalidResponse<MessageResponse>(Resources.PaymentChannel.CodeExists);
        //    }

        //    await _commands.CreateAsync(new PaymentChannel
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = request.Name,
        //        Code = request.Code,
        //        PaymentId = request.PaymentId,
        //        PaymentMethodId = request.PaymentMethodId,
        //        CreatedByUsername = Username,
        //        UpdatedByUsername = Username,
        //        CreatedOn = DateTimeOffset.Now,
        //        UpdatedOn = DateTimeOffset.Now
        //    });

        //    return CreateValidResponse<MessageResponse>();
        //}

        //[HttpPut("{id}")]
        //[Description("Update a PaymentChannel")]
        //public async Task<MessageResponse> UpdateAsync(Guid id, UpdatePaymentChannelRequest request)
        //{
        //    var channel = await _queries.GetSingleAsync(p => p.Id == id);

        //    if (channel == null)
        //    {
        //        return CreateInvalidResponse<MessageResponse>(Resources.PaymentChannel.NotFound);
        //    }

        //    channel.PaymentId = request.PaymentId;
        //    channel.PaymentMethodId = request.PaymentMethodId;
        //    channel.Description = request.Description;
        //    channel.UpdatedByUsername = Username;
        //    channel.UpdatedOn = DateTimeOffset.Now;

        //    await _commands.UpdateAsync(channel);

        //    return CreateValidResponse<MessageResponse>();
        //}
    }
}