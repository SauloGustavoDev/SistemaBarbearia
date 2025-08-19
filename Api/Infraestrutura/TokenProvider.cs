using Api.Models.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Infraestrutura
{
    public class TokenProvider(IConfiguration configuration)
    {
        public string CreateToken(Barbeiro barbeiro)
        {
            string secretKey = configuration.GetSection("Jwt:Secret").Value;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                    new Claim("Id", barbeiro.Id.ToString()),
                    new Claim("Numero", barbeiro.Numero),
                    new Claim("Acesso", barbeiro.Acesso.ToString())
                    ]),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = credential,
                Issuer = configuration.GetSection("Jwt:Issuer").Value,
                Audience = configuration.GetSection("Jwt:Audience").Value
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
