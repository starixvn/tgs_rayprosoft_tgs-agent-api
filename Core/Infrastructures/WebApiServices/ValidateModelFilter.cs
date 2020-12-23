using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public sealed class ValidateModelFilter : IAsyncActionFilter
    {
        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next();
                return;
            }

            var errors = context.ModelState.Values.SelectMany(mse => mse.Errors.Select(e =>
                e.ErrorMessage.IsNullOrEmpty() ? e.Exception?.Message ?? string.Empty : e.ErrorMessage))
                .JoinAsStringBySemicolon();

            throw new ModelBindingException($"Error binding model: {errors}");
        }
    }
}