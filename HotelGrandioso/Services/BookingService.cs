using HotelGrandioso.Models;

namespace HotelGrandioso.Services;

// Reglerna bor här så att menyn slipper veta något om databasen, och repositoryt slipper veta något om reglerna.
public class BookingService
{
    private readonly HotelRepository _repository;

    public BookingService(HotelRepository repository)
    {
        _repository = repository;
    }

    public List<Room> GetAllRooms()
    {
        return _repository.GetAllRooms();
    }

    public List<Room> GetAvailableRooms(DateOnly from, DateOnly to)
    {
        ValidateDates(from, to);
        return _repository.GetAvailableRooms(from, to);
    }

    public List<Booking> GetAllBookings()
    {
        return _repository.GetAllBookings();
    }

    public decimal CalculateTotalPrice(Booking booking)
    {
        var nights = booking.CheckOut.DayNumber - booking.CheckIn.DayNumber;
        return nights * booking.Room.PricePerNight;
    }

    public Booking BookRoom(string guestName, int roomNumber, DateOnly from, DateOnly to)
    {
        ValidateDates(from, to);
        ValidateName(guestName);

        var room = _repository.GetRoomByNumber(roomNumber);
        if (room == null)
            throw new InvalidOperationException($"Rum {roomNumber} finns inte.");

        // Kontrollen görs här och inte bara i menyn, så att dubbelbokning är omöjlig oavsett hur bokningen kommer in
 
        if (_repository.IsRoomBooked(room.Id, from, to))
            throw new InvalidOperationException($"Rum {roomNumber} är redan bokat under den perioden.");

        var booking = new Booking
        {
            Guest = GetOrCreateGuest(guestName),
            Room = room,
            CheckIn = from,
            CheckOut = to
        };
        _repository.AddBooking(booking);
        return booking;
    }

    public void CancelBooking(int bookingId)
    {
        var booking = _repository.GetBookingById(bookingId);
        if (booking == null)
            throw new InvalidOperationException($"Bokning {bookingId} finns inte.");

        _repository.RemoveBooking(booking);
    }

    public List<Treatment> GetTreatments()
    {
        return _repository.GetAllTreatments();
    }

    public SpaBooking BookSpa(string guestName, int treatmentId, DateTime date)
    {
        ValidateName(guestName);

        if (date < DateTime.Now)
            throw new ArgumentException("Tiden kan inte vara i det förflutna.");

        var treatment = _repository.GetTreatmentById(treatmentId);
        if (treatment == null)
            throw new InvalidOperationException($"Behandling {treatmentId} finns inte.");

        var spaBooking = new SpaBooking
        {
            Guest = GetOrCreateGuest(guestName),
            Treatment = treatment,
            Date = date
        };
        _repository.AddSpaBooking(spaBooking);
        return spaBooking;
    }

    // En återkommande gäst återanvänds, så att samma person inte sparas flera gånger.
    private Guest GetOrCreateGuest(string name)
    {
        var guest = _repository.GetGuestByName(name.Trim());
        if (guest == null)
            guest = new Guest { Name = name.Trim() };
        return guest;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Namnet måste fyllas i.");
    }

    private static void ValidateDates(DateOnly from, DateOnly to)
    {
        if (from < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Incheckning kan inte vara i det förflutna.");
        if (to <= from)
            throw new ArgumentException("Utcheckning måste vara efter incheckning.");
    }
}