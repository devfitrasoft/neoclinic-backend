using Microsoft.IdentityModel.Tokens;
using Shared.Entities.Objs.Enterprise;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace neo.admin.Services.Factories
{
    public interface IJwtProvider
    {
        string GenerateToken(Login login);
    }

    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _cfg;

        public JwtProvider(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public string GenerateToken(Login login)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Username),
                new Claim("loginId", login.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["JwtToken:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _cfg["JwtToken:Issuer"],
                audience: _cfg["JwtToken:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_cfg.GetValue<int>("JwtToken:AccessExpiry")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
