using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTUBookingFacilitySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound(new { message = $"Room with ID {id} not found." });
            }
            return Ok(room);
        }

        // [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                var room = await _roomService.CreateRoomAsync(request);
                return CreatedAtAction(nameof(GetRoomById), new { id = room.RoomId }, room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                var room = await _roomService.UpdateRoomAsync(id, request);
                if (room == null)
                {
                    return NotFound(new { message = $"Room with ID {id} not found." });
                }
                return Ok(room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Roles = "Staff")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRoomStatus(int id, [FromBody] UpdateRoomStatusRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RoomStatus))
            {
                return BadRequest(new { message = "RoomStatus is required." });
            }

            try
            {
                var updated = await _roomService.UpdateRoomStatusAsync(id, request);

                if (!updated)
                {
                    return NotFound(new { message = "Room not found." });
                }

                return Ok(new
                {
                    message = "Room status updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Room with ID {id} not found." });
            }
            return Ok(new { message = $"Room with ID {id} deleted successfully." });
        }
    }
}

