using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;

namespace HotelReservationAgentChatBot.Plugins;

public class DateRangePlugin
{
    private readonly IReservationService _reservationService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public DateRangePlugin(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [KernelFunction, Description("Parse date ranges like '12-16 January 2025' and find available rooms for those dates. Use this when user provides date ranges in natural language.")]
    public async Task<string> FindRoomsForDateRange(
        [Description("Date range in natural language like '12-16 January 2025', '5-10 ocak 2025', etc.")] string dateRange,
        [Description("Number of guests (1 for single room, 2 for double room)")] int guestCount = 1,
        [Description("Hotel preference (optional)")] string? hotelName = null)
    {
        try
        {
            var (checkIn, checkOut) = ParseDateRange(dateRange);

            if (!checkIn.HasValue || !checkOut.HasValue)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Could not parse the date range. Please provide dates like '12-16 January 2025' or '5-10 ocak 2025'.",
                    providedRange = dateRange
                }, JsonOptions);
            }

            // Get all rooms
            var allRooms = await _reservationService.GetAllRoomsAsync();

            // Filter by hotel if specified
            if (!string.IsNullOrWhiteSpace(hotelName))
            {
                allRooms = allRooms.Where(r =>
                    r.Hotel.HotelName.ToLower().Contains(hotelName.ToLower()) ||
                    r.Hotel.City.ToLower().Contains(hotelName.ToLower())
                ).ToList();
            }

            // Filter by guest count (room capacity)
            allRooms = allRooms.Where(r => r.Capacity >= guestCount).ToList();

            var availableRooms = new List<object>();

            foreach (var room in allRooms)
            {
                var isAvailable = await _reservationService.IsRoomAvailableAsync(room.Id, checkIn.Value, checkOut.Value);

                if (isAvailable)
                {
                    var nights = (checkOut.Value - checkIn.Value).Days;
                    var totalPrice = room.TotalPrice * nights;

                    availableRooms.Add(new
                    {
                        roomId = room.Id,
                        roomNumber = room.RoomNumber,
                        floor = room.Floor,
                        capacity = room.Capacity,
                        hotel = new
                        {
                            id = room.Hotel.HotelId,
                            name = room.Hotel.HotelName,
                            city = room.Hotel.City,
                            country = room.Hotel.Country,
                            starRating = room.Hotel.StarRating,
                            description = room.Hotel.Description
                        },
                        features = new
                        {
                            isSeaView = room.IsSeaView,
                            isCityView = room.IsCityView
                        },
                        pricing = new
                        {
                            pricePerNight = room.TotalPrice,
                            totalPrice = totalPrice,
                            nights = nights,
                            currency = "USD"
                        }
                    });
                }
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                parsedDates = new
                {
                    checkIn = checkIn.Value.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.Value.ToString("yyyy-MM-dd"),
                    nights = (checkOut.Value - checkIn.Value).Days
                },
                searchCriteria = new
                {
                    dateRange = dateRange,
                    guestCount = guestCount,
                    hotelPreference = hotelName
                },
                availableRooms = availableRooms,
                totalFoundRooms = availableRooms.Count,
                message = availableRooms.Any()
                    ? $"Found {availableRooms.Count} available rooms for your dates."
                    : "No rooms available for the specified dates and criteria."
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching for rooms.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    private (DateTime? checkIn, DateTime? checkOut) ParseDateRange(string dateRange)
    {
        if (string.IsNullOrWhiteSpace(dateRange))
            return (null, null);

        dateRange = dateRange.Trim().ToLower();

        // Türkçe ve İngilizce ay isimleri
        var months = new Dictionary<string, int>
        {
            {"ocak", 1}, {"january", 1}, {"jan", 1},
            {"şubat", 2}, {"february", 2}, {"feb", 2},
            {"mart", 3}, {"march", 3}, {"mar", 3},
            {"nisan", 4}, {"april", 4}, {"apr", 4},
            {"mayıs", 5}, {"may", 5},
            {"haziran", 6}, {"june", 6}, {"jun", 6},
            {"temmuz", 7}, {"july", 7}, {"jul", 7},
            {"ağustos", 8}, {"august", 8}, {"aug", 8},
            {"eylül", 9}, {"september", 9}, {"sep", 9},
            {"ekim", 10}, {"october", 10}, {"oct", 10},
            {"kasım", 11}, {"november", 11}, {"nov", 11},
            {"aralık", 12}, {"december", 12}, {"dec", 12}
        };

        // "12-16 ocak 2025" veya "12-16 january 2025" formatı
        var parts = dateRange.Split(' ');

        if (parts.Length >= 3)
        {
            var dayRange = parts[0].Split('-');
            if (dayRange.Length == 2 &&
                int.TryParse(dayRange[0], out var startDay) &&
                int.TryParse(dayRange[1], out var endDay) &&
                months.ContainsKey(parts[1]) &&
                int.TryParse(parts[2], out var year))
            {
                var month = months[parts[1]];
                var checkIn = new DateTime(year, month, startDay);
                var checkOut = new DateTime(year, month, endDay);
                return (checkIn, checkOut);
            }
        }

        // "5 ocak - 10 ocak 2025" formatı
        if (dateRange.Contains(" - "))
        {
            var rangeParts = dateRange.Split(" - ");
            if (rangeParts.Length == 2)
            {
                var startDate = ParseSingleDate(rangeParts[0].Trim());
                var endDate = ParseSingleDate(rangeParts[1].Trim());

                if (startDate.HasValue && endDate.HasValue)
                    return (startDate, endDate);
            }
        }

        return (null, null);
    }

    private DateTime? ParseSingleDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        // Standart parse işlemi
        if (DateTime.TryParse(dateString, out var result))
            return result;

        var months = new Dictionary<string, int>
        {
            {"ocak", 1}, {"january", 1}, {"jan", 1},
            {"şubat", 2}, {"february", 2}, {"feb", 2},
            {"mart", 3}, {"march", 3}, {"mar", 3},
            {"nisan", 4}, {"april", 4}, {"apr", 4},
            {"mayıs", 5}, {"may", 5},
            {"haziran", 6}, {"june", 6}, {"jun", 6},
            {"temmuz", 7}, {"july", 7}, {"jul", 7},
            {"ağustos", 8}, {"august", 8}, {"aug", 8},
            {"eylül", 9}, {"september", 9}, {"sep", 9},
            {"ekim", 10}, {"october", 10}, {"oct", 10},
            {"kasım", 11}, {"november", 11}, {"nov", 11},
            {"aralık", 12}, {"december", 12}, {"dec", 12}
        };

        // "12 ocak 2025" formatı
        var parts = dateString.Split(' ');
        if (parts.Length >= 3 &&
            int.TryParse(parts[0], out var day) &&
            months.ContainsKey(parts[1].ToLower()) &&
            int.TryParse(parts[2], out var year))
        {
            var month = months[parts[1].ToLower()];
            return new DateTime(year, month, day);
        }

        return null;
    }
}