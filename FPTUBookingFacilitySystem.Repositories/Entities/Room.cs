using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public int CampusId { get; set; }

    public int RoomTypeId { get; set; }

    public string RoomStatus { get; set; } = null!;

    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<ConflictLog> ConflictLogs { get; set; } = new List<ConflictLog>();

    public virtual RoomType RoomType { get; set; } = null!;

    public virtual ICollection<RoomUsageStat> RoomUsageStats { get; set; } = new List<RoomUsageStat>();
}
