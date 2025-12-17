using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;

namespace FPTUBookingFacilitySystem.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse?> CreateBookingAsync(CreateBookingRequest request, int accountId);
        Task<BookingResponse?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingResponse>> GetAllBookingsAsync();
        Task<IEnumerable<BookingHistoryResponse>> GetBookingHistoryByBookingIdAsync(int bookingId, int accountId);
        Task<BookingResponse?> ApproveBookingAsync(int bookingId, int staffAccountId);
        Task<BookingResponse?> RejectBookingAsync(int bookingId, int staffAccountId, string? comment = null);
        Task<BookingResponse?> CancelBookingAsync(int bookingId, int accountId);
    }
}