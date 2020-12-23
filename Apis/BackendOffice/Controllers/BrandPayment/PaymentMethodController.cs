using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : BaseContextController
    {
        private readonly ITgsCommands<PaymentMethod> _commands;
        private readonly ITgsQueries<PaymentMethod> _queries;

        public PaymentMethodController(
            ITgsQueries<PaymentMethod> queries,
            ITgsCommands<PaymentMethod> commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get PaymentMethods")]
        public Task<PagedResponse<PaymentMethodModel>> GetPaymentMethodsAsync([FromQuery] PaymentMethodsRequest request)
        {
            return _queries.GetQueries()
                .Include(p => p.Payment)
                .Where(request.Code, q => q.Code == request.Code)
                .Select(q => new PaymentMethodModel
                {
                    Id = q.Id,
                    Name = q.Name,
                    Code = q.Code,
                    Description = q.Description,
                    PaymentName = q.Payment.Name,
                    CreatedByUsername = q.CreatedByUsername,
                    UpdatedByUsername = q.UpdatedByUsername,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn
                }).AsNoTracking().CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a PaymentMethod")]
        public async Task<GetPaymentMethodResponse> GetPaymentMethodAsync(Guid id)
        {
            var method = await _queries.GetSingleAsync(q => q.Id == id,
                p => p
                    .Include(pm => pm.Payment)
                    .AsNoTracking());

            return new GetPaymentMethodResponse
            {
                Valid = true,
                PaymentMethod = new PaymentMethodModel
                {
                    Id = method.Id,
                    Name = method.Name,
                    Code = method.Code,
                    Description = method.Description,
                    PaymentName = method.Payment.Name,
                    CreatedOn = method.CreatedOn,
                    UpdatedOn = method.UpdatedOn,
                    CreatedByUsername = method.CreatedByUsername,
                    UpdatedByUsername = method.UpdatedByUsername
                }
            };
        }

        [HttpPost]
        [Description("Create a PaymentMethod")]
        public async Task<MessageResponse> CreateAsync(CreatePaymentMethodRequest request)
        {
            if (await _queries.ExistsAsync(p => p.Code == request.Code))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.PaymentMethod.CodeExists);
            }

            await _commands.CreateAsync(new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                PaymentId = request.PaymentId,
                CreatedByUsername = Username,
                UpdatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a PaymentMethod")]
        public async Task<MessageResponse> UpdateAsync(Guid id, UpdatePaymentMethodRequest request)
        {
            var method = await _queries.GetSingleAsync(p => p.Id == id);

            if (method == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.PaymentMethod.NotFound);
            }

            method.Name = request.Name;
            method.PaymentId = request.PaymentId;
            method.Description = request.Description;
            method.UpdatedByUsername = Username;
            method.UpdatedOn = DateTimeOffset.Now;

            await _commands.UpdateAsync(method);

            return CreateValidResponse<MessageResponse>();
        }
    }
}