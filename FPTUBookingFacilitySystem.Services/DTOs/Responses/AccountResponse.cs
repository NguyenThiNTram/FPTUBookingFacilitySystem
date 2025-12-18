namespace FPTUBookingFacilitySystem.Services.DTOs.Responses
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}