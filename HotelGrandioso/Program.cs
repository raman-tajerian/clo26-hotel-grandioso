using HotelGrandioso.Data;
using HotelGrandioso.Services;
using HotelGrandioso.UI;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

using var db = new HotelContext();

// Skapar databasen och kör migrationer
db.Database.Migrate();

var repository = new HotelRepository(db);
var service = new BookingService(repository);

new Menu(service).Run();