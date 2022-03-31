using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightsOut.Web
{
    public class GlobalExceptionHandlerMiddleware
    {
        private RequestDelegate Next { get; }
        
        public GlobalExceptionHandlerMiddleware(RequestDelegate next) => Next = next;

        public async Task InvokeAsync(HttpContext context, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            try
            {
                await Next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Unhandled exception was raised");

                var problem = new ProblemDetails
                {
                    Title = "Unhandled exception",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = $"Issue occurred and was logged with traceId: {context.TraceIdentifier}"
                };

                context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
            }
        }
    }
}