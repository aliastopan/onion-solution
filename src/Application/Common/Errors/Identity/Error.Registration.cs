using System.Net;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Registration
    {
        public static ErrorResult UsernameTaken => ErrorResult
            .Conflict(
                HttpStatusCode.UnprocessableEntity,
                "Username.Taken",
                "Username is already taken.");

        public static ErrorResult EmailInUse => ErrorResult
            .Conflict(
                HttpStatusCode.UnprocessableEntity,
                "Email.InUse",
                "Email is already in use.");
    }
}
