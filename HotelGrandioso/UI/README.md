# Hotel Grandioso — Inlämning 1

**Namn:** Raman Tajerian

Ett konsolprogram för bokning av hotellrum och spabehandlingar. Det går inte att dubbelboka ett rum.

## Så kör du projektet

1. Forka repot på GitHub och klona din fork
2. Öppna `HotelGrandioso.slnx` i Visual Studio (eller öppna mappen i VS Code)
3. Tryck **F5** för att starta

Via terminalen:

```bash
dotnet run --project HotelGrandioso
```

## Databas

- Databas: SQLite (filen `hotel.db` skapas automatiskt i mappen du kör från)
- Migrationer ligger i `HotelGrandioso/Migrations`. Programmet kör `Migrate()` vid start, så databasen skapas av sig själv första gången. Så här skapar och kör du migrationer manuellt:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project HotelGrandioso
dotnet ef database update --project HotelGrandioso
```

Rum, rumstyper och spabehandlingar läggs in som startdata (`HasData`) i `HotelContext`.

## UML-klassdiagram

```mermaid
classDiagram
    class Guest {
        +int Id
        +string Name
    }
    class RoomType {
        +int Id
        +string Name
        +string Description
    }
    class Room {
        +int Id
        +int RoomNumber
        +int Floor
        +int SizeM2
        +decimal PricePerNight
        +int RoomTypeId
    }
    class Booking {
        +int Id
        +int GuestId
        +int RoomId
        +DateOnly CheckIn
        +DateOnly CheckOut
    }
    class Treatment {
        +int Id
        +string Name
        +int DurationMinutes
        +decimal Price
    }
    class SpaBooking {
        +int Id
        +int GuestId
        +int TreatmentId
        +DateTime Date
    }

    RoomType "1" --> "0..*" Room
    Guest "1" --> "0..*" Booking
    Room "1" --> "0..*" Booking
    Guest "1" --> "0..*" SpaBooking
    Treatment "1" --> "0..*" SpaBooking
```

## GDPR

Hotellchefens önskelista för `Guest` innehöll namn, hårfärg, längd och religion. Jag utgår från dataminimering: jag sparar bara det som har ett tydligt syfte i systemet.

| Fält | Beslut | Motivering |
|------|--------|------------|
| `Name` | Behålls | Hotellet måste veta vem som har bokat. |
| `HairColor` | Tas bort | Ingen funktion i systemet behöver den, så det finns inget syfte att spara den. |
| `HeightCm` | Tas bort | Samma skäl. Den har inget med bokning av rum eller spa att göra. |
| `Religion` | Tas bort | Religion är en känslig personuppgift (GDPR artikel 9) som kräver särskilda skäl. En bokning har inget sådant skäl. |

### Mina egna tankar om GDPR

När jag tittade på vilka uppgifter hotellet ville spara funderade jag på vilka som faktiskt behövs för att systemet ska fungera. För att kunna hantera en bokning räcker det att veta gästens namn. Hårfärg och längd tillför inget till själva bokningen och därför valde jag att inte ta med dem.

Jag tycker också att det är viktigt att inte samla in mer information än nödvändigt. Om en databas skulle läcka vill man inte att fler personuppgifter än nödvändigt ska finnas där. Religion är dessutom en känslig personuppgift enligt GDPR artikel 9 och det finns ingen anledning för hotellet att behandla den vid en vanlig rumsbokning.

En sak jag tänkte på är att det kan bli problem om två gäster har samma namn, eftersom systemet använder namnet för att identifiera gästerna. I ett större system hade det behövts ett mer unikt sätt att identifiera varje person. Samtidigt måste även en sådan uppgift ha ett tydligt syfte innan man börjar spara den.

## Designmönster

Jag har använt **Repository-mönstret**. `HotelRepository` är den enda klassen som pratar med databasen (via `HotelContext`). Den hämtar och sparar rum, bokningar, gäster och spabehandlingar, men innehåller inga regler.

Jag valde det för att:

- reglerna (dubbelbokning, prisberäkning, validering) ligger för sig i `BookingService`
- menyn (`Menu`) inte behöver veta något om databasen
- det blir lätt att ändra en fråga eller byta databas på ett ställe

### Sammanfattning

Jag har valt att samla all kontakt med databasen i `HotelRepository`. Där finns metoderna som hämtar och sparar information med hjälp av `HotelContext` och LINQ. Däremot ligger inga bokningsregler där. Till exempel kontrolleras dubbelbokningar och beräknas priser i `BookingService`, som hämtar den information den behöver från repositoryt.

Jag tycker att den här uppdelningen gör koden enklare att följa. `Menu` behöver bara hantera det användaren skriver in och visa resultatet, medan `BookingService` tar hand om reglerna och `HotelRepository` sköter databasen. Om jag senare behöver ändra hur information hämtas från databasen kan jag göra det i repositoryt utan att behöva ändra resten av programmet.

### Tillgänglighetskontrollen (LINQ)

```csharp
_db.Rooms.Where(r => !_db.Bookings.Any(b =>
    b.RoomId == r.Id && b.CheckIn < to && from < b.CheckOut))
```

Ett rum är ledigt om det inte finns någon bokning på rummet som överlappar perioden. Två perioder överlappar om varje periods start ligger före den andras slut. Jag använder `<` och inte `<=`, så att en gäst kan checka in samma dag som en annan checkar ut.

`Any(...)` frågar om det finns minst en bokning som krockar med perioden, och `!` vänder på det. `Where` behåller alltså de rum där ingen krockande bokning finns.

## Tekniker

- C# / .NET 10
- Entity Framework Core 10 (Code First, migrationer) med SQLite
- LINQ
- Mermaid för UML