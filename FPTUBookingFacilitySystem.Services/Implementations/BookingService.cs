using FPTUBookingFacilitySystem.Repositories.Common;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;
using FPTUBookingFacilitySystem.Services.Interfaces;

namespace FPTUBookingFacilitySystem.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IAccountRepository _accountRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserProfileRepository userProfileRepository,
            IRoomRepository roomRepository,
            ITimeSlotRepository timeSlotRepository,
            IAccountRepository accountRepository)
        {
            _bookingRepository = bookingRepository;
            _userProfileRepository = userProfileRepository;
            _roomRepository = roomRepository;
            _timeSlotRepository = timeSlotRepository;
            _accountRepository = accountRepository;
        }

        public async Task<BookingResponse?> CreateBookingAsync(CreateBookingRequest request, int accountId)
        {
            // Lấy account và role
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            // Role check: chỉ Lecturer hoặc Student mới được tạo booking
            if ((UserRole)account.RoleId != UserRole.Lecturer && (UserRole)account.RoleId != UserRole.Student)
                throw new UnauthorizedAccessException("Only lecturers or students can create booking.");

            // Get UserProfile from AccountId
            var user = await _userProfileRepository.GetUserProfileByAccountIdAsync(accountId);
            if (user == null)
                throw new ArgumentException("User profile not found. Please complete your profile.");
            
            var userId = user.UserId;

            // Validate room exists
            var room = await _roomRepository.GetRoomByIdAsync(request.RoomId);
            if (room == null)
            {
                throw new ArgumentException("Room not found.");
            }

            // Validate room is available
            if (room.RoomStatus.ToLower() != "available")
            {
                // // Log conflict (before booking creation, so no BookingId)
                // await _bookingRepository.CreateConflictLogWithoutBookingAsync(
                //     request.RoomId,
                //     request.TimeSlotId,
                //     "Room Unavailable",
                //     $"Room {room.RoomName} is not available. Current status: {room.RoomStatus}");

                // throw new InvalidOperationException($"Room {room.RoomName} is not available for booking. Status: {room.RoomStatus}");
                await _bookingRepository.CreateConflictLogAsync(new ConflictLog
                {
                    RoomId = room.RoomId,
                    TimeSlotId = request.TimeSlotId,
                    ConflictType = "Room Unavailable",
                    Message = $"Room {room.RoomName} is {room.RoomStatus}",
                    CreatedAt = DateTime.UtcNow
                });
                throw new InvalidOperationException($"Room {room.RoomName} is not available.");
            }

            // Validate time slot exists
            var timeSlot = await _timeSlotRepository.GetTimeSlotByIdAsync(request.TimeSlotId);
            if (timeSlot == null)
                throw new ArgumentException("Time slot not found.");

            if (!timeSlot.IsActive)
                throw new InvalidOperationException("Time slot is not active.");

            // Convert DateTime to DateOnly
            var bookingDate = DateOnly.FromDateTime(request.BookingDate);

            // Check if room is already booked for this date and time slot
            var isAvailable = await _bookingRepository.IsRoomAvailableForDateAndTimeSlotAsync(
                request.RoomId, bookingDate, request.TimeSlotId);

            if (!isAvailable)
            {
                // // Log conflict (before booking creation, so no BookingId)
                // await _bookingRepository.CreateConflictLogWithoutBookingAsync(
                //     request.RoomId,
                //     request.TimeSlotId,
                //     "Double Booking",
                //     $"Room {room.RoomName} is already booked for {bookingDate} at time slot {timeSlot.Name}");

                // throw new InvalidOperationException($"Room {room.RoomName} is already booked for {bookingDate} at time slot {timeSlot.Name}");
                await _bookingRepository.CreateConflictLogAsync(new ConflictLog
                {
                    RoomId = room.RoomId,
                    TimeSlotId = timeSlot.Id,
                    ConflictType = "Double Booking",
                    Message = $"Room {room.RoomName} already booked on {bookingDate} for {timeSlot.Name}",
                    CreatedAt = DateTime.UtcNow
                });
                throw new InvalidOperationException($"Room {room.RoomName} already booked for this slot.");
            }

            // Create booking with "pending" status
            var booking = new Booking
            {
                UserId = userId,
                RoomId = request.RoomId,
                TimeSlotId = request.TimeSlotId,
                BookingDate = bookingDate,
                BookingStatus = "pending",
                Reason = request.Reason,
                BookingTime = DateTime.UtcNow
            };

            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);

            // Create initial booking history (from null to "pending")
            var bookingHistory = new BookingHistory
            {
                BookingId = createdBooking.BookingId,
                ChangedBy = userId,
                OldStatus = null,
                NewStatus = "pending",
                ChangedAt = DateTime.UtcNow,
                Comment = "Booking created"
            };
            await _bookingRepository.CreateBookingHistoryAsync(bookingHistory);

            return MapToResponse(createdBooking);
        }

        public async Task<BookingResponse?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return null;
            }

            return MapToResponse(booking);
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return bookings.Select(MapToResponse);
        }

        public async Task<IEnumerable<BookingHistoryResponse>> GetBookingHistoryByBookingIdAsync(int bookingId, int accountId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
                throw new ArgumentException("Booking not found.");

            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new UnauthorizedAccessException();

            var user = await _userProfileRepository.GetUserProfileByAccountIdAsync(accountId);

            var role = (UserRole)account.RoleId;

            // User chỉ xem booking của chính mình
            if (role == UserRole.Student || role == UserRole.Lecturer)
            {
                if (user == null || booking.UserId != user.UserId)
                    throw new UnauthorizedAccessException("You are not allowed to view this booking history.");
            }

            // Staff / Admin → xem tất cả
            var history = await _bookingRepository.GetBookingHistoryByBookingIdAsync(bookingId);
            return history.Select(MapToHistoryResponse);
        }


        public async Task<BookingResponse?> ApproveBookingAsync(int bookingId, int staffAccountId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                throw new ArgumentException("Booking not found.");
            }

            if (booking.BookingStatus.ToLower() != "pending")
            {
                throw new InvalidOperationException($"Cannot approve booking. Current status is {booking.BookingStatus}.");
            }

            // Get staff UserProfile
            var staffUser = await _userProfileRepository.GetUserProfileByAccountIdAsync(staffAccountId);
            if (staffUser == null)
            {
                throw new ArgumentException("Staff user profile not found.");
            }

            var oldStatus = booking.BookingStatus;
            var success = await _bookingRepository.UpdateBookingStatusAsync(bookingId, "approved");
            if (!success)
            {
                return null;
            }

            // Create booking history
            var bookingHistory = new BookingHistory
            {
                BookingId = bookingId,
                ChangedBy = staffUser.UserId,
                OldStatus = oldStatus,
                NewStatus = "approved",
                ChangedAt = DateTime.UtcNow,
                Comment = "Booking approved by staff"
            };
            await _bookingRepository.CreateBookingHistoryAsync(bookingHistory);

            // Reload booking to get updated data
            booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            return booking != null ? MapToResponse(booking) : null;
        }

        public async Task<BookingResponse?> RejectBookingAsync(int bookingId, int staffAccountId, string? comment = null)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                throw new ArgumentException("Booking not found.");
            }

            if (booking.BookingStatus.ToLower() != "pending")
            {
                throw new InvalidOperationException($"Cannot reject booking. Current status is {booking.BookingStatus}.");
            }

            // Get staff UserProfile
            var staffUser = await _userProfileRepository.GetUserProfileByAccountIdAsync(staffAccountId);
            if (staffUser == null)
            {
                throw new ArgumentException("Staff user profile not found.");
            }

            var oldStatus = booking.BookingStatus;
            var success = await _bookingRepository.UpdateBookingStatusAsync(bookingId, "rejected");
            if (!success)
            {
                return null;
            }

            // Create booking history
            var bookingHistory = new BookingHistory
            {
                BookingId = bookingId,
                ChangedBy = staffUser.UserId,
                OldStatus = oldStatus,
                NewStatus = "rejected",
                ChangedAt = DateTime.UtcNow,
                Comment = comment ?? "Booking rejected by staff"
            };
            await _bookingRepository.CreateBookingHistoryAsync(bookingHistory);

            // Reload booking to get updated data
            booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            return booking != null ? MapToResponse(booking) : null;
        }

        public async Task<BookingResponse?> CancelBookingAsync(int bookingId, int accountId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                throw new ArgumentException("Booking not found.");
            }

            // Only allow cancellation if status is pending
            if (booking.BookingStatus.ToLower() != "pending")
            {
                throw new InvalidOperationException($"Cannot cancel booking. Current status is {booking.BookingStatus}. Only pending bookings can be cancelled.");
            }

            // Get UserProfile to verify ownership
            var user = await _userProfileRepository.GetUserProfileByAccountIdAsync(accountId);
            if (user == null)
            {
                throw new ArgumentException("User profile not found.");
            }

            // Verify user owns the booking
            if (booking.UserId != user.UserId)
            {
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");
            }

            var userId = user.UserId;

            var oldStatus = booking.BookingStatus;
            var success = await _bookingRepository.UpdateBookingStatusAsync(bookingId, "cancelled");
            if (!success)
            {
                return null;
            }

            // Create booking history
            var bookingHistory = new BookingHistory
            {
                BookingId = bookingId,
                ChangedBy = userId,
                OldStatus = oldStatus,
                NewStatus = "cancelled",
                ChangedAt = DateTime.UtcNow,
                Comment = "Booking cancelled by user"
            };
            await _bookingRepository.CreateBookingHistoryAsync(bookingHistory);

            // Reload booking to get updated data
            booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            return booking != null ? MapToResponse(booking) : null;
        }

        private static BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                RoomId = booking.RoomId,
                RoomName = booking.Room?.RoomName ?? string.Empty,
                UserId = booking.UserId,
                UserName = booking.User?.FullName,
                TimeSlotId = booking.TimeSlotId,
                TimeSlotName = booking.TimeSlot?.Name ?? string.Empty,
                BookingDate = booking.BookingDate.ToDateTime(TimeOnly.MinValue),
                Status = booking.BookingStatus,
                Reason = booking.Reason ?? string.Empty,
                CreatedAt = booking.BookingTime
            };
        }

        private static BookingHistoryResponse MapToHistoryResponse(BookingHistory history)
        {
            return new BookingHistoryResponse
            {
                HistoryId = history.HistoryId,
                BookingId = history.BookingId,
                ChangedBy = history.ChangedBy,
                ChangedByName = history.ChangedByNavigation?.FullName,
                OldStatus = history.OldStatus,
                NewStatus = history.NewStatus,
                ChangedAt = history.ChangedAt,
                Comment = history.Comment
            };
        }

        public async Task<BookingReportResponse> GetBookingReportAsync()
        {
            var totalBookings = await _bookingRepository.GetTotalBookingsCountAsync();
            var bookingsByStatus = await _bookingRepository.GetBookingsCountByStatusAsync();
            var roomBookingCounts = await _bookingRepository.GetRoomBookingCountsAsync(10);

            return new BookingReportResponse
            {
                TotalBookings = totalBookings,
                BookingsByStatus = bookingsByStatus,
                MostBookedRooms = roomBookingCounts.Select(r => new RoomBookingRanking
                {
                    RoomId = r.RoomId,
                    RoomName = r.RoomName,
                    BookingCount = r.BookingCount
                }).ToList()
            };
        }
    }
}