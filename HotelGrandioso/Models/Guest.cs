namespace HotelGrandioso.Models;

public class Guest
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<Booking> Bookings { get; set; } = new();
    public List<SpaBooking> SpaBookings { get; set; } = new();
}
