using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class RoomDataSeeder
{
    public List<Room> GetRooms()
    {
        var hotelSeeder = new HotelDataSeeder();
        var hotels = hotelSeeder.GetHotels().ToList();
        var rooms = new List<Room>();

        // Hotel 1 için 3 oda
        var hotel1 = hotels.First(h => h.HotelId == "11");
        rooms.AddRange(CreateRoomsForHotel1(hotel1));

        // Hotel 2 için 2 oda
        var hotel2 = hotels.First(h => h.HotelId == "11");
        rooms.AddRange(CreateRoomsForHotel2(hotel2));

        // Hotel 3 için 2 oda
        var hotel3 = hotels.First(h => h.HotelId == "11");
        rooms.AddRange(CreateRoomsForHotel3(hotel3));

        return rooms;
    }

    private List<Room> CreateRoomsForHotel1(Hotel hotel)
    {
        return new List<Room>
        {
            new Room
            {
                Id = 1,
                RoomNumber = "101",
                Floor = 1,
                Hotel = hotel,
                Capacity = 1,
                IsSeaView = false,
                IsCityView = true,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.StandardDouble],
                Availabilities = GenerateRoomAvailabilities(1, DateTime.Today, DateTime.Today.AddMonths(6))
            },
            new Room
            {
                Id = 2,
                RoomNumber = "301",
                Floor = 3,
                Hotel = hotel,
                Capacity = 2,
                IsSeaView = false,
                IsCityView = true,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.DeluxeSingle],
                Availabilities = GenerateRoomAvailabilities(2, DateTime.Today, DateTime.Today.AddMonths(6))
            },
            new Room
            {
                Id = 3,
                RoomNumber = "505",
                Floor = 5,
                Hotel = hotel,
                Capacity = 3,
                IsSeaView = false,
                IsCityView = true,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.DeluxeDouble],
                Availabilities = GenerateRoomAvailabilities(3, DateTime.Today, DateTime.Today.AddMonths(6))
            }
        };
    }

    private List<Room> CreateRoomsForHotel2(Hotel hotel)
    {
        return new List<Room>
        {
            new Room
            {
                Id = 4,
                RoomNumber = "205",
                Floor = 2,
                Hotel = hotel,
                Capacity = 1,
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.StandardDouble],
                Availabilities = GenerateRoomAvailabilities(4, DateTime.Today, DateTime.Today.AddMonths(6))
            },
            new Room
            {
                Id = 5,
                RoomNumber = "410",
                Floor = 4,
                Hotel = hotel,
                Capacity = 2,
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.DeluxeDouble],
                Availabilities = GenerateRoomAvailabilities(5, DateTime.Today, DateTime.Today.AddMonths(6))
            },
            new Room
            {
                Id = 8,
                RoomNumber = "301",
                Floor = 3,
                Hotel = hotel,
                Capacity = 3,
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.StandardSingle],
                Availabilities = GenerateRoomAvailabilities(8, DateTime.Today, DateTime.Today.AddMonths(6))
            }
        };
    }

    private List<Room> CreateRoomsForHotel3(Hotel hotel)
    {
        return new List<Room>
        {
            new Room
            {
                Id = 6,
                RoomNumber = "102",
                Floor = 1,
                Hotel = hotel,
                Capacity = 1,
                IsSeaView = false,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.StandardSingle],
                Availabilities = GenerateRoomAvailabilities(6, DateTime.Today, DateTime.Today.AddMonths(6))
            },
            new Room
            {
                Id = 7,
                RoomNumber = "304",
                Floor = 2,
                Hotel = hotel,
                Capacity = 2,
                IsSeaView = false,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = RoomPricing.BasePrices[RoomType.StandardDouble],
                Availabilities = GenerateRoomAvailabilities(7, DateTime.Today, DateTime.Today.AddMonths(6))
            }
        };
    }

    private List<RoomAvailability> GenerateRoomAvailabilities(int roomId, DateTime startDate, DateTime endDate)
    {
        var availabilities = new List<RoomAvailability>();

        switch (roomId)
        {
            case 1: // Room 101 - Gelecekte rezerve edilmiş
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = startDate.AddDays(6), // İlk 7 gün müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 2,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(7),
                        End = startDate.AddDays(9), // 7-10 arası rezerve
                        Status = AvailabilityStatus.Reserved,
                        Note = "Ahmet Yılmaz rezervasyonu"
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 3,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(10),
                        End = endDate, // Geri kalan dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;

            case 2: // Room 301 - Gelecekte rezerve edilmiş
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = startDate.AddDays(2), // İlk 3 gün müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 2,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(3),
                        End = startDate.AddDays(6), // 3-7 arası rezerve
                        Status = AvailabilityStatus.Reserved,
                        Note = "Emily Johnson rezervasyonu"
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 3,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(7),
                        End = endDate, // Geri kalan dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;

            case 4: // Room 205 - Gelecekte rezerve edilmiş
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = startDate.AddDays(14), // İlk 15 gün müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 2,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(15),
                        End = startDate.AddDays(17), // 15-18 arası rezerve
                        Status = AvailabilityStatus.Reserved,
                        Note = "Fatma Kaya rezervasyonu"
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 3,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(18),
                        End = endDate, // Geri kalan dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;

            case 5: // Room 410 - Gelecekte rezerve edilmiş
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = startDate.AddDays(19), // İlk 20 gün müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 2,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(20),
                        End = startDate.AddDays(24), // 20-25 arası rezerve
                        Status = AvailabilityStatus.Reserved,
                        Note = "Mehmet Demir rezervasyonu"
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 3,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(25),
                        End = endDate, // Geri kalan dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;

            case 6: // Room 102 - Gelecekte rezerve edilmiş
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = startDate.AddDays(11), // İlk 12 gün müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 2,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(12),
                        End = startDate.AddDays(15), // 12-16 arası rezerve
                        Status = AvailabilityStatus.Reserved,
                        Note = "Can Şen rezervasyonu"
                    }
                });
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 3,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate.AddDays(16),
                        End = endDate, // Geri kalan dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;

            case 8: // Room 301 (Seaside Resort) - 1 kişilik oda, genellikle müsait
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = endDate, // Tüm dönem müsait
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;
            default: // Diğer odalar tamamen müsait
                availabilities.Add(new RoomAvailability
                {
                    Id = roomId * 100 + 1,
                    AvailabilitySlot = new AvailabilitySlot
                    {
                        Start = startDate,
                        End = endDate,
                        Status = AvailabilityStatus.Available,
                        Note = null
                    }
                });
                break;
        }

        return availabilities;
    }
}