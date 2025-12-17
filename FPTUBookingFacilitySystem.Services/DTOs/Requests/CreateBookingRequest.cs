using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTUBookingFacilitySystem.Services.DTOs.Requests
{
    public class CreateBookingRequest
    {
        public int RoomId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}