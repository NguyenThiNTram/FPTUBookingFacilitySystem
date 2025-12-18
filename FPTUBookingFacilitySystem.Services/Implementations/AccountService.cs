using FPTUBookingFacilitySystem.Repositories.Common;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;
using FPTUBookingFacilitySystem.Services.Interfaces;

namespace FPTUBookingFacilitySystem.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<AccountResponse>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            return accounts.Select(MapToResponse);
        }

        public async Task<AccountResponse?> GetAccountByIdAsync(int accountId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return null;
            }
            return MapToResponse(account);
        }

        public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            {
                throw new ArgumentException("Valid email is required.");
            }

            // Check if email already exists
            var existingAccount = await _accountRepository.GetAccountByEmailAsync(request.Email);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Account with email {request.Email} already exists.");
            }
            
            if (string.IsNullOrWhiteSpace(request.Email) ||
                !request.Email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email must end with @fpt.edu.vn");
            }


            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }

            // Validate role
            if (!Enum.IsDefined(typeof(UserRole), request.RoleId))
            {
                throw new ArgumentException("Invalid role ID.");
            }

            var account = new Account
            {
                Email = request.Email,
                Password = request.Password,
                RoleId = request.RoleId,
                IsActive = request.IsActive
            };

            var createdAccount = await _accountRepository.CreateAccountAsync(account);
            return MapToResponse(createdAccount);
        }

        public async Task<AccountResponse?> UpdateAccountAsync(int accountId, UpdateAccountRequest request)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return null;
            }

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != account.Email)
            {
                var existingAccount = await _accountRepository.GetAccountByEmailAsync(request.Email);
                if (existingAccount != null)
                {
                    throw new InvalidOperationException($"Account with email {request.Email} already exists.");
                }
            }

            // Create account object with updated values
            var updatedAccount = new Account
            {
                Email = request.Email ?? account.Email,
                Password = request.Password ?? account.Password,
                RoleId = request.RoleId ?? account.RoleId,
                IsActive = request.IsActive ?? account.IsActive
            };

            // Validate role if it's being updated
            if (request.RoleId.HasValue && !Enum.IsDefined(typeof(UserRole), request.RoleId.Value))
            {
                throw new ArgumentException("Invalid role ID.");
            }

            // Validate password if it's being updated
            if (!string.IsNullOrWhiteSpace(request.Password) && request.Password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long.");
            }

            var result = await _accountRepository.UpdateAccountAsync(accountId, updatedAccount);
            return result != null ? MapToResponse(result) : null;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            return await _accountRepository.DeleteAccountAsync(accountId);
        }

        private static AccountResponse MapToResponse(Account account)
        {
            return new AccountResponse
            {
                AccountId = account.AccountId,
                Email = account.Email,
                IsActive = account.IsActive,
                RoleId = account.RoleId,
                Role = ((UserRole)account.RoleId).ToString(),
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        
    }
}