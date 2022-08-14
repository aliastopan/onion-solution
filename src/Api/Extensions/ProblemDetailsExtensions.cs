using System.Diagnostics;

namespace Onion.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(
        this AssertiveResults.IResult result,
        HttpContext httpContext,
        int statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Title = result.FirstError.Code,
            Status = statusCode,
            Detail = result.FirstError.Description,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        return problemDetails;
    }
}
