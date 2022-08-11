using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Jwt
    {
        public static ErrorResult InvalidPrincipal => ErrorResult.Validation("Token.Invalid", "Invalid JWT.");
        public static ErrorResult HasNotExpiredYet => ErrorResult.Failure("Token.Invalid", "JWT has not expired yet.");
        public static ErrorResult HasExpired => ErrorResult.Failure("Token.Invalid", "JWT has expired.");
    }

    public static class RefreshToken
    {
        public static ErrorResult Invalid => ErrorResult.Validation("Token.Invalid", "Refresh token is invalid.");
        public static ErrorResult HasExpired => ErrorResult.Failure("Token.Invalid", "Refresh token has expired.");
        public static ErrorResult NotFound => ErrorResult.NotFound("Token.Invalid", "Refresh token not found.");
        public static ErrorResult IsUsed => ErrorResult.Failure("Token.Invalid", "Refresh token has been used.");
        public static ErrorResult IsInvalidated => ErrorResult.Validation("Token.Invalid", "Refresh token has been invalidated.");
    }
}
