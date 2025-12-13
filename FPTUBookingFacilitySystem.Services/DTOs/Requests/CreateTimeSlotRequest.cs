namespace FPTUBookingFacilitySystem.Services.DTOs.Requests
{
    public class CreateTimeSlotRequest
    {
        public string Name { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

