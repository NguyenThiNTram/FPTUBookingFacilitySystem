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
    }
}