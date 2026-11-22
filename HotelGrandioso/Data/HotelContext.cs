using Microsoft.EntityFrameworkCore;
using HotelGrandioso.Models;

namespace HotelGrandioso.Data;

public class HotelContext : DbContext
{
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<SpaBooking> SpaBookings => Set<SpaBooking>();

    // Parameterlös konfiguration, dotnet ef hittar databasen
    protected override void OnConfiguring (DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseSqlite("Data Source=hotel.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Två rum får inte ha samma nummer
        modelBuilder.Entity<Room>().HasIndex(r => r.RoomNumber).IsUnique();

        modelBuilder.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Standard", Description = "Standardrum" },
            new RoomType { Id = 2, Name = "Luxury", Description = "Lyxsvit" },
            new RoomType { Id = 3, Name = "Bridal", Description = "Bröllopsvit" });

        // Rummen byggs med en hjälpmetod istället för en massa nästan likadana rader 
        var rooms = new List<Room>();
        var nextId = 1;

        void AddRooms(int firstNumber, int lastNumber, int floor, int sizeM2, decimal price, int roomTypeId)
        {
            for (var number = firstNumber; number <= lastNumber; number++)
            {
                rooms.Add(new Room
                {
                    Id = nextId++,
                    RoomNumber = number, 
                    Floor = floor,
                    SizeM2 = sizeM2,
                    PricePerNight = price,
                    RoomTypeId = roomTypeId


                });
            }
        }

        AddRooms(101, 105, 1, 80, 800m, 1);
        AddRooms(201, 205, 2, 100, 1000m, 1);
        AddRooms(301, 303, 3, 120, 1200m, 1);
        AddRooms(304, 304, 3, 180, 2800m, 2);
        AddRooms(305, 305, 3, 220, 3500m, 3);

        modelBuilder.Entity<Room>().HasData(rooms);

        // Längderna utom massagen står inte i uppgiften, så de är rimliga antaganden.
        modelBuilder.Entity<Treatment>().HasData(
            new Treatment { Id = 1, Name = "Entré (per timme)", DurationMinutes = 60, Price = 150m },
            new Treatment { Id = 2, Name = "Massage", DurationMinutes = 60, Price = 950m },
            new Treatment { Id = 3, Name = "Pedikyr (stortån)", DurationMinutes = 30, Price = 350m },
            new Treatment { Id = 4, Name = "Pedikyr (hel fot)", DurationMinutes = 45, Price = 650m });

    }
}

