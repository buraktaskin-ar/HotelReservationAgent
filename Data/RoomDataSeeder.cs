using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class RoomDataSeeder
{
    private static readonly Random _random = new Random();

    public List<Room> GetRooms()
    {
        var hotelSeeder = new HotelDataSeeder();
        var hotels = hotelSeeder.GetHotels().ToList();
        var rooms = new List<Room>();
        int roomId = 1;

        foreach (var hotel in hotels)
        {
            // Grand Plaza Hotel (Hotel ID: 1) - 30 rooms
            if (hotel.HotelId == "1")
            {
                rooms.AddRange(CreateRoomsForHotel(hotel, 30, ref roomId, hasSeaView: false, hasCityView: true));
            }
            // Seaside Resort (Hotel ID: 2) - 40 rooms  
            else if (hotel.HotelId == "2")
            {
                rooms.AddRange(CreateRoomsForHotel(hotel, 40, ref roomId, hasSeaView: true, hasCityView: false));
            }
            // Other hotels - 25 rooms each
            else
            {
                rooms.AddRange(CreateRoomsForHotel(hotel, 25, ref roomId, hasSeaView: false, hasCityView: false));
            }
        }

        return rooms;
    }

    private List<Room> CreateRoomsForHotel(Hotel hotel, int roomCount, ref int roomId, bool hasSeaView, bool hasCityView)
    {
        var rooms = new List<Room>();
        var roomTypes = new[] { RoomType.StandardSingle, RoomType.StandardDouble, RoomType.DeluxeSingle, RoomType.DeluxeDouble };

        for (int i = 1; i <= roomCount; i++)
        {
            var floor = (i - 1) / 10 + 1;
            var roomNumber = $"{floor}{(i % 10 == 0 ? 10 : i % 10).ToString("00")}";
            var roomType = roomTypes[_random.Next(roomTypes.Length)];

            // Sea view probability for seaside hotels
            var isSeaView = hasSeaView && _random.NextDouble() < 0.6; // 60% chance for seaside hotels

            // City view probability for city hotels
            var isCityView = hasCityView && _random.NextDouble() < 0.7; // 70% chance for city hotels

            var room = new Room
            {
                Id = roomId++,
                RoomNumber = roomNumber,
                Floor = floor,
                Hotel = hotel,
                Capacity = RoomPricing.Capacities[roomType],
                IsSeaView = isSeaView,
                IsCityView = isCityView,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[roomType],
                Availabilities = GenerateRoomAvailabilities(roomId - 1)
            };

            rooms.Add(room);
        }

        return rooms;
    }

    private List<RoomAvailability> GenerateRoomAvailabilities(int roomId)
    {
        var availabilities = new List<RoomAvailability>();
        var startDate = DateTime.Today;
        int availabilityId = roomId * 1000;

        // Generate availability for next 6 months
        for (int month = 0; month < 6; month++)
        {
            var monthStart = startDate.AddMonths(month);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var currentDate = monthStart;
            while (currentDate <= monthEnd)
            {
                var status = GenerateRandomAvailabilityStatus();
                var slotDuration = _random.Next(1, 8); // 1-7 days
                var slotEnd = currentDate.AddDays(slotDuration);

                if (slotEnd > monthEnd)
                    slotEnd = monthEnd;

                availabilities.Add(new RoomAvailability
                {
                    Id = availabilityId++,
                    Room = null, // Will be set when room is assigned
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = currentDate,
                        End = slotEnd,
                        Status = status,
                        Note = GetAvailabilityNote(status)
                    }
                });

                currentDate = slotEnd.AddDays(1);
            }
        }

        return availabilities;
    }

    private AvailabilityStatus GenerateRandomAvailabilityStatus()
    {
        var rand = _random.NextDouble();
        return rand switch
        {
            < 0.75 => AvailabilityStatus.Available,   // 75% available
            < 0.88 => AvailabilityStatus.Reserved,    // 13% reserved
            < 0.96 => AvailabilityStatus.Blocked,     // 8% blocked
            _ => AvailabilityStatus.OutOfService      // 4% out of service
        };
    }

    private string GetAvailabilityNote(AvailabilityStatus status)
    {
        return status switch
        {
            AvailabilityStatus.Reserved => "Mevcut rezervasyon",
            AvailabilityStatus.Blocked => "Bakım çalışması",
            AvailabilityStatus.OutOfService => "Renovasyon",
            _ => null
        };
    }
}