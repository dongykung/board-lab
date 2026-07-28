using Microsoft.AspNetCore.Diagnostics;

namespace BoardApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailService;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment env
    )
    {
        _logger = logger;
        _problemDetailService = problemDetailsService;
        _env = env;
    } 

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception on {Path} -> {StatusCode} {Title}",
                httpContext.Request.Path, statusCode, title);
        }
        else
        {
            _logger.LogWarning("Request failed on {Path} -> {StatusCode} {Title}: {Message}",
                httpContext.Request.Path, statusCode, title, exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;
        return await _problemDetailService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new()
            {
                Status = statusCode,
                Title = title,
                Detail = exception is BoardApiException || _env.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred."
            }
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        BoardApiException ex          => (ex.StatusCode, ex.Title),
        OperationCanceledException    => (499, "Client Closed Request"),
        _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
    };
}