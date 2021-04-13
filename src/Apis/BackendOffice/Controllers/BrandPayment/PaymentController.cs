using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

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
        public BoGenericReponse<IEnumerable<PaymentModel>> GetAllPayments()
        {
            return new BoGenericReponse<IEnumerable<PaymentModel>>
            {
                Valid = true,
                Data = _queries.GetQueries()
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
                    .ToArray()
            };
        }
    }
}
