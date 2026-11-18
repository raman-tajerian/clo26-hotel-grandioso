namespace HotelGrandioso.Models;

public class Treatment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public List<SpaBooking> SpaBookings { get; set; } = new();
}