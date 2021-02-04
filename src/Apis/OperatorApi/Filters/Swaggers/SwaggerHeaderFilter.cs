using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TGS.Apis.OperatorApi.Attributes;

namespace TGS.Apis.OperatorApi.Filters.Swaggers
{
    public sealed class SwaggerHeaderFilter : IOperationFilter
    {
        void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            var meta = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (meta.All(m => m.GetType() != typeof(AllowAnonymousAttribute)))
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Api Authorization",
                    Description = "authorized jwt token, get from api/auth/jwt",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {securityScheme, Array.Empty<string>()}
                });
            }

            var attribute = (AuthorizePlayerTokenAttribute) meta
                .FirstOrDefault(o => o is AuthorizePlayerTokenAttribute);

            if (attribute != null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Token",
                    In = ParameterLocation.Header,
                    Required = !attribute.AllowNullOrEmpty,
                    Description = "player token, provided when player login successfully",
                    AllowEmptyValue = attribute.AllowNullOrEmpty
                });
            }
        }
    }
}
