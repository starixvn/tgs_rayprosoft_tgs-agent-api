using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    public class BaseMessageController : ControllerBase
    {
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
