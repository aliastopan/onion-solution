using System.Net;
using AssertiveResults.Errors;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Authentication
    {
        public static IError UserNotFound => ErrorResult
            .NotFound(
                HttpStatusCode.Unauthorized,
                "User.NotFound",
                "User does not exist.");

        public static IError IncorrectPassword => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "Password.Validation",
                "Incorrect password.");
    }
}
