namespace HotelGrandioso.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int Floor { get; set; }
        public int SizeM2 { get; set; }
        public decimal PricePerNight { get; set; }

        // Nyckel sparas i databasen, objektet en genväg i koden
        public int RoomTypeId { get; set; } 
        public RoomType RoomType { get; set; } = null!;


        public List<Booking> Bookings { get; set; } = new();

    }
}
