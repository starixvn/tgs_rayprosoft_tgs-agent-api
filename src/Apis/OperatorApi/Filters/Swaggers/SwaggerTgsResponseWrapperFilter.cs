using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models;

namespace TGS.Apis.OperatorApi.Filters.Swaggers
{
    public class SwaggerTgsResponseWrapperFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var responseAdjustments = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<TgsResponseWrapperAttribute>();

            if (responseAdjustments.Any())
            {
                foreach (var response in operation.Responses)
                {
                    var resType = context.ApiDescription.SupportedResponseTypes
                        .Where(r => r.StatusCode.ToString() == response.Key)
                        .FirstOrDefault();

                    foreach (var content in response.Value.Content)
                    {
                        if (resType != null)
                        {
                            var type = typeof(SuccessResponse<>).MakeGenericType(resType.Type);
                            var schema = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);

                            content.Value.Schema = schema;
                        }

                    }
                }
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
        }
    }
}
