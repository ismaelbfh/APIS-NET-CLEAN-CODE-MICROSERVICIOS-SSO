using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GK_API_AUTH.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Concatenamos todos los mensajes de error
                var errores = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                var errorResponse = new
                {
                    message = string.Join("\n", errores),
                    source = context.ActionDescriptor.DisplayName,
                    exceptionType = "ModelValidationException",
                    statusCode = 400
                };

                context.Result = new JsonResult(errorResponse)
                {
                    StatusCode = 400
                };
            }
        }
    }
}
