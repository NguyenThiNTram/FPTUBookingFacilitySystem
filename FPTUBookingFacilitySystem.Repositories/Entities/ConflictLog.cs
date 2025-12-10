using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class ConflictLog
{
    public int ConflictId { get; set; }

    public int BookingId { get; set; }

    public int RoomId { get; set; }

    public int TimeSlotId { get; set; }

    public string ConflictType { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
