namespace HotelGrandioso.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }

        public List<Booking> Bookings { get; set; } = new();

    }
}
