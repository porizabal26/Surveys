using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Surveys.Context;
using Surveys.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Surveys.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        public LoginController(IConfiguration configuration, AppDbContext context)
        {
            _config = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public dynamic Login([FromBody] Object loginData)
        {
            var jsonData = JsonConvert.DeserializeObject<dynamic>(loginData.ToString());

            if (jsonData.email == null || jsonData.password == null)
            {
                return BadRequest("Credenciales no ingresadas o incompletas");
            }

            string email = jsonData.email.ToString();
            string password = jsonData.password.ToString();

            Usr user = _context.Usr.Where(u => u.Email == email && u.Pass == password).FirstOrDefault();

            if(user == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            var jwt = _config.GetSection("Jwt").Get<Jwt>();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signIn
            );

            return new
            {
                success = true,
                message = "Inicio de sesión satisfactorio",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
