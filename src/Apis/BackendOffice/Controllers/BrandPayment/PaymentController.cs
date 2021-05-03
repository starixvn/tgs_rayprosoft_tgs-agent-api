using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices.Base.Interfaces;
using PaymentData = TGS.Core.DataServices.Tgs.Models.Payment;

namespace TGS.Apis.BackendOffice.Controllers.BrandPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseContextController
    {
        private readonly ITgsCommands<PaymentData> _commands;
        private readonly ITgsQueries<PaymentData> _queries;

        public PaymentController(
            ITgsQueries<PaymentData> queries,
            ITgsCommands<PaymentData> commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Description("Get Payments")]
        public GenericResponse<IEnumerable<PaymentModel>> GetAllPayments()
        {
            return new GenericResponse<IEnumerable<PaymentModel>>
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
