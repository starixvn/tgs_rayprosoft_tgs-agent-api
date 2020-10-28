using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : BaseContextController
    {
        private readonly ITgsCommands<PaymentMethod> _commands;
        private readonly ITgsQueries<PaymentMethod> _queries;

        public PaymentMethodController(
            ITgsQueries<PaymentMethod> queries,
            ITgsCommands<PaymentMethod> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get PaymentMethods")]
        public Task<PagedResponse<PaymentMethodModel>> GetPaymentMethodsAsync([FromQuery] PaymentMethodsRequest request)
        {
            return _queries.GetQueries()
                .Include(p => p.PaymentVendor)
                .Where(request.Code, q => q.Code == request.Code)
                .Select(q => new PaymentMethodModel
                {
                    Id = q.Id,
                    Code = q.Code,
                    Description = q.Description,
                    PaymentVendorCode = q.PaymentVendor.Code,
                    CreatedByUsername = q.CreatedByUsername,
                    UpdatedByUsername = q.UpdatedByUsername,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a PaymentMethod")]
        public async Task<GetPaymentMethodResponse> GetPaymentMethodAsync(Guid id)
        {
            var vendor = await _queries.GetSingleAsync(q => q.Id == id,
                p => p.Include(pm => pm.PaymentVendor));

            return new GetPaymentMethodResponse
            {
                Valid = true,
                PaymentMethod = new PaymentMethodModel
                {
                    Id = vendor.Id,
                    Code = vendor.Code,
                    Description = vendor.Description,
                    PaymentVendorCode = vendor.PaymentVendor.Code,
                    CreatedOn = vendor.CreatedOn,
                    UpdatedOn = vendor.UpdatedOn,
                    CreatedByUsername = vendor.CreatedByUsername,
                    UpdatedByUsername = vendor.UpdatedByUsername
                }
            };
        }

        [HttpPost]
        [Description("Create a PaymentMethod")]
        public async Task<MessageResponse> CreateAsync([FromBody] CreateUpdatePaymentMethodRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Code == request.Code))
                return CreateInvalidResponse<MessageResponse>(Resources.PaymentMethod.CodeExists);

            await _commands.CreateAsync(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                PaymentVendorId = request.PaymentVendorId,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut]
        [Description("Update a PaymentMethod")]
        public async Task<MessageResponse> UpdateAsync([FromBody] CreateUpdatePaymentMethodRequest request)
        {
            var method = await _queries.GetSingleAsync(p => p.Id == request.Id);

            if (method == null) return CreateInvalidResponse<MessageResponse>(Resources.PaymentMethod.NotFound);

            method.PaymentVendorId = request.PaymentVendorId;
            method.Description = request.Description;
            method.UpdatedByUsername = Username;
            method.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(method);

            return CreateValidResponse<MessageResponse>();
        }
    }
}