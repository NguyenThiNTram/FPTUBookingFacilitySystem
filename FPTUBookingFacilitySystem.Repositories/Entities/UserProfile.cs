using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class UserProfile
{
    public int UserId { get; set; }

    public string UserCode { get; set; } = null!;

    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public int CampusId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Campus Campus { get; set; } = null!;
}
