using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors;

public static partial class Error
{
    public static class Authentication
    {
        public static ErrorResult UserNotFound => ErrorResult
            .NotFound("User.NotFound", "User does not exist.");

        public static ErrorResult IncorrectPassword => ErrorResult
            .Validation("Password.Validation", "Incorrect password.");
    }
}
