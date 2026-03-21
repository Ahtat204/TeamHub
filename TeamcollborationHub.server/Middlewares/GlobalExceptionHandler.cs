using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Error:{Message}", exception.Message);
        switch (exception)
        {
            case ValueNotFoundException:
                var issue = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "value was not found",
                };
                await Console.Error.WriteLineAsync(issue.Status + issue.Title + issue.Detail);
                return true;
            case NotFoundException<User>:
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "the user does not exist",
                };
                httpContext.Response.StatusCode = problem.Status.Value;
                httpContext.Response.ContentType = "application/problem+json";
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            case AlreadyExistsException<string>:
                var prob = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "value already exists",
                    Detail = "The value already exists.Try using different value."
                };
                httpContext.Response.StatusCode = prob.Status.Value;
                httpContext.Response.ContentType = "application/problem+json";
                await httpContext.Response.WriteAsJsonAsync(prob, cancellationToken);
                return true;
            case NotFoundException<Project>:
                var probl = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "the project does not exist",
                    Detail = "The project does not exist."
                };
                httpContext.Response.StatusCode = probl.Status.Value;
                httpContext.Response.ContentType = "application/problem+json";
                await httpContext.Response.WriteAsJsonAsync(probl, cancellationToken);
                return true;

        }

        return true;
    }
}