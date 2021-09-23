using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Core.Models.Messages;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Global Exception Handler Middleware, needs to add the following dependencies beforehand:
        /// 1. AddTgsLog() in Infrastructure.Log
        /// 2. AddErrorMappers() in Infrastructure.ErrorMapper
        /// </summary>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionFeature?.Error;
                    if (exception == null)
                        return;

                    using var scope = app.ApplicationServices.CreateScope();
                    var serviceProvider = scope.ServiceProvider;
                    var log = serviceProvider.GetRequiredService<ITgsLog>();
                    var mapper = serviceProvider.GetRequiredService<Func<string, IErrorMapper>>();
                    var errorMapper = mapper(Consts.Default);
                    var error = errorMapper.GetApiErrorByException(exception);
                    var statusCode = errorMapper.GetHttpStatusByException(exception);
                    var message = exception.GetMessageChain();

                    log.LogError($"Caught global error on :{context.Request.Host} with message: {message}", exception);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = statusCode;

                    await context.Response.WriteAsJsonAsync(new TgsResponse
                    {
                        Error = error,
                        ErrorMessage = message
                    });
                }
            });

            return app;
        }

        public static IApplicationBuilder UseTgsErrorHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(
                new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = exceptionFeature?.Error;
                        if (exception == null)
                            return;

                        var log = context.RequestServices.GetRequiredService<ITgsLog>();
                        var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandlerOptions>>();
                        var errorMapper = context.RequestServices.GetRequiredService<IErrorMapper>();
                        var error = errorMapper.GetApiErrorByException(exception);
                        var statusCode = errorMapper.GetHttpStatusByException(exception);
                        var message = exception.GetMessageChain();
                        var messageKey = errorMapper.GetMessageKeyByException(exception);

                        if (statusCode >= 500)
                        {
                            logger.LogError(exception, "Caught global error!");
                            log.LogError(
                                $"Caught global error on :{context.Request.Host} " +
                                $"with message: {message}", exception);
                        }
                        else
                            logger.LogWarning(exception, "Caught global exception!");

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsJsonAsync(new TgsResponse
                        {
                            Error = error,
                            ErrorMessage = message,
                            ErrorMessageKey = messageKey
                        });
                    }
                });
        }
    }
}
