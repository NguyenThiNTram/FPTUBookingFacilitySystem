using FPTUBookingFacilitySystem.Repositories.Entities;

namespace FPTUBookingFacilitySystem.Repositories.Interfaces
{
    public interface ITimeSlotRepository
    {
        Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync();
        Task<TimeSlot?> GetTimeSlotByIdAsync(int id);
        Task<TimeSlot> CreateTimeSlotAsync(TimeSlot timeSlot);
        Task<TimeSlot?> UpdateTimeSlotAsync(int id, TimeSlot timeSlot);
        Task<bool> DeleteTimeSlotAsync(int id);
    }
}

