using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
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

                    log.LogError($"Catched globla error on :{context.Request.Host} with message: {message}", exception);

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
    }
}