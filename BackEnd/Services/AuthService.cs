using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _config;

        public AuthService(
            IRepository<User> userRepository,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            List<User> users = await _userRepository.GetAllAsync();

            User? user = users.FirstOrDefault(u =>
                !string.IsNullOrWhiteSpace(u.Username) &&
                u.Username.Equals(loginRequest.Username?.Trim(), StringComparison.OrdinalIgnoreCase)
            );

            if (user == null) return null;

            if (user.Password != loginRequest.Password) return null;


            Claim[] claims = new Claim[]
            {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("userName", user.Username),
                    new Claim("fullName", user.FullName),
                    new Claim("role", user.Role.ToString()),
            };

            // Set Credential
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double expireMinutes = double.TryParse(_config["Jwt:ExpireMinutes"], out var parsedMinutes)
                ? parsedMinutes
                : 60;

            // Create Token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new LoginResponse
            {
                Status = "success",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = new UserSummary
                {
                    Id = user.Id,
                    UserName = user.Username,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                }
            };
        }
    }
}