using System.Diagnostics;
using HotChocolate.Execution;
using Microsoft.Extensions.Logging;
using RequestDelegate = HotChocolate.Execution.RequestDelegate;

namespace AbsoluteCinema.WebAPI.GraphQL.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask InvokeAsync(IRequestContext context)
        {
            var start = Stopwatch.GetTimestamp();
            try
            {
                await _next(context);
            }
            finally
            {
                var elapsed = Stopwatch.GetElapsedTime(start);

                if (context.Operation is not null)
                {
                    _logger.LogInformation(
                        "GraphQL operation '{OperationName}' completed in {ElapsedMilliseconds}ms",
                        context.Operation.Name ?? "anonymous",
                        elapsed.TotalMilliseconds);
                }
            }
        }
    }
}
