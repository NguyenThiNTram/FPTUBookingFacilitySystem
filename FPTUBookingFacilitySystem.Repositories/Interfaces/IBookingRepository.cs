using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<bool> IsRoomAvailableForDateAndTimeSlotAsync(int roomId, DateOnly bookingDate, int timeSlotId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus);
        Task<BookingHistory> CreateBookingHistoryAsync(BookingHistory bookingHistory);
        Task<IEnumerable<BookingHistory>> GetBookingHistoryByBookingIdAsync(int bookingId);
        Task<ConflictLog> CreateConflictLogAsync(ConflictLog conflictLog);
        Task<ConflictLog> CreateConflictLogWithoutBookingAsync(int roomId, int timeSlotId, string conflictType, string message);
        Task<int> GetTotalBookingsCountAsync();
        Task<Dictionary<string, int>> GetBookingsCountByStatusAsync();
        Task<IEnumerable<RoomBookingCount>> GetRoomBookingCountsAsync(int topCount = 10);
    }

    public class RoomBookingCount
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }
}