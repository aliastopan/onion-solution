using System.Net;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Jwt
    {
        public static ErrorResult InvalidPrincipal => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "Token.Invalid",
                "Invalid JWT.");

        public static ErrorResult HasNotYetExpired => ErrorResult
            .Failure(
                HttpStatusCode.NotAcceptable,
                "Jwt.HasNotYetExpired",
                "JWT has not yet expired.");

        public static ErrorResult HasExpired => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "Jwt.HasExpired",
                "JWT has expired.");
    }

    public static class RefreshToken
    {
        public static ErrorResult InvalidJwt => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "RefreshToken.InvalidJwt",
                "Refresh token has invalid Jwt.");

        public static ErrorResult HasExpired => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasExpired",
                "Refresh token has expired.");

        public static ErrorResult NotFound => ErrorResult
            .NotFound(
                HttpStatusCode.Unauthorized,
                "RefreshToken.NotFound",
                "Refresh token not found.");

        public static ErrorResult HasBeenUsed => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasBeenUsed",
                "Refresh token has been used.");

        public static ErrorResult HasBeenInvalidated => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasBeenInvalidated",
                "Refresh token has been invalidated.");
    }
}
