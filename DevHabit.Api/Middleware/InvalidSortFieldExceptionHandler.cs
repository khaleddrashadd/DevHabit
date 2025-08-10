using DevHabit.Api.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Middleware;

public sealed class InvalidSortFieldExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler

{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not InvalidSortFieldException invalidSortFieldException) return false;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext, Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Invalid Sort Field",
                Status = StatusCodes.Status400BadRequest,
                Detail = invalidSortFieldException.Message
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}