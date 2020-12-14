using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseContextController
    {
        private readonly ITgsCommands<Payment> _commands;
        private readonly ITgsQueries<Payment> _queries;

        public PaymentController(
            ITgsQueries<Payment> queries,
            ITgsCommands<Payment> commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get Payments")]
        public Task<PagedResponse<PaymentModel>> GetPaymentsAsync([FromQuery] PaymentsRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Code, q => q.Code == request.Code)
                .Select(q => new PaymentModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Code = q.Code,
                    Description = q.Description,
                    CreatedByUsername = q.CreatedByUsername,
                    UpdatedByUsername = q.UpdatedByUsername,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Payment")]
        public async Task<GetPaymentResponse> GetPaymentAsync(Guid id)
        {
            var vendor = await _queries.GetSingleAsync(q => q.Id == id, q =>
                q.Include(i => i.PaymentMethods));

            return new GetPaymentResponse
            {
                Valid = true,
                PaymentVendor = new PaymentModel
                {
                    Id = vendor.Id,
                    Name = vendor.Name,
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
        [Description("Create a Payment")]
        public async Task<MessageResponse> CreateAsync(CreatePaymentRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Code == request.Code))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Payment.CodeExists);
            }

            await _commands.CreateAsync(new Payment
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a Payment")]
        public async Task<MessageResponse> UpdateAsync(Guid id, UpdatePaymentRequest request)
        {
            var vendor = await _queries.GetSingleAsync(p => p.Id == id);

            if (vendor == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Payment.NotFound);
            }

            if (vendor.Code == request.Code)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Payment.CodeExists);
            }

            vendor.Name = request.Name;
            vendor.Description = request.Description;
            vendor.UpdatedByUsername = Username;
            vendor.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(vendor);

            return CreateValidResponse<MessageResponse>();
        }
    }
}