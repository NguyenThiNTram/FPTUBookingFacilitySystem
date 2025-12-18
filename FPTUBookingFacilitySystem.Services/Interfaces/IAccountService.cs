using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;

namespace FPTUBookingFacilitySystem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountResponse>> GetAllAccountsAsync();
        Task<AccountResponse?> GetAccountByIdAsync(int accountId);
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
        Task<AccountResponse?> UpdateAccountAsync(int accountId, UpdateAccountRequest request);
        Task<bool> DeleteAccountAsync(int accountId);
    }
}