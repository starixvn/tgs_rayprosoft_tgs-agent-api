using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
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
        public IAsyncEnumerable<PaymentModel> GetAllPaymentsAsync()
        {
            return _queries.GetQueries()
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
                })
                .AsNoTracking()
                .ToAsyncEnumerable();
        }
    }
}