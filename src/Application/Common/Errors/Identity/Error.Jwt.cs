using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Jwt
    {
        public static ErrorResult InvalidPrincipal => ErrorResult.Validation("Token.Invalid", "Invalid JWT.");
        public static ErrorResult HasNotYetExpired => ErrorResult.Failure("Jwt.HasNotYetExpired", "JWT has not yet expired.");
        public static ErrorResult HasExpired => ErrorResult.Failure("Jwt.HasExpired", "JWT has expired.");
    }

    public static class RefreshToken
    {
        public static ErrorResult InvalidJwt => ErrorResult.Validation("RefreshToken.InvalidJwt", "Refresh token has invalid Jwt.");
        public static ErrorResult HasExpired => ErrorResult.Failure("RefreshToken.HasExpired", "Refresh token has expired.");
        public static ErrorResult NotFound => ErrorResult.NotFound("RefreshToken.NotFound", "Refresh token not found.");
        public static ErrorResult HasBeenUsed => ErrorResult.Failure("RefreshToken.HasBeenUsed", "Refresh token has been used.");
        public static ErrorResult HasBeenInvalidated => ErrorResult.Validation("RefreshToken.HasBeenInvalidated", "Refresh token has been invalidated.");
    }
}
