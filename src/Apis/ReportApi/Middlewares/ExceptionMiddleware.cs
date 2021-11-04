using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using Tgs.Logging;

namespace TGS.Apis.ReportApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private static readonly string[] _MethodNotifies =
        {
            "CreateAgentDividendsAsync",
            "CreatePlayerAggregateAsync",
            "CreatePlayerRebatesAsync",
            "CreateRoundAggregateAsync"
        };

        private readonly RequestDelegate _next;
        private readonly ITelegramService _telegramService;

        public ExceptionMiddleware(RequestDelegate next, ITelegramService telegramService)
        {
            _next = next;
            _telegramService = telegramService;
        }

        public async Task InvokeAsync(HttpContext context, ITgsLog log)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var endpoint = context.GetEndpoint();
                var name = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>()?.MethodInfo.Name;

                var message = exception.GetMessageChain();
                log.SetLogger(name);
                log.LogError($"Report api error on :{context.Request.Host} with message: {message}", exception);

                if (_MethodNotifies.Contains(name))
                {
                    await _telegramService.SendMessage(message);
                }

                await context.Response.WriteAsJsonAsync(new MessageResponse
                {
                    Message = message,
                    Valid = false
                });
            }
        }
    }
}