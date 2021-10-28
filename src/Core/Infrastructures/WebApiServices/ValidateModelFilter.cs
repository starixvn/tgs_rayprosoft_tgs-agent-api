using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public sealed class ValidateModelFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
                return next();

            string GetErrorMessage(ModelError err)
                => err.ErrorMessage.IsNullOrEmpty()
                    ? err.Exception?.Message ?? string.Empty
                    : err.ErrorMessage;

            var errors = context.ModelState
                .SelectMany(p => p.Value.Errors
                    .Select(e => $"{p.Key} => {GetErrorMessage(e)}"))
                .JoinAsStringBySemicolon();

            throw new ModelBindingException(
                $"{context.HttpContext.Request.Path} Error binding model: {errors}");
        }
    }
}
