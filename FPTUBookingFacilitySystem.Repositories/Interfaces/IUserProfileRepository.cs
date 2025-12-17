using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetUserProfileByAccountIdAsync(int accountId);
        Task<UserProfile?> GetUserProfileByIdAsync(int userId);
    }
}