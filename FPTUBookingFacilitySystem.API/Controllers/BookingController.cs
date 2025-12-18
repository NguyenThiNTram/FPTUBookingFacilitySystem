using System.Security.Claims;
using FPTUBookingFacilitySystem.Repositories.Common;
using FPTUBookingFacilitySystem.Services.DTOs.Requests;
using FPTUBookingFacilitySystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTUBookingFacilitySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        private int GetCurrentAccountId()
        {
            // JWT token stores AccountId in "sub" claim (from AuthService.GenerateToken)
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
            {
                throw new UnauthorizedAccessException("Account ID not found in token.");
            }

            return accountId;
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer,Student")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(new { message = "Reason is required." });
            }

            try
            {
                var accountId = GetCurrentAccountId();
                var booking = await _bookingService.CreateBookingAsync(request, accountId);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Lecturer,Student,Staff")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Lecturer,Student,Staff")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
return NotFound(new { message = $"Booking with ID {id} not found." });
            }
            return Ok(booking);
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetBookingHistory(int id)
        {
            try
            {
                 var accountId = GetCurrentAccountId();

                var history = await _bookingService
                    .GetBookingHistoryByBookingIdAsync(id, accountId);

                return Ok(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Lecturer,Student")]
        [HttpGet("my/history")]
        public async Task<IActionResult> GetMyBookingHistory()
        {
            var accountId = GetCurrentAccountId();
            var history = await _bookingService.GetBookingHistoryByAccountAsync(accountId);
            return Ok(history);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            try
            {
                var staffAccountId = GetCurrentAccountId();
                var booking = await _bookingService.ApproveBookingAsync(id, staffAccountId);
                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> RejectBooking(int id, [FromBody] RejectBookingRequest? request = null)
        {
            try
            {
                var staffAccountId = GetCurrentAccountId();
                var comment = request?.Comment;
                var booking = await _bookingService.RejectBookingAsync(id, staffAccountId, comment);
                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                var booking = await _bookingService.CancelBookingAsync(id, accountId);
                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("report")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetBookingReport()
        {
            var report = await _bookingService.GetBookingReportAsync();
            return Ok(report);
        }
    }
}