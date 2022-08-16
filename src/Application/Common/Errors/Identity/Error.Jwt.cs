using System.Net;
using AssertiveResults.Errors;
using ErrorResult = AssertiveResults.Errors.Error;

namespace Onion.Application.Common.Errors.Identity;

public static partial class Error
{
    public static class Jwt
    {
        public static IError InvalidPrincipal => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "Token.Invalid",
                "Invalid JWT.");

        public static IError HasNotYetExpired => ErrorResult
            .Failure(
                HttpStatusCode.NotAcceptable,
                "Jwt.HasNotYetExpired",
                "JWT has not yet expired.");

        public static IError HasExpired => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "Jwt.HasExpired",
                "JWT has expired.");
    }

    public static class RefreshToken
    {
        public static IError InvalidJwt => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "RefreshToken.InvalidJwt",
                "Refresh token has invalid Jwt.");

        public static IError HasExpired => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasExpired",
                "Refresh token has expired.");

        public static IError NotFound => ErrorResult
            .NotFound(
                HttpStatusCode.Unauthorized,
                "RefreshToken.NotFound",
                "Refresh token not found.");

        public static IError HasBeenUsed => ErrorResult
            .Failure(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasBeenUsed",
                "Refresh token has been used.");

        public static IError HasBeenInvalidated => ErrorResult
            .Validation(
                HttpStatusCode.Unauthorized,
                "RefreshToken.HasBeenInvalidated",
                "Refresh token has been invalidated.");
    }
}
