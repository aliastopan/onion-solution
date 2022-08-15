using System.Net;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Authentication
    {
        public static ErrorResult UserNotFound => ErrorResult
            .NotFound(
                HttpStatusCode.Unauthorized,
                "User.NotFound",
                "User does not exist.");

        public static ErrorResult IncorrectPassword => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "Password.Validation",
                "Incorrect password.");
    }
}
