using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Tests.Common;

public static class TestsExtensions
{
    public static async Task<T> ToResponseModel<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content)
               ?? throw new ArgumentException("Response content cannot be null.");
    }

    public static async Task<string> ToResponseModel(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync()
               ?? throw new ArgumentException("Response content cannot be null.");
    }
    
    public static string GenerateMockJwt(string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Iss, "https://test.clerk.dev"),
            new Claim(JwtRegisteredClaimNames.Aud, "http://localhost"),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // Create the token without signing (signature is null)
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: null // No signature
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}