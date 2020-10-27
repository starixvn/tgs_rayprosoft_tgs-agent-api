using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PaymentVendorController : BaseContextController
    {
        private readonly IBackendOfficeCommands<PaymentVendor> _commands;
        private readonly IPaymentVendorCommands _paymentVendorCommands;
        private readonly IBackendOfficeQueries<PaymentVendor> _queries;

        public PaymentVendorController(
            IBackendOfficeQueries<PaymentVendor> queries,
            IBackendOfficeCommands<PaymentVendor> commands,
            IPaymentVendorCommands paymentVendorCommands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
            _paymentVendorCommands = paymentVendorCommands;
        }

        [HttpGet]
        [Description("Get PaymentVendors")]
        public Task<PagedResponse<PaymentVendorModel>> GetPaymentVendorsAsync([FromQuery] PaymentMethodsRequest request)
        {
            return _queries.GetQueries().Select(q => new PaymentVendorModel
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