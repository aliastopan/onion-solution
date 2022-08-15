using System.Diagnostics;

namespace Onion.Api.Extensions;

public static class AssertiveResultsExtensions
{
    public static ProblemDetails ToProblemDetails(this AssertiveResults.IResult result,
        HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Title = result.FirstError.Title,
            Status = int.Parse(result.FirstError.Status),
            Detail = result.FirstError.Detail,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        return problemDetails;
    }
}
