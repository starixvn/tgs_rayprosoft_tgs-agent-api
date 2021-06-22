using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public class HandleAllBetExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        public HandleAllBetExceptionMiddleware(
            Func<string, IErrorMapper> mapper, ITgsLog log)
            : base(mapper, Consts.GameProvider.AllBet, log, "allbet")
        {
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception) => throw new NotImplementedException();
    }
}