using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TGS.Apis.AgentApi.Attributes;

namespace TGS.Apis.AgentApi.Filters.Swaggers;

public sealed class SwaggerHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        var meta = context.ApiDescription.ActionDescriptor.EndpointMetadata;

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
