using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class RoomUsageStat
{
    public int StatId { get; set; }

    public int RoomId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public int TotalBookings { get; set; }

    public double TotalUsageHours { get; set; }

    public double TotalAvailableHours { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Room Room { get; set; } = null!;
}
