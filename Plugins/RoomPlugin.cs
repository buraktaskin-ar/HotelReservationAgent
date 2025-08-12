using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;

namespace HotelReservationAgentChatBot.Plugins;

public class RoomPlugin
{
    private readonly IReservationService _reservationService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public RoomPlugin(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [KernelFunction, Description("Get all available rooms in the system with their details and availability status")]
    public async Task<string> GetAllRooms()
    {
        try
        {
            var rooms = await _reservationService.GetAllRoomsAsync();

            if (!rooms.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    message = "No rooms found in the system.",
                    totalRooms = 0,
                    rooms = new List<object>()
                }, JsonOptions);
            }

            var roomsList = rooms.Select(r => new
            {
                roomId = r.Id,
                roomNumber = r.RoomNumber,
                floor = r.Floor,
                hotel = new
                {
                    id = r.Hotel.HotelId,
                    name = r.Hotel.HotelName,
                    city = r.Hotel.City,
                    country = r.Hotel.Country,
                    starRating = r.Hotel.StarRating
                },
                capacity = r.Capacity,
                features = new
                {
                    isSeaView = r.IsSeaView,
                    isCityView = r.IsCityView
                },
                pricing = new
                {
                    basePrice = r.BasePrice,
                    seaViewSurcharge = r.SeaViewSurcharge,
                    cityViewSurcharge = r.CityViewSurcharge,
                    totalPrice = r.TotalPrice
                },
                currentAvailability = r.Availabilities.Select(a => new
                {
                    startDate = a.AvailabilitySlot.Start.ToString("yyyy-MM-dd"),
                    endDate = a.AvailabilitySlot.End.ToString("yyyy-MM-dd"),
                    status = a.AvailabilitySlot.Status.ToString(),
                    note = a.AvailabilitySlot.Note
                }).ToList()
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                message = "All rooms retrieved successfully.",
                totalRooms = rooms.Count,
                rooms = roomsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                error = "An error occurred while retrieving rooms.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get detailed information about a specific room by its ID")]
    public async Task<string> GetRoomInfo(
        [Description("The ID of the room to retrieve information for")] int roomId)
    {
        try
        {
            var room = await _reservationService.GetRoomAsync(roomId);

            if (room == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Room with ID {roomId} not found.",
                    roomId = roomId
                }, JsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Room information retrieved successfully.",
                room = new
                {
                    roomId = room.Id,
                    roomNumber = room.RoomNumber,
                    floor = room.Floor,
                    hotel = new
                    {
                        id = room.Hotel.HotelId,
                        name = room.Hotel.HotelName,
                        city = room.Hotel.City,
                        country = room.Hotel.Country,
                        address = room.Hotel.Address,
                        starRating = room.Hotel.StarRating,
                        description = room.Hotel.Description,
                        amenities = room.Hotel.Amenities
                    },
                    capacity = room.Capacity,
                    features = new
                    {
                        isSeaView = room.IsSeaView,
                        isCityView = room.IsCityView
                    },
                    pricing = new
                    {
                        basePrice = room.BasePrice,
                        seaViewSurcharge = room.SeaViewSurcharge,
                        cityViewSurcharge = room.CityViewSurcharge,
                        totalPricePerNight = room.TotalPrice,
                        currency = "USD"
                    },
                    availability = room.Availabilities.Select(a => new
                    {
                        startDate = a.AvailabilitySlot.Start.ToString("yyyy-MM-dd"),
                        endDate = a.AvailabilitySlot.End.ToString("yyyy-MM-dd"),
                        status = a.AvailabilitySlot.Status.ToString(),
                        note = a.AvailabilitySlot.Note,
                        daysCount = (a.AvailabilitySlot.End - a.AvailabilitySlot.Start).Days + 1
                    }).ToList()
                }
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while retrieving room information.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    
    [KernelFunction, Description("Check if a specific room is available for given dates")]
    public async Task<string> CheckRoomAvailability(
        [Description("The ID of the room to check availability for")] int roomId,
        [Description("Check-in date in various formats (YYYY-MM-DD, DD-MM-YYYY, DD/MM/YYYY, or natural language like '12 January 2025')")] string checkInDate,
        [Description("Check-out date in various formats (YYYY-MM-DD, DD-MM-YYYY, DD/MM/YYYY, or natural language like '16 January 2025')")] string checkOutDate)
    {
        try
        {
            // Gelişmiş tarih parse işlemi
            var checkIn = ParseDate(checkInDate);
            var checkOut = ParseDate(checkOutDate);

            if (!checkIn.HasValue)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid check-in date format. Please provide date in DD-MM-YYYY, YYYY-MM-DD, DD/MM/YYYY format, or natural language like '12 January 2025'.",
                    providedDate = checkInDate
                }, JsonOptions);
            }

            if (!checkOut.HasValue)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid check-out date format. Please provide date in DD-MM-YYYY, YYYY-MM-DD, DD/MM/YYYY format, or natural language like '16 January 2025'.",
                    providedDate = checkOutDate
                }, JsonOptions);
            }

            // Tarih mantık kontrolü
            if (checkIn.Value >= checkOut.Value)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Check-out date must be after check-in date.",
                    checkIn = checkIn.Value.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.Value.ToString("yyyy-MM-dd")
                }, JsonOptions);
            }

            if (checkIn.Value < DateTime.Today)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Check-in date cannot be in the past.",
                    checkIn = checkIn.Value.ToString("yyyy-MM-dd"),
                    today = DateTime.Today.ToString("yyyy-MM-dd")
                }, JsonOptions);
            }

            // Oda var mı kontrol et
            var room = await _reservationService.GetRoomAsync(roomId);
            if (room == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Room with ID {roomId} not found.",
                    roomId = roomId
                }, JsonOptions);
            }

            // Müsaitlik kontrolü
            var isAvailable = await _reservationService.IsRoomAvailableAsync(roomId, checkIn.Value, checkOut.Value);
            var nights = (checkOut.Value - checkIn.Value).Days;
            var totalPrice = room.TotalPrice * nights;

            return JsonSerializer.Serialize(new
            {
                success = true,
                roomId = roomId,
                roomNumber = room.RoomNumber,
                hotel = new
                {
                    name = room.Hotel.HotelName,
                    city = room.Hotel.City
                },
                requestedDates = new
                {
                    checkIn = checkIn.Value.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.Value.ToString("yyyy-MM-dd"),
                    nights = nights
                },
                isAvailable = isAvailable,
                pricing = isAvailable ? new
                {
                    pricePerNight = room.TotalPrice,
                    totalPrice = totalPrice,
                    currency = "USD"
                } : null,
                message = isAvailable
                    ? "Room is available for the requested dates."
                    : "Room is not available for the requested dates. It might be reserved, blocked, or out of service during this period."
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while checking room availability.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    private DateTime? ParseDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        dateString = dateString.Trim();

        // Önce standart parse işlemi dene
        if (DateTime.TryParse(dateString, out var result))
            return result;

        // Türkçe ay isimleri için manuel parsing
        var turkishMonths = new Dictionary<string, int>
        {
            {"ocak", 1}, {"şubat", 2}, {"mart", 3}, {"nisan", 4},
            {"mayıs", 5}, {"haziran", 6}, {"temmuz", 7}, {"ağustos", 8},
            {"eylül", 9}, {"ekim", 10}, {"kasım", 11}, {"aralık", 12},
            {"january", 1}, {"february", 2}, {"march", 3}, {"april", 4},
            {"may", 5}, {"june", 6}, {"july", 7}, {"august", 8},
            {"september", 9}, {"october", 10}, {"november", 11}, {"december", 12}
        };

        // "12-16 ocak 2025" formatını parse et
        if (dateString.Contains("-") && dateString.Split(' ').Length >= 3)
        {
            var parts = dateString.Split(' ');
            if (parts.Length >= 3)
            {
                var dayRange = parts[0].Split('-');
                if (dayRange.Length == 2 &&
                    int.TryParse(dayRange[0], out var startDay) &&
                    turkishMonths.ContainsKey(parts[1].ToLower()) &&
                    int.TryParse(parts[2], out var year))
                {
                    var month = turkishMonths[parts[1].ToLower()];
                    return new DateTime(year, month, startDay);
                }
            }
        }

        // DD-MM-YYYY veya DD/MM/YYYY formatları
        var formats = new[] { "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out result))
                return result;
        }

        return null;
    }
}