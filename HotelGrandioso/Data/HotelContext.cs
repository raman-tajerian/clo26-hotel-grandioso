using Microsoft.EntityFrameworkCore;
using HotelGrandioso.Models;
using System.Collections.Generic;
using System.Text;

namespace HotelGrandioso.Data;

public class HotelContext : DbContext
{
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void
OnConfiguring (DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseSqlite("Data Source=hotel.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Samma e-post ska inte ge två gäster

        modelBuilder.Entity<Guest>().HasIndex(g => g.Email).IsUnique();
        modelBuilder.Entity<Guest>().Property(g => g.FirstName).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Guest>().Property(g => g.LastName).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Guest>().Property(g => g.Email).IsRequired().HasMaxLength(100);

        modelBuilder.Entity<Room>().HasIndex(r => r.Number).IsUnique();
        // Enum som text gör databasen läsbar.
        modelBuilder.Entity<Room>().Property(r => r.Type).HasConversion<string>();

        // Ett rum ska inte kunna raderas om det har bokningar.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room).WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Guest).WithMany(g => g.Bookings)
            .HasForeignKey(b => b.GuestId).OnDelete(DeleteBehavior.Cascade);


        // Startdata: Hotellets rum
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, Number = 101, Type = RoomType.Single, PricePerNight = 900m },
            new Room { Id = 2, Number = 102, Type = RoomType.Single, PricePerNight = 900m },
            new Room { Id = 3, Number = 103, Type = RoomType.Single, PricePerNight = 950m },
            new Room { Id = 4, Number = 201, Type = RoomType.Double, PricePerNight = 1400m },
            new Room { Id = 5, Number = 202, Type = RoomType.Double, PricePerNight = 1400m },
            new Room { Id = 6, Number = 203, Type = RoomType.Double, PricePerNight = 1600m },
            new Room { Id = 7, Number = 301, Type = RoomType.Suite, PricePerNight = 2900m },
            new Room { Id = 8, Number = 302, Type = RoomType.Suite, PricePerNight = 3400m });
    }
}

