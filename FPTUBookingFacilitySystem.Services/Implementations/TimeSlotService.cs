using FPTUBookingFacilitySystem.Repositories.Entities;
using FPTUBookingFacilitySystem.Repositories.Interfaces;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.DTOs.Responses;
using FPTUBookingFacilitySystem.Services.Interfaces;

namespace FPTUBookingFacilitySystem.Services.Implementations
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<IEnumerable<TimeSlotResponse>> GetAllTimeSlotsAsync()
        {
            var timeSlots = await _timeSlotRepository.GetAllTimeSlotsAsync();
            return timeSlots.Select(ts => MapToResponse(ts));
        }

        public async Task<TimeSlotResponse?> GetTimeSlotByIdAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetTimeSlotByIdAsync(id);
            if (timeSlot == null)
            {
                return null;
            }
            return MapToResponse(timeSlot);
        }

        public async Task<TimeSlotResponse> CreateTimeSlotAsync(CreateTimeSlotRequest request)
        {
            // Validate that EndTime is after StartTime
            if (request.EndTime <= request.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }

            var timeSlot = new TimeSlot
            {
                Name = request.Name,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = request.IsActive
            };

            var createdTimeSlot = await _timeSlotRepository.CreateTimeSlotAsync(timeSlot);
            return MapToResponse(createdTimeSlot);
        }

        public async Task<TimeSlotResponse?> UpdateTimeSlotAsync(int id, UpdateTimeSlotRequest request)
        {
            // Validate that EndTime is after StartTime
            if (request.EndTime <= request.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }

            var timeSlot = new TimeSlot
            {
                Name = request.Name,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = request.IsActive
            };

            var updatedTimeSlot = await _timeSlotRepository.UpdateTimeSlotAsync(id, timeSlot);
            if (updatedTimeSlot == null)
            {
                return null;
            }
            return MapToResponse(updatedTimeSlot);
        }

        public async Task<bool> DeleteTimeSlotAsync(int id)
        {
            return await _timeSlotRepository.DeleteTimeSlotAsync(id);
        }

        private static TimeSlotResponse MapToResponse(TimeSlot timeSlot)
        {
            return new TimeSlotResponse
            {
                Id = timeSlot.Id,
                Name = timeSlot.Name,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                IsActive = timeSlot.IsActive,
                CreatedAt = timeSlot.CreatedAt,
                UpdatedAt = timeSlot.UpdatedAt
            };
        }
    }
}

