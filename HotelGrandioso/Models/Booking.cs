using System;
using System.Collections.Generic;
using System.Text;

namespace HotelGrandioso.Models;

public class Booking
{
    public int Id { get; set; }
    public int GuestId { get; set; }
    public Guest Guest { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; } // Räknar ej utcheckningsdag som en natt
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

