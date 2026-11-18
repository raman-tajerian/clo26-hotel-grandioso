namespace HotelGrandioso.Models;

public class SpaBooking
{
    public int Id { get; set; }

    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;

    public int TreatmentId { get; set; }
    public Treatment Treatment { get; set; } = null!;

    public DateTime Date { get; set; }   // DateTime eftersom klockslaget spelar roll för en behandling
}