using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class ReservationDataSeeder
{
    private static readonly Random _random = new Random();

    public List<Reservation> GetReservations(List<Person> persons, List<Room> rooms)
    {
        var reservations = new List<Reservation>();

        // Create some sample reservations
        reservations.Add(new Reservation
        {
            Id = 1,
            Person = persons.First(p => p.Id == 1), // Ahmet Yılmaz
            Hotel = rooms.First(r => r.Hotel.HotelId == "1").Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "101"),
            CheckIn = DateTime.Today.AddDays(7),
            CheckOut = DateTime.Today.AddDays(10),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "101"), 3)
        });

        reservations.Add(new Reservation
        {
            Id = 2,
            Person = persons.First(p => p.Id == 2), // Fatma Kaya
            Hotel = rooms.First(r => r.Hotel.HotelId == "2").Hotel, // Seaside Resort
            Room = rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "205"),
            CheckIn = DateTime.Today.AddDays(15),
            CheckOut = DateTime.Today.AddDays(18),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "205"), 3)
        });

        reservations.Add(new Reservation
        {
            Id = 3,
            Person = persons.First(p => p.Id == 6), // Emily Johnson
            Hotel = rooms.First(r => r.Hotel.HotelId == "1").Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "301"),
            CheckIn = DateTime.Today.AddDays(3),
            CheckOut = DateTime.Today.AddDays(7),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "301"), 4)
        });

        reservations.Add(new Reservation
        {
            Id = 4,
            Person = persons.First(p => p.Id == 3), // Mehmet Demir
            Hotel = rooms.First(r => r.Hotel.HotelId == "2").Hotel, // Seaside Resort
            Room = rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "410"),
            CheckIn = DateTime.Today.AddDays(20),
            CheckOut = DateTime.Today.AddDays(25),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "410"), 5)
        });

        reservations.Add(new Reservation
        {
            Id = 5,
            Person = persons.First(p => p.Id == 5), // Can Şen
            Hotel = rooms.First(r => r.Hotel.HotelId == "3").Hotel,
            Room = rooms.First(r => r.Hotel.HotelId == "3" && r.RoomNumber == "102"),
            CheckIn = DateTime.Today.AddDays(12),
            CheckOut = DateTime.Today.AddDays(16),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "3" && r.RoomNumber == "102"), 4)
        });

        reservations.Add(new Reservation
        {
            Id = 6,
            Person = persons.First(p => p.Id == 7), // Marco Rossi
            Hotel = rooms.First(r => r.Hotel.HotelId == "1").Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "505"),
            CheckIn = DateTime.Today.AddDays(30),
            CheckOut = DateTime.Today.AddDays(35),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "505"), 5)
        });

        // Add some past reservations
        reservations.Add(new Reservation
        {
            Id = 7,
            Person = persons.First(p => p.Id == 8), // Sophie Mueller
            Hotel = rooms.First(r => r.Hotel.HotelId == "2").Hotel, // Seaside Resort
            Room = rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "315"),
            CheckIn = DateTime.Today.AddDays(-10),
            CheckOut = DateTime.Today.AddDays(-7),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "315"), 3)
        });

        reservations.Add(new Reservation
        {
            Id = 8,
            Person = persons.First(p => p.Id == 4), // Ayşe Öz
            Hotel = rooms.First(r => r.Hotel.HotelId == "4").Hotel,
            Room = rooms.First(r => r.Hotel.HotelId == "4" && r.RoomNumber == "201"),
            CheckIn = DateTime.Today.AddDays(-5),
            CheckOut = DateTime.Today.AddDays(-2),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "4" && r.RoomNumber == "201"), 3)
        });

        // Add current active reservation
        reservations.Add(new Reservation
        {
            Id = 9,
            Person = persons.First(p => p.Id == 9), // Ali Çelik
            Hotel = rooms.First(r => r.Hotel.HotelId == "1").Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "420"),
            CheckIn = DateTime.Today.AddDays(-1),
            CheckOut = DateTime.Today.AddDays(3),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "1" && r.RoomNumber == "420"), 4)
        });

        reservations.Add(new Reservation
        {
            Id = 10,
            Person = persons.First(p => p.Id == 10), // Zeynep Aydın
            Hotel = rooms.First(r => r.Hotel.HotelId == "2").Hotel, // Seaside Resort
            Room = rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "108"),
            CheckIn = DateTime.Today.AddDays(45),
            CheckOut = DateTime.Today.AddDays(52),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Hotel.HotelId == "2" && r.RoomNumber == "108"), 7)
        });

        return reservations;
    }

    private decimal CalculateTotalPrice(Room room, int numberOfNights)
    {
        return room.TotalPrice * numberOfNights;
    }
}