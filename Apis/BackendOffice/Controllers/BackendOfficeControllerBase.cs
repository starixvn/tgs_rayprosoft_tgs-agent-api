using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TGS.Apis.BackendOffice.Controllers.Messages;

namespace TGS.Apis.TGS.Apis.BackendOffice.Controllers
{
    public class BackendOfficeControllerBase : ControllerBase
    {
        protected string MsHttpContext = "MS_HttpContext";

        protected static TResponse CreateValidResponse<TResponse>() where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = true
            };
        }

        protected static TResponse CreateInvalidResponse<TResponse>(string message) where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = false,
                Message = message
            };
        }
    }
}
