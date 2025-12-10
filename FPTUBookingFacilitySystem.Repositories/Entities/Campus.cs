using System;
using System.Collections.Generic;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class Campus
{
    public int CampusId { get; set; }

    public string CampusName { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
