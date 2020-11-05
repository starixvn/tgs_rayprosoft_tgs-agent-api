using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.BackendOffice.Filters
{
    public class FluentValidationInterceptor : IValidatorInterceptor
    {
        ValidationResult IValidatorInterceptor.AfterMvcValidation(ControllerContext controllerContext,
            IValidationContext commonContext, ValidationResult result)
        {
            return result;
        }

        IValidationContext IValidatorInterceptor.BeforeMvcValidation(ControllerContext controllerContext,
            IValidationContext commonContext)
        {
            return commonContext;
        }
    }
}