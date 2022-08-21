using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AssertiveResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Errors.Identity;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Entities.Identity;

namespace Onion.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtValidator _jwtValidator;
    private readonly IDateTime _dateTime;
    private readonly IDbContext _dbContext;

    public JwtProvider(
        IOptions<JwtSettings> jwtSettings,
        JwtValidator jwtValidator,
        IDateTime dateTime,
        IDbContext dbContext)
    {
        _jwtSettings = jwtSettings.Value;
        _jwtValidator = jwtValidator;
        _dateTime = dateTime;
        _dbContext = dbContext;
    }

    public string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Sub, user.Id.ToString()),
            new Claim(JwtClaimTypes.UniqueName, user.Username),
            new Claim(JwtClaimTypes.Role, user.Role.Name),
        };

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
            claims: claims,
            signingCredentials: signingCredentials);

        return jwtHandler.WriteToken(jwt);
    }

    public string GenerateJwt(ClaimsPrincipal principal, out User user)
    {
        var sub = principal.Claims.Single(x => x.Type == JwtClaimTypes.Sub).Value;
        var userId = Guid.Parse(sub);
        user = _dbContext.Users.Find(userId)!;
        return GenerateJwt(user);
    }

    public RefreshToken GenerateRefreshToken(string jwt, User user)
    {
        var principal = GetPrincipalFromToken(jwt);
        var jti = principal.Claims.Single(x => x.Type == JwtClaimTypes.Jti).Value;
        var refreshToken = new RefreshToken
        {
            Token = CCred.Sauce.GenerateGibberish(256, "-._"),
            JwtId = jti,
            CreationDate = _dateTime.UtcNow,
            ExpiryDate = _dateTime.UtcNow.Add(_jwtSettings.RefreshLifeTime),
            User = user
        };
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.Commit();
        return refreshToken;
    }

    public IResult<(string jwt, string refreshToken)> Refresh(string jwt, string refreshToken)
    {
        var principal = GetPrincipalFromToken(jwt);
        var step1 = ValidatePrincipal(principal);
        var step2 = ValidateJwt(step1, principal);
        var step3 = ValidateRefreshToken(step2, principal, refreshToken);
        var step4 = step3.Override<(string jwt, string refreshToken)>();
        var final = step4.Resolve(_ =>
        {
            jwt = GenerateJwt(principal, out var user);
            refreshToken = GenerateRefreshToken(jwt, user).Token;
            return (jwt, refreshToken);
        });
        return final;
    }

    // STEP 1
    private ISubject ValidatePrincipal(ClaimsPrincipal principal)
    {
        return Assertive.Result().Assert(ctx =>
        {
            ctx.Should.NotNull(principal).WithError(Error.Jwt.InvalidPrincipal);
        })
        .Assert(ctx =>
        {
            var sub = principal.Claims.Single(x => x.Type == JwtClaimTypes.Sub).Value;
            var any = _dbContext.Users.Any(x => x.Id == Guid.Parse(sub));
            ctx.Should.Satisfy(any).WithError(Error.Jwt.InvalidPrincipal);
        });
    }

    // STEP 2
    private ISubject ValidateJwt(ISubject subject, ClaimsPrincipal principal)
    {
        return subject.Assert(ctx =>
        {
            var expiry = principal.Claims.Single(x => x.Type == JwtClaimTypes.Exp).Value;
            var expiryDateUnix = long.Parse(expiry);
            var expiryDateUtc = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);
            ctx.Should.NotSatisfy(expiryDateUtc > _dateTime.UtcNow).WithError(Error.Jwt.HasNotYetExpired);
        });
    }

    // STEP 3
    private ISubject ValidateRefreshToken(ISubject subject, ClaimsPrincipal principal, string token)
    {
        RefreshToken refreshToken = null!;
        subject.Assert(ctx =>
        {
            refreshToken = _dbContext.RefreshTokens.SingleOrDefault(x => x.Token == token)!;
            ctx.Should.NotNull(refreshToken).WithError(Error.RefreshToken.NotFound);
        })
        .Assert(ctx =>
        {
            ctx.Should.NotSatisfy(refreshToken.ExpiryDate < _dateTime.UtcNow).WithError(Error.RefreshToken.HasExpired);
            ctx.Should.NotSatisfy(refreshToken.IsInvalidated).WithError(Error.RefreshToken.HasBeenInvalidated);
            ctx.Should.NotSatisfy(refreshToken.IsUsed).WithError(Error.RefreshToken.HasBeenUsed);
        })
        .Assert(ctx =>
        {
            var jti = principal.Claims.Single(x => x.Type == JwtClaimTypes.Jti).Value;
            ctx.Should.Equal(refreshToken.JwtId, jti).WithError(Error.RefreshToken.InvalidJwt);
        });

        return subject.Resolve(_ =>
        {
            MarkRefreshTokenAsUsed(refreshToken);
        });
    }

    private ClaimsPrincipal GetPrincipalFromToken(string jwt)
    {
        try
        {
            var validationParameters = _jwtValidator.RefreshValidationParameters();
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(jwt, validationParameters, out var securityToken);
            if(!HasValidSecurityAlgorithm(securityToken))
            {
                return null!;
            }

            return principal;
        }
        catch
        {
            return null!;
        }
    }

    private static bool HasValidSecurityAlgorithm(SecurityToken securityToken)
    {
        var securityAlgorithm = SecurityAlgorithms.HmacSha384;
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        return jwtSecurityToken is not null && jwtSecurityToken!.Header.Alg
            .Equals(securityAlgorithm, StringComparison.InvariantCultureIgnoreCase);
    }

    private void MarkRefreshTokenAsUsed(RefreshToken refreshToken)
    {
        refreshToken.IsUsed = true;
        _dbContext.RefreshTokens.Update(refreshToken);
        _dbContext.Commit();
    }
}
