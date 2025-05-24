using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Common.TokenValidator;

public class NoSignatureTokenValidator : ISecurityTokenValidator
{
    private readonly JwtSecurityTokenHandler _innerHandler = new JwtSecurityTokenHandler();

    public bool CanValidateToken => true;
    public int MaximumTokenSizeInBytes { get => _innerHandler.MaximumTokenSizeInBytes; set => _innerHandler.MaximumTokenSizeInBytes = value; }

    public bool CanReadToken(string securityToken) => _innerHandler.CanReadToken(securityToken);

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        validatedToken = new JwtSecurityToken(securityToken);
        var identity = new ClaimsIdentity(((JwtSecurityToken)validatedToken).Claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}