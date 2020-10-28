using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentVendor;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentVendorController : BaseContextController
    {
        private readonly ITgsCommands<PaymentVendor> _commands;
        private readonly ITgsQueries<PaymentVendor> _queries;

        public PaymentVendorController(
            ITgsQueries<PaymentVendor> queries,
            ITgsCommands<PaymentVendor> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get PaymentVendors")]
        public Task<PagedResponse<PaymentVendorModel>> GetPaymentVendorsAsync([FromQuery] PaymentVendorsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Code, q => q.Code == request.Code)
                .Select(q => new PaymentVendorModel
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
        [Description("Get a PaymentVendor")]
        public async Task<GetPaymentVendorResponse> GetPaymentVendorAsync(Guid id)
        {
            var vendor = await _queries.GetSingleAsync(q => q.Id == id, q =>
                q.Include(i => i.PaymentMethods));

            return new GetPaymentVendorResponse
            {
                Valid = true,
                PaymentVendor = new PaymentVendorModel
                {
                    Id = vendor.Id,
                    Code = vendor.Code,
                    Description = vendor.Description,
                    CreatedOn = vendor.CreatedOn,
                    UpdatedOn = vendor.UpdatedOn,
                    CreatedByUsername = vendor.CreatedByUsername,
                    UpdatedByUsername = vendor.UpdatedByUsername
                }
            };
        }

        [HttpPost]
        [Description("Create a PaymentVendor")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateUpdatePaymentVendorRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Code == request.Code))
                return CreateInvalidResponse<MessageResponse>(Resources.PaymentVendor.CodeExists);

            await _commands.CreateAsync(new PaymentVendor
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut]
        [Description("Update a PaymentVendor")]
        public async Task<MessageResponse> UpdateAsync([FromBody] CreateUpdatePaymentVendorRequest request)
        {
            var vendor = await _queries.GetSingleAsync(p => p.Id == request.Id);

            if (vendor == null) return CreateInvalidResponse<MessageResponse>(Resources.PaymentVendor.NotFound);

            vendor.Description = request.Description;
            vendor.UpdatedByUsername = Username;
            vendor.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(vendor);

            return CreateValidResponse<MessageResponse>();
        }
    }
}