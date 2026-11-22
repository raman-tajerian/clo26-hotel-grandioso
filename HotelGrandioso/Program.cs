using HotelGrandioso.Data;
using HotelGrandioso.Services;
using HotelGrandioso.UI;
using Microsoft.EntityFrameworkCore;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

using var db = new HotelContext();

// // Migrate skapar databasen första gången, så att den som klonar repot slipper göra det själv.
db.Database.Migrate();

var repository = new HotelRepository(db);
var service = new BookingService(repository);

new Menu(service).Run();