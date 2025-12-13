using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;

namespace FPTUBookingFacilitySystem.Services.Interfaces
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotResponse>> GetAllTimeSlotsAsync();
        Task<TimeSlotResponse?> GetTimeSlotByIdAsync(int id);
        Task<TimeSlotResponse> CreateTimeSlotAsync(CreateTimeSlotRequest request);
        Task<TimeSlotResponse?> UpdateTimeSlotAsync(int id, UpdateTimeSlotRequest request);
        Task<bool> DeleteTimeSlotAsync(int id);
    }
}

