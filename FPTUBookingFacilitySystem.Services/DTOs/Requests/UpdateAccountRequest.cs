namespace FPTUBookingFacilitySystem.Services.DTOs.Requests
{
    public class UpdateAccountRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}