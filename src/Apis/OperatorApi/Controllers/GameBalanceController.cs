using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Services.Operator;
using TGS.Services.Operator.GameBalance;
using TGS.ServiceUtility;
using PaymentData = TGS.Core.DataServices.Tgs.Models.Payment;


namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/game/balance")]
    [ApiController]
    public class GameBalanceController : ControllerBase
    {
        private readonly ITgsCommands<PaymentData> _commands;
        private readonly ITgsQueries<PaymentData> _queries;

        public GameBalanceController(
            ITgsQueries<PaymentData> queries,
            ITgsCommands<PaymentData> commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [Route("getgamebalance")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task GetGameBalanceAsync(
            [FromServices] IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,string gpCode)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var query = new GetGameBalanceQuery(playerTokenData.PlayerCode, gpCode);



            //return new GenericResponse<IEnumerable<PaymentModel>>
            //{
            //    Valid = true,
            //    Data = _queries.GetQueries()
            //        .Select(q => new PaymentModel
            //        {
            //            Id = q.Id,
            //            Name = q.Name,
            //            Code = q.Code,
            //            Description = q.Description,
            //            CreatedByUsername = q.CreatedByUsername,
            //            UpdatedByUsername = q.UpdatedByUsername,
            //            CreatedOn = q.CreatedOn,
            //            UpdatedOn = q.UpdatedOn
            //        })
            //        .AsNoTracking()
            //        .ToArray()
            //};
        }
    }
}
