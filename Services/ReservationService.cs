using HotelReservationAgentChatBot.Models;
using HotelReservationAgentChatBot.Data;

namespace HotelReservationAgentChatBot.Services;

public interface IReservationService
{
    Task<Reservation> CreateReservationAsync(int personId, string hotelId, int roomId, DateTime checkIn, DateTime checkOut);
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomAsync(int roomId);
    void AddPerson(Person person);
    Person? GetPerson(int personId);
}

public class ReservationService : IReservationService
{
    private readonly List<Reservation> _reservations;
    private readonly List<Room> _rooms;
    private readonly List<Hotel> _hotels;
    private readonly List<Person> _people;
    private int _nextReservationId = 6; 

    public ReservationService()
    {
        // Seed verileri yükle
        var roomSeeder = new RoomDataSeeder();
        var hotelSeeder = new HotelDataSeeder();
        var personSeeder = new PersonDataSeeder();

        _rooms = roomSeeder.GetRooms();
        _hotels = hotelSeeder.GetHotels().ToList();
        _people = personSeeder.GetPersons();

        // Rezervasyonları yükle
        var reservationSeeder = new ReservationDataSeeder();
        _reservations = reservationSeeder.GetReservations(_people, _rooms);
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

        // Oda müsaitliğini güncelle
        await UpdateRoomAvailabilityAsync(roomId, checkIn, checkOut, person);

        return reservation;
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await Task.FromResult(_reservations.ToList());
    }

    public async Task<List<Room>> GetAllRoomsAsync()
    {
        return await Task.FromResult(_rooms.ToList());
    }

    public async Task<Room?> GetRoomAsync(int roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        return await Task.FromResult(room);
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

        // İstenen tarih aralığını kapsayan slot'ları kontrol et
        foreach (var availability in room.Availabilities)
        {
            var slot = availability.AvailabilitySlot;

            // Tarih aralığının tamamen slot içinde olup olmadığını kontrol et
            if (slot.Start <= checkIn && slot.End >= checkOut)
            {
                // Sadece Available durumundaki slotlar rezerve edilebilir
                return slot.Status == AvailabilityStatus.Available;
            }
        }

        // Tarih aralığını kapsayan uygun slot bulunamadı
        return false;
    }

    private async Task UpdateRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut, Person person)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null) return;

        // Mevcut Available slot'u bul
        var availableSlot = room.Availabilities.FirstOrDefault(a =>
            a.AvailabilitySlot.Status == AvailabilityStatus.Available &&
            a.AvailabilitySlot.Start <= checkIn &&
            a.AvailabilitySlot.End >= checkOut);

        if (availableSlot != null)
        {
            var originalEnd = availableSlot.AvailabilitySlot.End;
            var originalStart = availableSlot.AvailabilitySlot.Start;

            // Mevcut slot'u güncelle veya böl
            if (originalStart == checkIn && originalEnd == checkOut)
            {
                // Tam eşleşme - slot'u reserved yap
                availableSlot.AvailabilitySlot.Status = AvailabilityStatus.Reserved;
                availableSlot.AvailabilitySlot.Note = $"Reserved for {person.FirstName} {person.LastName}";
            }
            else
            {
                // Slot'u bölmek gerekiyor
                room.Availabilities.Remove(availableSlot);

                // Önce kalan available kısımları ekle
                if (originalStart < checkIn)
                {
                    room.Availabilities.Add(new RoomAvailability
                    {
                        Id = room.Availabilities.Max(a => a.Id ?? 0) + 1,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = originalStart,
                            End = checkIn.AddDays(-1),
                            Status = AvailabilityStatus.Available,
                            Note = null
                        }
                    });
                }

                if (originalEnd > checkOut)
                {
                    room.Availabilities.Add(new RoomAvailability
                    {
                        Id = room.Availabilities.Max(a => a.Id ?? 0) + 1,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = checkOut.AddDays(1),
                            End = originalEnd,
                            Status = AvailabilityStatus.Available,
                            Note = null
                        }
                    });
                }

                // Sonra reserved kısmı ekle
                room.Availabilities.Add(new RoomAvailability
                {
                    Id = room.Availabilities.Max(a => a.Id ?? 0) + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = checkIn,
                        End = checkOut,
                        Status = AvailabilityStatus.Reserved,
                        Note = $"Reserved for {person.FirstName} {person.LastName}"
                    }
                });
            }
        }

        await Task.CompletedTask;
    }

    // PersonService için kişileri kaydetmek
    public void AddPerson(Person person)
    {
        if (_people.All(p => p.Id != person.Id))
        {
            _people.Add(person);
        }
    }

    public Person? GetPerson(int personId)
    {
        return _people.FirstOrDefault(p => p.Id == personId);
    }
}