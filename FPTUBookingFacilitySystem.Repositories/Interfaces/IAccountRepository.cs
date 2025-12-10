using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(int accountId);
    }
}

