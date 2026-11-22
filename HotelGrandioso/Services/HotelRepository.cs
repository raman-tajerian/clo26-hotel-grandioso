using HotelGrandioso.Data;
using HotelGrandioso.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelGrandioso.Services;

// All kontakt med databasen ligger här. Inga regler, de hör hemma i BookingService.
public class HotelRepository
{
    private readonly HotelContext _db;

    public HotelRepository(HotelContext db)
    {
        _db = db;
    }

    // Include behövs för att rumstypens namn ska följa med, annars är RoomType tom.
    public List<Room> GetAllRooms()
    {
        return _db.Rooms
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }

    // Gäst och rum hämtas med, annars är de tomma när bokningarna ska skrivas ut.
    public List<Booking> GetAllBookings()
    {
        return _db.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .ThenInclude(r => r.RoomType)
            .OrderBy(b => b.CheckIn)
            .ToList();
    }


    public void AddBooking(Booking booking)
    {
        // Är gästen ny sparas den automatiskt tillsammans med bokningen.
        _db.Bookings.Add(booking);

        // Utan SaveChanges skrivs ingenting till databasen.
        _db.SaveChanges();
    }

    public void RemoveBooking(Booking booking)
    {
        _db.Bookings.Remove(booking);
        _db.SaveChanges();
    }

    public List<Treatment> GetAllTreatments()
    {
        return _db.Treatments.OrderBy(t => t.Id).ToList();
    }

    public Treatment? GetTreatmentById(int id)
    {
        return _db.Treatments.FirstOrDefault(t => t.Id == id);
    }

    public void AddSpaBooking(SpaBooking spaBooking)
    {
        _db.SpaBookings.Add(spaBooking);
        _db.SaveChanges();
    }

    public Room? GetRoomByNumber(int roomNumber)
    {
        // Include behövs så att rumstypen följer med när bokningen bekräftas.
        return _db.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefault(r => r.RoomNumber == roomNumber);
    }

    public Guest? GetGuestByName(string name)
    {
        // Används för att återanvända en gäst som redan finns, så att ingen sparas två gånger.
        return _db.Guests.FirstOrDefault(g => g.Name == name);
    }

    public Booking? GetBookingById(int id)
    {
        return _db.Bookings.FirstOrDefault(b => b.Id == id);
    }

    public bool IsRoomBooked(int roomId, DateOnly from, DateOnly to)
    {
        // Två perioder överlappar om varje periods start ligger före den andras slut.
        // Med < och inte <= kan en ny gäst checka in samma dag som en annan checkar ut.
        return _db.Bookings.Any(b =>
            b.RoomId == roomId && b.CheckIn < to && from < b.CheckOut);
    }

    public List<Room> GetAvailableRooms(DateOnly from, DateOnly to)
    {
        // Ett rum är ledigt om ingen bokning på rummet överlappar perioden.
        return _db.Rooms
            .Include(r => r.RoomType)
            .Where(r => !_db.Bookings.Any(b =>
                b.RoomId == r.Id && b.CheckIn < to && from < b.CheckOut))
            .OrderBy(r => r.RoomNumber)
            .ToList();
    }


}
