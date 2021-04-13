using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

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
        public BoGenericReponse<IEnumerable<PaymentChannelModel>> GetAllPaymentChannels()
        {
            return new BoGenericReponse<IEnumerable<PaymentChannelModel>>
            {
                Valid = true,
                Data = _queries.GetQueries()
                    .Select(q => new PaymentChannelModel
                    {
                        Id = q.Id,
                        Code = q.Code,
                    })
                    .AsNoTracking()
                    .ToArray()
            };
        }
    }
}
