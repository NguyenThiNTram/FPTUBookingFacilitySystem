using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTUBookingFacilitySystem.Repositories.Entities;

public partial class Account
{
        [Column("account_id")]
    public int AccountId { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
