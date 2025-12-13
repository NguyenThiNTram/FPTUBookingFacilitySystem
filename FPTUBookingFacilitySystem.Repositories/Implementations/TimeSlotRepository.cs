using FPTUBookingFacilitySystem.Repositories.Context;
using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTUBookingFacilitySystem.Repositories.Implementations
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly FPTUBookingFacilityDbContext _context;

        public TimeSlotRepository(FPTUBookingFacilityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllTimeSlotsAsync()
        {
            return await _context.TimeSlots
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetTimeSlotByIdAsync(int id)
        {
            return await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<TimeSlot> CreateTimeSlotAsync(TimeSlot timeSlot)
        {
            timeSlot.CreatedAt = DateTime.UtcNow;
            timeSlot.UpdatedAt = DateTime.UtcNow;
            
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return timeSlot;
        }

        public async Task<TimeSlot?> UpdateTimeSlotAsync(int id, TimeSlot timeSlot)
        {
            var existingTimeSlot = await _context.TimeSlots.FindAsync(id);
            if (existingTimeSlot == null)
            {
                return null;
            }

            existingTimeSlot.Name = timeSlot.Name;
            existingTimeSlot.StartTime = timeSlot.StartTime;
            existingTimeSlot.EndTime = timeSlot.EndTime;
            existingTimeSlot.IsActive = timeSlot.IsActive;
            existingTimeSlot.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingTimeSlot;
        }

        public async Task<bool> DeleteTimeSlotAsync(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return false;
            }

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

