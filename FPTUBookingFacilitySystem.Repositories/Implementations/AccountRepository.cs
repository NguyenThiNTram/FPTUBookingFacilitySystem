using FPTUBookingFacilitySystem.Repositories.Context;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTUBookingFacilitySystem.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FPTUBookingFacilityDbContext _context;

        public AccountRepository(FPTUBookingFacilityDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
    }
}

