using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentMethod;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
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
        public BoGenericReponse<IEnumerable<PaymentMethodModel>> GetAllPaymentMethods()
        {
            return new BoGenericReponse<IEnumerable<PaymentMethodModel>>
            {
                Valid = true,
                Data = _queries.GetQueries()
                    .Select(q => new PaymentMethodModel
                    {
                        Id = q.Id,
                        Code = q.Code
                    })
                    .AsNoTracking()
                    .ToArray()
            };
        }

    }
}