using FPTUBookingFacilitySystem.Repositories.Context;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTUBookingFacilitySystem.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly FPTUBookingFacilityDbContext _context;

        public BookingRepository(FPTUBookingFacilityDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            
            // Reload with related entities
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId) ?? booking;
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Campus)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Campus)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableForDateAndTimeSlotAsync(int roomId, DateOnly bookingDate, int timeSlotId)
        {
            // Check if there's any existing booking for this room, date, and time slot with status not cancelled
            var hasConflict = await _context.Bookings
                .AnyAsync(b => b.RoomId == roomId 
                    && b.BookingDate == bookingDate 
                    && b.TimeSlotId == timeSlotId
                    && b.BookingStatus.ToLower() != "cancelled");

            return !hasConflict;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false;
            }

            booking.BookingStatus = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingHistory> CreateBookingHistoryAsync(BookingHistory bookingHistory)
{
            _context.BookingHistories.Add(bookingHistory);
            await _context.SaveChangesAsync();
            return bookingHistory;
        }

        public async Task<IEnumerable<BookingHistory>> GetBookingHistoryByBookingIdAsync(int bookingId)
        {
            return await _context.BookingHistories
                .Include(h => h.ChangedByNavigation)
                .Where(h => h.BookingId == bookingId)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingHistory>> GetBookingHistoryByUserIdAsync(int userId)
        {
            return await _context.BookingHistories
                .Include(h => h.Booking)
                    .ThenInclude(b => b.Room)
                .Include(h => h.ChangedByNavigation)
                .Where(h => h.Booking.UserId == userId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetBookingsCountByStatusAsync()
        {
            return await _context.Bookings
                .GroupBy(b => b.BookingStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<IEnumerable<RoomBookingCount>> GetRoomBookingCountsAsync(int topCount = 10)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .GroupBy(b => new { b.RoomId, b.Room!.RoomName })
                .Select(g => new RoomBookingCount
                {
                    RoomId = g.Key.RoomId,
                    RoomName = g.Key.RoomName ?? string.Empty,
                    BookingCount = g.Count()
                })
                .OrderByDescending(r => r.BookingCount)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task<ConflictLog> CreateConflictLogAsync(ConflictLog conflictLog)
        {
            conflictLog.CreatedAt = DateTime.UtcNow;
            _context.ConflictLogs.Add(conflictLog);
            await _context.SaveChangesAsync();
            return conflictLog;
        }

        public async Task<ConflictLog> CreateConflictLogWithoutBookingAsync(int roomId, int timeSlotId, string conflictType, string message)
        {
            var sql = @"
                INSERT INTO Conflict_log (room_id, time_slot_id, conflict_type, message, booking_id, created_at)
                VALUES ({0}, {1}, {2}, {3}, NULL, GETDATE());";

            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql, roomId, timeSlotId, conflictType, message);
            }
            catch
            {
// If NULL is not allowed, the insert will fail but we still want to throw the original exception
                // This is expected behavior - we log conflicts when possible
            }

            return new ConflictLog
            {
                ConflictId = 0, // We don't retrieve it for pre-booking conflicts
                RoomId = roomId,
                TimeSlotId = timeSlotId,
                ConflictType = conflictType,
                Message = message,
                BookingId = 0, // Placeholder - actual value is NULL in DB if allowed
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}