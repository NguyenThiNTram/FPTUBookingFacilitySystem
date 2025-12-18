using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FPTUBookingFacilitySystem.Repositories.Common;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;
using FPTUBookingFacilitySystem.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FPTUBookingFacilitySystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IAccountRepository _accountRepository;

        public AuthService(IConfiguration config, IAccountRepository accountRepository)
        {
            _config = config;
            _accountRepository = accountRepository;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Find account by email
            var account = await _accountRepository.GetAccountByEmailAsync(request.Email);
            
            if (account == null)
                return null;

            // Check if account is active
            if (!account.IsActive)
                return null;

            if (account.Password != request.Password)
                return null;

            // Map RoleId to UserRole enum
            UserRole role = (UserRole)account.RoleId;

            // Generate JWT token
            var token = GenerateToken(account.AccountId, account.Email, role);

            return new LoginResponse
            {
                Token = token,
                UserId = account.AccountId,
                Email = account.Email,
                Role = role.ToString()
            };
        }

        public async Task<bool> LogoutAsync(string token)
        {           
            await Task.CompletedTask;
            return true;
        }

        public string GenerateToken(int userId, string email, UserRole role)
        {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}