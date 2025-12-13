using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FPTUBookingFacilitySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTimeSlots()
        {
            var timeSlots = await _timeSlotService.GetAllTimeSlotsAsync();
            return Ok(timeSlots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeSlotById(int id)
        {
            var timeSlot = await _timeSlotService.GetTimeSlotByIdAsync(id);
            if (timeSlot == null)
            {
                return NotFound(new { message = $"TimeSlot with ID {id} not found." });
            }
            return Ok(timeSlot);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimeSlot([FromBody] CreateTimeSlotRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            try
            {
                var timeSlot = await _timeSlotService.CreateTimeSlotAsync(request);
                return CreatedAtAction(nameof(GetTimeSlotById), new { id = timeSlot.Id }, timeSlot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] UpdateTimeSlotRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            try
            {
                var timeSlot = await _timeSlotService.UpdateTimeSlotAsync(id, request);
                if (timeSlot == null)
                {
                    return NotFound(new { message = $"TimeSlot with ID {id} not found." });
                }
                return Ok(timeSlot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var result = await _timeSlotService.DeleteTimeSlotAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"TimeSlot with ID {id} not found." });
            }
            return Ok(new { message = $"TimeSlot with ID {id} deleted successfully." });
        }
    }
}

