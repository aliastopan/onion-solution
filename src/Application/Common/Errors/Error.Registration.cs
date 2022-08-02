using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors;

public static partial class Error
{
    public static class Registration
    {
        public static ErrorResult UsernameTaken => ErrorResult
            .Conflict("Username.Taken", "Username is already taken.");

        public static ErrorResult EmailInUse => ErrorResult
            .Conflict("Email.InUse", "Email is already in use.");
    }
}
