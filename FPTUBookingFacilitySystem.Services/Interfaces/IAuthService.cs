using FPTUBookingFacilitySystem.Repositories.Common;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;

namespace FPTUBookingFacilitySystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(string token);
        string GenerateToken(int userId, string email, UserRole role);
    }
}