using HotelReservationAgentChatBot.Models;
using System.Text.Json;

namespace HotelReservationAgentChatBot.Services;

public interface IReservationService
{
    Task<Reservation> CreateReservationAsync(int personId, string hotelId, int roomId, DateTime checkIn, DateTime checkOut);
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
}

public class ReservationService : IReservationService
{
    private readonly List<Reservation> _reservations;
    private readonly List<Room> _rooms;
    private readonly List<Hotel> _hotels;
    private readonly List<Person> _people;
    private int _nextReservationId = 1;

    public ReservationService()
    {
        _reservations = new List<Reservation>();
        _rooms = SeedRooms();
        _hotels = SeedHotels();
        _people = new List<Person>();
    }

    public async Task<Reservation> CreateReservationAsync(int personId, string hotelId, int roomId, DateTime checkIn, DateTime checkOut)
    {
        // Kişiyi bul
        var person = _people.FirstOrDefault(p => p.Id == personId);
        if (person == null)
            throw new InvalidOperationException($"Person with ID {personId} not found.");

        // Oteli bul
        var hotel = _hotels.FirstOrDefault(h => h.HotelId == hotelId);
        if (hotel == null)
            throw new InvalidOperationException($"Hotel with ID {hotelId} not found.");

        // Odayı bul
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
            throw new InvalidOperationException($"Room with ID {roomId} not found.");

        // Oda müsaitlik kontrolü
        if (!await IsRoomAvailableAsync(roomId, checkIn, checkOut))
        {
            throw new InvalidOperationException($"Room {room.RoomNumber} is not available between {checkIn:yyyy-MM-dd} and {checkOut:yyyy-MM-dd}.");
        }

        // Toplam fiyatı hesapla
        var nights = (checkOut - checkIn).Days;
        var totalPrice = room.TotalPrice * nights;

        // Rezervasyonu oluştur
        var reservation = new Reservation
        {
            Id = _nextReservationId++,
            Person = person,
            Hotel = hotel,
            Room = room,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalPrice = totalPrice
        };

        // Rezervasyonu kaydet
        _reservations.Add(reservation);

        // Oda müsaitliğini güncelle (rezerve edildi olarak işaretle)
        var availability = room.Availabilities.FirstOrDefault(a =>
            a.AvailabilitySlot.Start <= checkIn && a.AvailabilitySlot.End >= checkOut);

        if (availability != null)
        {
            availability.AvailabilitySlot.Status = AvailabilityStatus.Reserved;
            availability.AvailabilitySlot.Note = $"Reserved for {person.FirstName} {person.LastName}";
        }

        return reservation;
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await Task.FromResult(_reservations.ToList());
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        await Task.Delay(1); // Async method için

        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null) return false;

        // Eğer hiç müsaitlik tanımlanmamışsa, oda müsait kabul edilir
        if (!room.Availabilities.Any())
        {
            return true;
        }

        // Belirtilen tarih aralığında uygun slot var mı kontrol et
        foreach (var availability in room.Availabilities)
        {
            var slot = availability.AvailabilitySlot;

            // Tarih aralığı kontrolü
            if (slot.Start <= checkIn && slot.End >= checkOut)
            {
                // Sadece Available durumundaki slotlar rezerve edilebilir
                return slot.Status == AvailabilityStatus.Available;
            }
        }

        // Uygun slot bulunamadı
        return false;
    }

    // Test verileri için örnek odalar
    private List<Room> SeedRooms()
    {
        var rooms = new List<Room>();

        for (int i = 1; i <= 10; i++)
        {
            var room = new Room
            {
                Id = i,
                RoomNumber = $"10{i}",
                Floor = (i - 1) / 4 + 1,
                Capacity = i % 2 == 0 ? 2 : 1,
                IsSeaView = i % 3 == 0,
                IsCityView = i % 4 == 0,
                SeaViewSurcharge = 50m,
                CityViewSurcharge = 30m,
                BasePrice = 150m + (i * 25m),
                Availabilities = new List<RoomAvailability>
                {
                    new RoomAvailability
                    {
                        Id = i * 100,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = DateTime.Today,
                            End = DateTime.Today.AddMonths(6),
                            Status = AvailabilityStatus.Available,
                            Note = "Standard availability"
                        }
                    }
                }
            };

            rooms.Add(room);
        }

        return rooms;
    }

    // Test verileri için örnek oteller
    private List<Hotel> SeedHotels()
    {
        return new List<Hotel>
        {
            new Hotel
            {
                HotelId = "hotel-001",
                HotelName = "Grand Istanbul Hotel",
                City = "Istanbul",
                Country = "Turkey",
                Address = "Sultanahmet, Istanbul",
                StarRating = 5,
                PricePerNight = 200,
                Description = "Luxury hotel in the heart of Istanbul",
                Amenities = "Pool, Spa, Gym, Restaurant, Bar",
                HasPool = true,
                HasGym = true,
                HasSpa = true,
                PetFriendly = false,
                HasParking = true,
                HasWifi = true
            }
        };
    }

    // PersonService için kişileri kaydetmek
    public void AddPerson(Person person)
    {
        _people.Add(person);
    }

    public Person? GetPerson(int personId)
    {
        return _people.FirstOrDefault(p => p.Id == personId);
    }
}