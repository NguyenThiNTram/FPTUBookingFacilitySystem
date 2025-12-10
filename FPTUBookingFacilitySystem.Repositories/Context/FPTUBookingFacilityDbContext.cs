using System;
using System.Collections.Generic;
using FPTUBookingFacilitySystem.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FPTUBookingFacilitySystem.Repositories.Context;

public partial class FPTUBookingFacilityDbContext : DbContext
{
    public FPTUBookingFacilityDbContext()
    {
    }

    public FPTUBookingFacilityDbContext(DbContextOptions<FPTUBookingFacilityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingHistory> BookingHistories { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<ConflictLog> ConflictLogs { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<RoomUsageStat> RoomUsageStats { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Lấy connection string từ appsettings.json qua configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__46A222CD49C4AB29");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__5DE3A5B1DE4FF36C");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate).HasColumnName("booking_date");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("booking_status");
            entity.Property(e => e.BookingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_time");
            entity.Property(e => e.Reason)
                .HasColumnType("text")
                .HasColumnName("reason");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.TimeSlotId).HasColumnName("time_slot_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Room");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_TimeSlot");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<BookingHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Booking___096AA2E905D84488");

            entity.ToTable("Booking_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("old_status");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingHistory_Booking");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.BookingHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingHistory_User");
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__Campus__01989FD19DA870DF");

            entity.ToTable("Campus");

            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.CampusName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("campus_name");
        });

        modelBuilder.Entity<ConflictLog>(entity =>
        {
            entity.HasKey(e => e.ConflictId).HasName("PK__Conflict__B9863B7A868377D7");

            entity.ToTable("Conflict_log");

            entity.Property(e => e.ConflictId).HasColumnName("conflict_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ConflictType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("conflict_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.TimeSlotId).HasColumnName("time_slot_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.ConflictLogs)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conflict_Booking");

            entity.HasOne(d => d.Room).WithMany(p => p.ConflictLogs)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conflict_Room");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.ConflictLogs)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conflict_TimeSlot");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__19675A8AC7E48799");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.RoomName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("room_name");
            entity.Property(e => e.RoomStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("room_status");
            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Campus).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Campus");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_RoomType");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__Room_typ__42395E847AEF896D");

            entity.ToTable("Room_type");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<RoomUsageStat>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("PK__Room_usa__B8A52560D9723766");

            entity.ToTable("Room_usage_stats");

            entity.Property(e => e.StatId).HasColumnName("stat_id");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.TotalAvailableHours).HasColumnName("total_available_hours");
            entity.Property(e => e.TotalBookings).HasColumnName("total_bookings");
            entity.Property(e => e.TotalUsageHours).HasColumnName("total_usage_hours");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomUsageStats)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsageStats_Room");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__time_slo__3213E83F325753FD");

            entity.ToTable("time_slots");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_pro__B9BE370FD8C5BB28");

            entity.ToTable("User_profile");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CampusId).HasColumnName("campus_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UserCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_code");

            entity.HasOne(d => d.Account).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_Account");

            entity.HasOne(d => d.Campus).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_Campus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
