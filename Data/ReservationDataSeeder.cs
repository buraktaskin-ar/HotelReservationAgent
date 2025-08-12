using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class ReservationDataSeeder
{
    public List<Reservation> GetReservations(List<Person> persons, List<Room> rooms)
    {
        var reservations = new List<Reservation>();

        // Rezervasyon 1: Ahmet Yılmaz - Grand Plaza Hotel Room 101
        reservations.Add(new Reservation
        {
            Id = 1,
            Person = persons.First(p => p.Id == 1), // Ahmet Yılmaz
            Hotel = rooms.First(r => r.Id == 1).Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Id == 1), // Room 101
            CheckIn = DateTime.Today.AddDays(7),
            CheckOut = DateTime.Today.AddDays(10),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Id == 1), 3)
        });

        // Rezervasyon 2: Emily Johnson - Grand Plaza Hotel Room 301
        reservations.Add(new Reservation
        {
            Id = 2,
            Person = persons.First(p => p.Id == 6), // Emily Johnson
            Hotel = rooms.First(r => r.Id == 2).Hotel, // Grand Plaza Hotel
            Room = rooms.First(r => r.Id == 2), // Room 301
            CheckIn = DateTime.Today.AddDays(3),
            CheckOut = DateTime.Today.AddDays(7),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Id == 2), 4)
        });

        // Rezervasyon 3: Fatma Kaya - Seaside Resort Room 205
        reservations.Add(new Reservation
        {
            Id = 3,
            Person = persons.First(p => p.Id == 2), // Fatma Kaya
            Hotel = rooms.First(r => r.Id == 4).Hotel, // Seaside Resort
            Room = rooms.First(r => r.Id == 4), // Room 205
            CheckIn = DateTime.Today.AddDays(15),
            CheckOut = DateTime.Today.AddDays(18),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Id == 4), 3)
        });

        // Rezervasyon 4: Mehmet Demir - Seaside Resort Room 410
        reservations.Add(new Reservation
        {
            Id = 4,
            Person = persons.First(p => p.Id == 3), // Mehmet Demir
            Hotel = rooms.First(r => r.Id == 5).Hotel, // Seaside Resort
            Room = rooms.First(r => r.Id == 5), // Room 410
            CheckIn = DateTime.Today.AddDays(20),
            CheckOut = DateTime.Today.AddDays(25),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Id == 5), 5)
        });

        // Rezervasyon 5: Can Şen - Mountain Lodge Room 102
        reservations.Add(new Reservation
        {
            Id = 5,
            Person = persons.First(p => p.Id == 5), // Can Şen
            Hotel = rooms.First(r => r.Id == 6).Hotel, // Mountain Lodge
            Room = rooms.First(r => r.Id == 6), // Room 102
            CheckIn = DateTime.Today.AddDays(12),
            CheckOut = DateTime.Today.AddDays(16),
            TotalPrice = CalculateTotalPrice(rooms.First(r => r.Id == 6), 4)
        });

        return reservations;
    }

    private decimal CalculateTotalPrice(Room room, int numberOfNights)
    {
        return room.TotalPrice * numberOfNights;
    }
}