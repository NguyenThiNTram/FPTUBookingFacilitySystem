using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class BookingHistory
{
    public int HistoryId { get; set; }

    public int BookingId { get; set; }

    public int ChangedBy { get; set; }

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Comment { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual UserProfile ChangedByNavigation { get; set; } = null!;
}
