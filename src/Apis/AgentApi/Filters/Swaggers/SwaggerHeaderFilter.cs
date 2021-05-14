using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TGS.Apis.AgentApi.Attributes;

namespace TGS.Apis.AgentApi.Filters.Swaggers
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
                    Reference = new()
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme
                    }
                };

                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new() {{securityScheme, Array.Empty<string>()}});
            }

            var attribute = (AuthorizeAgentTokenAttribute)meta
                .FirstOrDefault(o => o is AuthorizeAgentTokenAttribute);

            if (attribute != null)
            {
                operation.Parameters.Add(new()
                {
                    Name = "Token",
                    In = ParameterLocation.Header,
                    Required = !attribute.AllowNullOrEmpty,
                    Description = "agent token, provided when agent login successfully",
                    AllowEmptyValue = attribute.AllowNullOrEmpty
                });
            }
        }
    }
}