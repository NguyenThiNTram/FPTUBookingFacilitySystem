using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public int TimeSlotId { get; set; }

    public DateOnly BookingDate { get; set; }

    public DateTime BookingTime { get; set; }

    public string BookingStatus { get; set; } = null!;

    public string? Reason { get; set; }

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    public virtual ICollection<ConflictLog> ConflictLogs { get; set; } = new List<ConflictLog>();

    public virtual Room Room { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
