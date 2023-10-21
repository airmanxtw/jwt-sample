using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using jwt_sample.Api.Interface;
using Microsoft.IdentityModel.Tokens;
using static LanguageExt.Prelude;
namespace jwt_sample.Api;
public class Token : IToken
{
    private readonly string _iss;
    private readonly string _aud;
    private readonly RsaSecurityKey _rsaKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly int _expAdd;

    public Token(string iss, string aud, string privateKey, int expAdd)
    {
        _iss = iss;
        _aud = aud;
        _expAdd = expAdd;
        var rsa = RSA.Create();
        rsa.FromXmlString(privateKey);
        _rsaKey = new RsaSecurityKey(rsa);
        _signingCredentials = new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256);
    }

    private static List<Claim> GetUserClaims(string userid) => new()
    {
        new(ClaimTypes.Name,userid),
        new(ClaimTypes.Role,"USER")
    };

    private JwtSecurityToken GetJwtSecurity(List<Claim> claims) =>
        new(issuer: _iss,
        audience: _aud,
        claims: claims,
        notBefore: DateTime.Now,
        expires: DateTime.Now.AddMinutes(_expAdd),
        signingCredentials: _signingCredentials);

    public string GetToken(string userid) => compose<string, List<Claim>, JwtSecurityToken, string>
    (GetUserClaims, GetJwtSecurity, new JwtSecurityTokenHandler().WriteToken)(userid);

}
