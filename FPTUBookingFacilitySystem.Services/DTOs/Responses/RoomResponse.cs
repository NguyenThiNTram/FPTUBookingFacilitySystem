namespace FPTUBookingFacilitySystem.Services.DTOs.Responses
{
    public class RoomResponse
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int CampusId { get; set; }
        public string? CampusName { get; set; }
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public string RoomStatus { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

