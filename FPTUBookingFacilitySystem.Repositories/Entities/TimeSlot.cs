using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class TimeSlot
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ConflictLog> ConflictLogs { get; set; } = new List<ConflictLog>();
}
