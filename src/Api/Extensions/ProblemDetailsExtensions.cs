using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Onion.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails AddTraceId(this ProblemDetails problemDetails, HttpContext httpContext)
    {
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        return problemDetails;
    }
}
