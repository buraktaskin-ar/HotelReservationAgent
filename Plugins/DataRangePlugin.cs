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

    [KernelFunction, Description("Parse date ranges like '12/12/2025 - 15/12/2025' and find available rooms for those dates. Prioritizes Kids Paradise Family Resort rooms, especially room 304.")]
    public async Task<string> FindRoomsForDateRange(
        [Description("Date range in natural language like '12/12/2025 - 15/12/2025', '12-16 January 2025', etc.")] string dateRange,
        [Description("Number of guests (1 for single room, 2 for double room, 3+ for family room)")] int guestCount = 3,
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
                    error = "Tarih formatını anlayamadım. Lütfen '12/12/2025 - 15/12/2025' formatında yazın.",
                    providedRange = dateRange
                }, JsonOptions);
            }

            // Tüm odaları al
            var allRooms = await _reservationService.GetAllRoomsAsync();

            // Kids Paradise Family Resort'u öncelikli göster
            var kidsParadiseRooms = allRooms.Where(r =>
                r.Hotel.HotelName.Contains("Kids Paradise Family Resort"))
                .ToList();

            var availableRooms = new List<object>();

            // Önce Kids Paradise odalarını kontrol et
            foreach (var room in kidsParadiseRooms.OrderBy(r => r.RoomNumber))
            {
                var isAvailable = await _reservationService.IsRoomAvailableAsync(room.Id, checkIn.Value, checkOut.Value);

                if (isAvailable && room.Capacity >= guestCount)
                {
                    var nights = (checkOut.Value - checkIn.Value).Days;
                    var totalPrice = room.TotalPrice * nights;

                    var roomInfo = new
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
                            isCityView = room.IsCityView,
                            viewDescription = room.IsSeaView ? "Deniz Manzaralı" : room.IsCityView ? "Şehir Manzaralı" : "Standart"
                        },
                        pricing = new
                        {
                            pricePerNight = room.TotalPrice,
                            totalPrice = totalPrice,
                            nights = nights,
                            currency = "USD"
                        },
                        isRecommended = room.RoomNumber == "304" // 304 numaralı odayı özel işaretle
                    };

                    availableRooms.Add(roomInfo);
                }
            }

            // Eğer istenen kapasite için başka otel odaları da eklenebilir
            if (availableRooms.Count < 3)
            {
                var otherRooms = allRooms.Where(r =>
                    !r.Hotel.HotelName.Contains("Kids Paradise Family Resort") &&
                    r.Capacity >= guestCount)
                    .Take(2);

                foreach (var room in otherRooms)
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
                                starRating = room.Hotel.StarRating
                            },
                            features = new
                            {
                                isSeaView = room.IsSeaView,
                                isCityView = room.IsCityView,
                                viewDescription = room.IsSeaView ? "Deniz Manzaralı" : room.IsCityView ? "Şehir Manzaralı" : "Standart"
                            },
                            pricing = new
                            {
                                pricePerNight = room.TotalPrice,
                                totalPrice = totalPrice,
                                nights = nights,
                                currency = "USD"
                            },
                            isRecommended = false
                        });
                    }
                }
            }

            var response = new
            {
                success = true,
                parsedDates = new
                {
                    checkIn = checkIn.Value.ToString("dd/MM/yyyy"),
                    checkOut = checkOut.Value.ToString("dd/MM/yyyy"),
                    nights = (checkOut.Value - checkIn.Value).Days
                },
                searchCriteria = new
                {
                    dateRange = dateRange,
                    guestCount = guestCount,
                    hotelPreference = hotelName ?? "Kids Paradise Family Resort (Öncelikli)"
                },
                availableRooms = availableRooms,
                totalFoundRooms = availableRooms.Count,
                message = availableRooms.Any()
                    ? $"✅ {availableRooms.Count} adet müsait oda bulundu!"
                    : "❌ Belirtilen tarihlerde müsait oda bulunamadı.",
                specialOffers = availableRooms.Any(r => ((dynamic)r).isRecommended)
                    ? "🌟 304 numaralı oda 3 kişilik deniz manzaralı aile odası - ÖNERİLEN!"
                    : null,
                nextStep = availableRooms.Any()
                    ? "Hangi odayı seçmek istersiniz? Oda numarası söyleyebilirsiniz."
                    : "Farklı tarihler deneyin veya başka otel seçeneklerine bakın."
            };

            return JsonSerializer.Serialize(response, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Oda arama sırasında hata oluştu.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    private (DateTime? checkIn, DateTime? checkOut) ParseDateRange(string dateRange)
    {
        if (string.IsNullOrWhiteSpace(dateRange))
            return (null, null);

        dateRange = dateRange.Trim().ToLower();

        // "12/12/2025 - 15/12/2025" veya "12/12/2025-15/12/2025" formatı
        if (dateRange.Contains("-"))
        {
            var parts = dateRange.Split('-');
            if (parts.Length == 2)
            {
                var startDate = ParseSingleDate(parts[0].Trim());
                var endDate = ParseSingleDate(parts[1].Trim());

                if (startDate.HasValue && endDate.HasValue)
                    return (startDate, endDate);
            }
        }

        // "12/12/2025 15/12/2025" formatı (boşlukla ayrılmış)
        var spaceParts = dateRange.Split(' ');
        if (spaceParts.Length >= 2)
        {
            var startDate = ParseSingleDate(spaceParts[0]);
            var endDate = ParseSingleDate(spaceParts[1]);

            if (startDate.HasValue && endDate.HasValue)
                return (startDate, endDate);
        }

        return (null, null);
    }

    private DateTime? ParseSingleDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        dateString = dateString.Trim();

        // Önce standart parse işlemi dene
        if (DateTime.TryParse(dateString, out var result))
            return result;

        // DD/MM/YYYY, DD-MM-YYYY formatları
        var formats = new[]
        {
            "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy",
            "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd",
            "d/M/yyyy", "d-M-yyyy"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
        }

        return null;
    }
}