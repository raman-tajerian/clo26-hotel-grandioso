using HotelGrandioso.Models;
using HotelGrandioso.Services;

namespace HotelGrandioso.UI;

// Menyn visar och läser in. Den vet inget om databasen eller reglerna, de ligger i BookingService.
public class Menu
{
    private readonly BookingService _service;
    public Menu(BookingService bookingService)
    {
        _service = bookingService;
    }
    public void Run()
    {
        Console.WriteLine("Välkommen till Hotel Grandioso!");

        var running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("1. Boka rum");
            Console.WriteLine("2. Avboka");
            Console.WriteLine("3. Visa bokningar");
            Console.WriteLine("4. Visa lediga rum");
            Console.WriteLine("5. Boka spa");
            Console.WriteLine("6. Visa alla rum");
            Console.WriteLine("7. Avsluta");
            Console.Write("Val: ");

            // Regelbrott från servicen visas som ett meddelande, så att programmet inte kraschar.
            try
            {
                switch (Console.ReadLine()?.Trim())
                {
                    case "1": BookRoom(); break;
                    case "2": CancelBooking(); break;
                    case "3": ShowBookings(); break;
                    case "4": ShowAvailableRooms(); break;
                    case "5": BookSpa(); break;
                    case "6": PrintRooms(_service.GetAllRooms()); break;
                    case "7": running = false; break;
                    default: Console.WriteLine("Ogiltigt val, försök igen."); break;
                }
            }

            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                Console.WriteLine($"Fel: {ex.Message}");
            }
        }

        Console.WriteLine("Välkommen åter!");
    }

    private void BookRoom()
    {
        Console.Write("Gästnamn: ");
        var name = Console.ReadLine() ?? "";
        var from = ReadDate("Incheckning (YYYY-MM-DD): ");
        var to = ReadDate("Utcheckning (YYYY-MM-DD): ");


        var available = _service.GetAvailableRooms(from, to);
        if (available.Count == 0)
        {
            Console.WriteLine("Inga lediga rum under den perioden.");
            return;
        }

        Console.WriteLine("Välj rum:");
        PrintRooms(available);
        var roomNumber = ReadInt("Rumsnummer: ");

        var booking = _service.BookRoom(name, roomNumber, from, to);
        var nights = to.DayNumber - from.DayNumber;
        Console.WriteLine($"Bokning bekräftad! Totalt: {_service.CalculateTotalPrice(booking):N0} kr " +
                          $"({nights} nätter × {booking.Room.PricePerNight:N0} kr)");
    }

    private void CancelBooking()
    {
        ShowBookings();
        var id = ReadInt("Bokningsnummer att avboka: ");
        _service.CancelBooking(id);
        Console.WriteLine("Bokningen är avbokad.");
    }

    private void ShowBookings()
    {
        var bookings = _service.GetAllBookings();
        if (bookings.Count == 0)
        {
            Console.WriteLine("Det finns inga bokningar.");
            return;
        }

        Console.WriteLine("Nr   Rum  Gäst                 In          Ut          Totalt");
        foreach (var b in bookings)
        {
            Console.WriteLine($"{b.Id,-4} {b.Room.RoomNumber,-4} {b.Guest.Name,-20} " +
                              $"{b.CheckIn:yyyy-MM-dd}  {b.CheckOut:yyyy-MM-dd}  {_service.CalculateTotalPrice(b):N0} kr");
        }
    }

    private void ShowAvailableRooms()
    {
        var from = ReadDate("Från (YYYY-MM-DD): ");
        var to = ReadDate("Till (YYYY-MM-DD): ");

        var rooms = _service.GetAvailableRooms(from, to);
        if (rooms.Count == 0)
            Console.WriteLine("Inga lediga rum under den perioden.");
        else
            PrintRooms(rooms);
    }

    private void BookSpa()
    {
        Console.Write("Gästnamn: ");
        var name = Console.ReadLine() ?? "";

        Console.WriteLine("Behandlingar:");
        foreach (var t in _service.GetTreatments())
            Console.WriteLine($"  {t.Id} - {t.Name} ({t.DurationMinutes} min) {t.Price:N0} kr");

        var treatmentId = ReadInt("Behandling (nummer): ");
        var date = ReadDateTime("Datum och tid (YYYY-MM-DD HH:mm): ");

        var spaBooking = _service.BookSpa(name, treatmentId, date);
        Console.WriteLine($"Spa bokat: {spaBooking.Treatment.Name} för {spaBooking.Guest.Name}, " +
                          $"{spaBooking.Date:yyyy-MM-dd HH:mm}, {spaBooking.Treatment.Price:N0} kr.");
    }

    private static void PrintRooms(List<Room> rooms)
    {
        foreach (var r in rooms)
        {
            Console.WriteLine($"  {r.RoomNumber,-4} {r.RoomType.Name,-9} {r.SizeM2,4} m²  {r.PricePerNight,6:N0} kr/natt");
        }
    }

    // Läshjälparna frågar om tills inmatningen är giltig, så att programmet inte kraschar.
    private static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateOnly.TryParseExact(Console.ReadLine()?.Trim(), "yyyy-MM-dd", out var date))
                return date;
            Console.WriteLine("Skriv datumet som YYYY-MM-DD, till exempel 2026-12-24.");
        }
    }

    private static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out var date))
                return date;
            Console.WriteLine("Skriv datum och tid som YYYY-MM-DD HH:mm, till exempel 2026-12-24 14:00.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value))
                return value;
            Console.WriteLine("Skriv ett heltal.");
        }
    }
}


