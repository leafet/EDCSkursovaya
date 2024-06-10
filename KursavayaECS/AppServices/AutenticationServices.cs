using KursavayaECS.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace KursavayaECS.AppServices
{
    public interface IAuthenticationServices
    {
        public string GenerateHash(string password);
        public bool Verify(string password, string hashedPassword);
        public string GenerateToken(AppUser user, string policy);
        public Guid GetUserIdFromToken(HttpContext Hctx);
    }

    public class AutenticationServices : IAuthenticationServices
    {
        public string GenerateHash(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool Verify(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);

        public string GenerateToken(AppUser user, string policy)
        {
            Claim[] claims = [new("userId", user.ID.ToString())];

            switch(policy)
            {
                case "Admin":
                    claims =
                    [
                    new("userId", user.ID.ToString()),
                    new("Admin", "true")
                    ];
                break;
                case "Default":
                    claims =
                    [
                    new("userId", user.ID.ToString()),
                    new("Default", "true")
                    ];
                    break;
                case "Student":
                    claims =
                    [
                    new("userId", user.ID.ToString()),
                    new("Student", "true")
                    ];
                    break;
                case "Teacher":
                    claims =
                    [
                    new("userId", user.ID.ToString()),
                    new("Teacher", "true")
                    ];
                break;
            }
                

            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SomeSecretSheeshAndShushAndShashAndManyOtherSecretThingsAlreadyToComplicated")),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signinCredentials,
                expires: DateTime.UtcNow.AddHours(12));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }

        public Guid GetUserIdFromToken(HttpContext Hctx)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var SecretKey = "SomeSecretSheeshAndShushAndShashAndManyOtherSecretThingsAlreadyToComplicated";
            var key = Encoding.UTF8.GetBytes(SecretKey);
            string token;
            Hctx.Request.Cookies.TryGetValue("tasty-kokis", out token);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return Guid.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);
        }
    }
}
