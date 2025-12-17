using FPTUBookingFacilitySystem.Repositories.Context;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTUBookingFacilitySystem.Repositories.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly FPTUBookingFacilityDbContext _context;

        public UserProfileRepository(FPTUBookingFacilityDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetUserProfileByAccountIdAsync(int accountId)
        {
            return await _context.UserProfiles
                .Include(u => u.Campus)
                .FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync(int userId)
        {
            return await _context.UserProfiles
                .Include(u => u.Campus)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}