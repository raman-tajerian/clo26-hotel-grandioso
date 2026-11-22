namespace HotelGrandioso.Models;

// Rumstyp är en egen entitet i stället för en enum, så att namn och beskrivning
// ligger i databasen och delas av många rum.
public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public List<Room> Rooms { get; set; } = new();
}

