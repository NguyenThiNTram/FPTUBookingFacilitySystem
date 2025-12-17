using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTUBookingFacilitySystem.Services.DTOs.Responses
{
    public class BookingResponse
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlotName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}