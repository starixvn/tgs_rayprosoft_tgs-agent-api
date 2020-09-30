using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TGS.Apis.OperatorApi.Filters
{
    public sealed class SwaggerHeaderFilter : IOperationFilter
    {
        void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "clientid",
                In = ParameterLocation.Header,
                Required = true,
                Description = "client id of licensee",
                AllowEmptyValue = false,
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "timestamp",
                In = ParameterLocation.Header,
                Required = true,
                Description = "timestamp with timezone when sending api, should be ISO-8601 format: 2014-09-25T07:07:31+00:00",
                AllowEmptyValue = false,
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "authcode",
                In = ParameterLocation.Header,
                Required = true,
                Description = "SHA512 hash value with (clientid + secret + timestamp), clientid and secret are provided by tgs team",
                AllowEmptyValue = false,
            });

        }
    }
}
