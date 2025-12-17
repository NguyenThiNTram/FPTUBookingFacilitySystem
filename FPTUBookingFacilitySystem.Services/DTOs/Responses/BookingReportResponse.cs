namespace FPTUBookingFacilitySystem.Services.DTOs.Responses
{
    public class BookingReportResponse
    {
        public int TotalBookings { get; set; }
        public Dictionary<string, int> BookingsByStatus { get; set; } = new();
        public List<RoomBookingRanking> MostBookedRooms { get; set; } = new();
    }

    public class RoomBookingRanking
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }
}