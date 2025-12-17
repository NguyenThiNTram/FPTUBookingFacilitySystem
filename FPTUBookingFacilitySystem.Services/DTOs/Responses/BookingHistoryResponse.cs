namespace FPTUBookingFacilitySystem.Services.DTOs.Responses
{
    public class BookingHistoryResponse
    {
        public int HistoryId { get; set; }
        public int BookingId { get; set; }
        public int ChangedBy { get; set; }
        public string? ChangedByName { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Comment { get; set; }
    }
}