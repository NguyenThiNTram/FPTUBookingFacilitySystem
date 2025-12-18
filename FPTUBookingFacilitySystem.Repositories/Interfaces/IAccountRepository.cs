using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task<Account?> UpdateAccountAsync(int accountId, Account account);
        Task<bool> DeleteAccountAsync(int accountId);
        Task<Account?> GetActiveAccountByEmailAsync(string email);

    }
}

