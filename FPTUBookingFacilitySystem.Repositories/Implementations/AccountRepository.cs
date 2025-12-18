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

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .OrderBy(a => a.AccountId)
                .ToListAsync();
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> UpdateAccountAsync(int accountId, Account account)
        {
            var existingAccount = await _context.Accounts.FindAsync(accountId);
            if (existingAccount == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                existingAccount.Email = account.Email;
            }

            if (!string.IsNullOrWhiteSpace(account.Password))
            {
                existingAccount.Password = account.Password;
            }

            existingAccount.RoleId = account.RoleId;
            existingAccount.IsActive = account.IsActive;
            existingAccount.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return false;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<Account?> GetActiveAccountByEmailAsync(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email && a.IsActive);
        }

    }
}

