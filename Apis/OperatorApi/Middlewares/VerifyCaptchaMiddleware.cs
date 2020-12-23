using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyCaptchaMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyCaptchaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICaptchaService captchaService)
        {
            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<VerifyCaptchaAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var request = await context.Request.ReadAsObjectFromJsonAsync<BaseUserRequest>();
            var bypassCaptcha = request.BypassCaptcha ?? false;

            if (!bypassCaptcha)
            {
                await captchaService.VerifyAsync(request.Answer, request.Captcha);
            }

            await _next(context);
        }
    }
}