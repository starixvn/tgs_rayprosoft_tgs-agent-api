using Microsoft.AspNetCore.Builder;
using TGS.Apis.OperatorApi.Middlewares;

namespace TGS.Apis.OperatorApi.Pipelines
{
    public sealed class VerifyCredentialsPipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<VerifyCredentialsMiddleware>();
        }
    }
}
