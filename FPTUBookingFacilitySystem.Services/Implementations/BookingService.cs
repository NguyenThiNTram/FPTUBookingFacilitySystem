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

        public async Task<BookingResponse?> CreateBookingAsync( CreateBookingRequest request, int accountId)
        {
            // ===== 1. Validate account & role =====
            var account = await _accountRepository.GetAccountByIdAsync(accountId)
                ?? throw new ArgumentException("Account not found.");

            if ((UserRole)account.RoleId != UserRole.Student &&
                (UserRole)account.RoleId != UserRole.Lecturer)
                throw new UnauthorizedAccessException(
                    "Only lecturers or students can create booking.");

            var user = await _userProfileRepository
                .GetUserProfileByAccountIdAsync(accountId)
                ?? throw new ArgumentException("User profile not found.");

            // ===== 2. Validate room & timeslot =====
            var room = await _roomRepository.GetRoomByIdAsync(request.RoomId)
                ?? throw new ArgumentException("Room not found.");

            var timeSlot = await _timeSlotRepository
                .GetTimeSlotByIdAsync(request.TimeSlotId)
                ?? throw new ArgumentException("Time slot not found.");

            if (!timeSlot.IsActive)
                throw new InvalidOperationException("Time slot is not active.");

            var bookingDate = DateOnly.FromDateTime(request.BookingDate);

            // ===== 3. Tạo Booking trước (status = pending) =====
            var booking = new Booking
            {
                UserId = user.UserId,
                RoomId = room.RoomId,
                TimeSlotId = timeSlot.Id,
                BookingDate = bookingDate,
                BookingStatus = "pending",
                Reason = request.Reason,
                BookingTime = DateTime.UtcNow
            };

            booking = await _bookingRepository.CreateBookingAsync(booking);

            // ===== 4. Check room unavailable =====
            if (!room.RoomStatus.Equals("available", StringComparison.OrdinalIgnoreCase))
            {
                await HandleConflictAsync(
                    booking,
                    "room_unavailable",
                    $"Room {room.RoomName} is {room.RoomStatus}");

                throw new InvalidOperationException(
                    $"Room {room.RoomName} is not available.");
            }

            // ===== 5. Check double booking =====
            var isAvailable =
                await _bookingRepository.IsRoomAvailableForDateAndTimeSlotAsync(
                    room.RoomId, bookingDate, timeSlot.Id);

            if (!isAvailable)
            {
                await HandleConflictAsync(
                    booking,
                    "double_booking",
                    $"Room {room.RoomName} already booked on {bookingDate} ({timeSlot.Name})");

                throw new InvalidOperationException(
                    $"Room {room.RoomName} already booked for this time slot.");
            }

            // ===== 6. Create initial booking history =====
            await _bookingRepository.CreateBookingHistoryAsync(new BookingHistory
            {
                BookingId = booking.BookingId,
                ChangedBy = user.UserId,
                OldStatus = null,
                NewStatus = "pending",
                ChangedAt = DateTime.UtcNow,
                Comment = "Booking created"
            });

            return MapToResponse(booking);
        }

        private async Task HandleConflictAsync(Booking booking, string conflictType, string message)
        {
            // Update booking status
            booking.BookingStatus = "rejected";
            await _bookingRepository.UpdateBookingStatusAsync(booking.BookingId, "rejected");

            // Log conflict
            await _bookingRepository.CreateConflictLogAsync(new ConflictLog
            {
                BookingId = booking.BookingId,
                RoomId = booking.RoomId,
                TimeSlotId = booking.TimeSlotId,
                ConflictType = conflictType,
                Message = message,
                CreatedAt = DateTime.UtcNow
            });

            // Booking history
            await _bookingRepository.CreateBookingHistoryAsync(new BookingHistory
            {
                BookingId = booking.BookingId,
                ChangedBy = booking.UserId,
                OldStatus = "pending",
                NewStatus = "rejected",
                ChangedAt = DateTime.UtcNow,
                Comment = conflictType
            });
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
                

            // var user = await _userProfileRepository.GetUserProfileByAccountIdAsync(accountId);

            // var role = (UserRole)account.RoleId;

            // // User chỉ xem booking của chính mình
            // if (role == UserRole.Student || role == UserRole.Lecturer)
            // {
            //     if (user == null || booking.UserId != user.UserId)
            //         throw new UnauthorizedAccessException("You are not allowed to view this booking history.");
            // }

            var history = await _bookingRepository.GetBookingHistoryByBookingIdAsync(bookingId);
            return history.Select(MapToHistoryResponse);
        }

        public async Task<IEnumerable<BookingHistoryResponse>> GetBookingHistoryByAccountAsync(int accountId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new UnauthorizedAccessException();

            var user = await _userProfileRepository.GetUserProfileByAccountIdAsync(accountId);
            if (user == null)
                throw new ArgumentException("User profile not found.");

            var histories = await _bookingRepository
                .GetBookingHistoryByUserIdAsync(user.UserId);

            return histories.Select(MapToHistoryResponse);
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