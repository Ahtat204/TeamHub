using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
      logger.LogError(exception, "Error:{Message}",exception.Message);
      if (exception is NotFoundException<User>)
      {
          var problem = new ProblemDetails
          {
              Status = StatusCodes.Status404NotFound,
              Title = "the user does not exist",
          };
          httpContext.Response.StatusCode = problem.Status.Value;
          httpContext.Response.ContentType = "application/problem+json";
          await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
          return true;
      }
     
      return true;
    }
}