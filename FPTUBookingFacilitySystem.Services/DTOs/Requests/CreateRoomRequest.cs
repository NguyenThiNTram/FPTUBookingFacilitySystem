namespace FPTUBookingFacilitySystem.Services.DTOs.Requests
{
    public class CreateRoomRequest
    {
        public string RoomName { get; set; } = string.Empty;
        public int CampusId { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomStatus { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}

