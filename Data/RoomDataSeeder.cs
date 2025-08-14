using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Data;

public class RoomDataSeeder
{
    public List<Room> GetRooms()
    {
        var hotelSeeder = new HotelDataSeeder();
        var hotels = hotelSeeder.GetHotels().ToList();
        var rooms = new List<Room>();

        // Hotel 1 için odalar (Grand Plaza)
        var hotel1 = hotels.First(h => h.HotelId == "1");
        rooms.AddRange(CreateRoomsForHotel1(hotel1));

        // Hotel 2 için odalar (Seaside Resort)
        var hotel2 = hotels.First(h => h.HotelId == "2");
        rooms.AddRange(CreateRoomsForHotel2(hotel2));

        // Hotel 3 için odalar (Mountain Lodge)
        var hotel3 = hotels.First(h => h.HotelId == "3");
        rooms.AddRange(CreateRoomsForHotel3(hotel3));

        // YENİ: Kids Paradise Family Resort için odalar
        var kidsParadiseHotel = hotels.First(h => h.HotelId == "2");
        rooms.AddRange(CreateRoomsForKidsParadise(kidsParadiseHotel));

        return rooms;
    }

    private List<Room> CreateRoomsForKidsParadise(Hotel hotel)
    {
        return new List<Room>
        {
            // İSTEDİĞİNİZ 304 NUMARALI 3 KİŞİLİK ODA
            new Room
            {
                Id = 304,
                RoomNumber = "304",
                Floor = 3,
                Hotel = hotel,
                Capacity = 3, // 3 KİŞİLİK
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = 220.00m, // Aile dostu fiyat
                Availabilities = new List<RoomAvailability>
                {
                    new RoomAvailability
                    {
                        Id = 30401,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = DateTime.Today,
                            End = DateTime.Today.AddMonths(12), // 1 yıl müsait
                            Status = AvailabilityStatus.Available,
                            Note = "3 kişilik aile odası - deniz manzaralı"
                        }
                    }
                }
            },
            // Diğer odalar
            new Room
            {
                Id = 305,
                RoomNumber = "305",
                Floor = 3,
                Hotel = hotel,
                Capacity = 4,
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = 250.00m,
                Availabilities = new List<RoomAvailability>
                {
                    new RoomAvailability
                    {
                        Id = 30501,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = DateTime.Today,
                            End = DateTime.Today.AddMonths(12),
                            Status = AvailabilityStatus.Available,
                            Note = "4 kişilik aile odası"
                        }
                    }
                }
            },
            new Room
            {
                Id = 306,
                RoomNumber = "205",
                Floor = 2,
                Hotel = hotel,
                Capacity = 2,
                IsSeaView = true,
                IsCityView = false,
                SeaViewSurcharge = RoomPricing.SeaViewSurcharge,
                CityViewSurcharge = RoomPricing.CityViewSurcharge,
                BasePrice = 180.00m,
                Availabilities = new List<RoomAvailability>
                {
                    new RoomAvailability
                    {
                        Id = 30601,
                        AvailabilitySlot = new AvailabilitySlot
                        {
                            Start = DateTime.Today,
                            End = DateTime.Today.AddMonths(12),
                            Status = AvailabilityStatus.Available,
                            Note = "2 kişilik oda"
                        }
                    }
                }
            }
        };
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
                Capacity = 2,
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
                Capacity = 1,
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
                Capacity = 2,
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
                Capacity = 2,
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
                Capacity = 1,
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
                Floor = 3,
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

        // Tüm odalar için basit müsaitlik
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

        return availabilities;
    }
}