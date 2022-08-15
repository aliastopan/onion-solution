using System.Net;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class User
    {
        public static ErrorResult NotFound => ErrorResult
            .NotFound(
                HttpStatusCode.NotFound,
                "User.NotFound",
                "User not found.");
    }
}
