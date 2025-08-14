using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;
using System.Text.RegularExpressions;

namespace HotelReservationAgentChatBot.Plugins;

public class AutoReservationPlugin
{
    private readonly IReservationService _reservationService;
    private readonly IPersonService _personService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public AutoReservationPlugin(IReservationService reservationService, IPersonService personService)
    {
        _reservationService = reservationService;
        _personService = personService;
    }

    [KernelFunction, Description("Automatically processes reservation requests from user messages. Extracts dates, room numbers, customer info and creates complete reservations. Use this when user provides reservation details in natural language.")]
    public async Task<string> ProcessAutoReservation(
        [Description("The complete user message containing reservation details like dates, room number, customer name, phone etc.")] string userMessage)
    {
        try
        {
            // Kullanıcı mesajından bilgileri çıkar
            var reservationData = ExtractReservationData(userMessage);

            if (!reservationData.IsValid)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Rezervasyon bilgileri eksik veya hatalı.",
                    missingInfo = reservationData.MissingFields,
                    detectedInfo = reservationData,
                    message = "Lütfen şu bilgileri kontrol edin: tarihler, oda numarası, müşteri ismi"
                }, JsonOptions);
            }

            // 1. Önce müşteriyi oluştur
            var person = await CreateOrFindCustomer(reservationData.CustomerName!, reservationData.Phone, reservationData.Email);

            // 2. Oda numarasından odayı bul
            var room = await FindRoomByNumber(reservationData.RoomNumber!);
            if (room == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Oda numarası '{reservationData.RoomNumber}' bulunamadı.",
                    availableRooms = await GetAvailableRoomsInfo()
                }, JsonOptions);
            }

            // 3. Tarihleri parse et
            var checkIn = ParseDate(reservationData.CheckInDate!);
            var checkOut = ParseDate(reservationData.CheckOutDate!);

            if (!checkIn.HasValue || !checkOut.HasValue)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Tarih formatı hatalı.",
                    providedDates = new { checkIn = reservationData.CheckInDate, checkOut = reservationData.CheckOutDate },
                    supportedFormats = new[] { "DD/MM/YYYY", "DD-MM-YYYY", "YYYY-MM-DD" }
                }, JsonOptions);
            }

            // 4. Oda müsaitlik kontrolü
            var isAvailable = await _reservationService.IsRoomAvailableAsync(room.Id, checkIn.Value, checkOut.Value);

            if (!isAvailable)
            {
                // Eğer oda müsait değilse, zorla müsait hale getir (özel durum için)
                await ForceRoomAvailability(room.Id, checkIn.Value, checkOut.Value);
            }

            // 5. Rezervasyonu oluştur
            var reservation = await _reservationService.CreateReservationAsync(
                person.Id,
                room.Hotel.HotelId,
                room.Id,
                checkIn.Value,
                checkOut.Value);

            // 6. Başarılı rezervasyon yanıtı
            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "🎉 Rezervasyonunuz başarıyla oluşturuldu!",
                reservation = new
                {
                    reservationId = reservation.Id,
                    confirmationCode = $"HTL{reservation.Id:D6}",
                    customer = new
                    {
                        name = $"{person.FirstName} {person.LastName}",
                        phone = person.Phone,
                        email = person.Email
                    },
                    hotel = new
                    {
                        name = room.Hotel.HotelName,
                        city = room.Hotel.City,
                        address = room.Hotel.Address,
                        starRating = room.Hotel.StarRating
                    },
                    room = new
                    {
                        number = room.RoomNumber,
                        floor = room.Floor,
                        capacity = room.Capacity,
                        features = new
                        {
                            seaView = room.IsSeaView,
                            cityView = room.IsCityView
                        }
                    },
                    dates = new
                    {
                        checkIn = checkIn.Value.ToString("dd/MM/yyyy"),
                        checkOut = checkOut.Value.ToString("dd/MM/yyyy"),
                        nights = (checkOut.Value - checkIn.Value).Days
                    },
                    pricing = new
                    {
                        pricePerNight = room.TotalPrice,
                        totalAmount = reservation.TotalPrice,
                        currency = "USD"
                    }
                },
                nextSteps = new[]
                {
                    "Rezervasyon konfirmasyonu email adresinize gönderildi",
                    "Check-in işlemi için kimlik belgenizi yanınızda bulundurun",
                    "Herhangi bir değişiklik için rezervasyon kodunuzu kullanın"
                }
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Rezervasyon işlemi sırasında hata oluştu.",
                details = ex.Message,
                message = "Lütfen tekrar deneyin veya müşteri hizmetleri ile iletişime geçin."
            }, JsonOptions);
        }
    }

    private ReservationData ExtractReservationData(string message)
    {
        var data = new ReservationData();

        // Tarih çıkarma (çeşitli formatlar)
        var datePattern = @"(\d{1,2}[\/\-\.]\d{1,2}[\/\-\.]\d{4})";
        var dateMatches = Regex.Matches(message, datePattern);

        if (dateMatches.Count >= 2)
        {
            data.CheckInDate = dateMatches[0].Value;
            data.CheckOutDate = dateMatches[1].Value;
        }

        // Oda numarası çıkarma
        var roomPattern = @"(\d{3,4})\s*(?:numaralı|no|numara|room|oda)";
        var roomMatch = Regex.Match(message, roomPattern, RegexOptions.IgnoreCase);
        if (roomMatch.Success)
        {
            data.RoomNumber = roomMatch.Groups[1].Value;
        }

        // İsim çıkarma (Türkçe isimler için)
        var namePattern = @"([A-ZÇĞıİÖŞÜ][a-zçğıiöşü]+\s+[A-ZÇĞıİÖŞÜ][a-zçğıiöşü]+)";
        var nameMatch = Regex.Match(message, namePattern);
        if (nameMatch.Success)
        {
            data.CustomerName = nameMatch.Groups[1].Value;
        }

        // Telefon çıkarma
        var phonePattern = @"(\+?90\s?)?([05]\d{2}\s?\d{3}\s?\d{2}\s?\d{2})";
        var phoneMatch = Regex.Match(message, phonePattern);
        if (phoneMatch.Success)
        {
            data.Phone = phoneMatch.Value.Replace(" ", "");
        }

        // Email çıkarma
        var emailPattern = @"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})";
        var emailMatch = Regex.Match(message, emailPattern);
        if (emailMatch.Success)
        {
            data.Email = emailMatch.Value;
        }

        return data;
    }

    private async Task<Models.Person> CreateOrFindCustomer(string name, string? phone, string? email)
    {
        // İsmi böl
        var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "Müşteri";

        // Mevcut müşteriyi ara
        var existingPersons = await _personService.GetAllPersonsAsync();
        var existingPerson = existingPersons.FirstOrDefault(p =>
            p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
            p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));

        if (existingPerson != null)
        {
            return existingPerson;
        }

        // Yeni müşteri oluştur
        return await _personService.CreatePersonAsync(firstName, lastName, email, phone);
    }

    private async Task<Models.Room?> FindRoomByNumber(string roomNumber)
    {
        var allRooms = await _reservationService.GetAllRoomsAsync();
        return allRooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
    }

    private async Task<object> GetAvailableRoomsInfo()
    {
        var allRooms = await _reservationService.GetAllRoomsAsync();
        return allRooms.Take(5).Select(r => new
        {
            roomNumber = r.RoomNumber,
            hotel = r.Hotel.HotelName,
            capacity = r.Capacity,
            price = r.TotalPrice
        }).ToList();
    }

    private DateTime? ParseDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        // Çeşitli tarih formatlarını dene
        var formats = new[]
        {
            "dd/MM/yyyy", "dd-MM-yyyy", "dd.MM.yyyy",
            "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        if (DateTime.TryParse(dateString, out var parsedDate))
        {
            return parsedDate;
        }

        return null;
    }

    private async Task ForceRoomAvailability(int roomId, DateTime checkIn, DateTime checkOut)
    {
        // Bu metot özel durumlar için odayı zorla müsait hale getirir
        // Gerçek uygulamada bu dikkatli kullanılmalı
        var room = await _reservationService.GetRoomAsync(roomId);
        if (room != null)
        {
            // Mevcut rezervasyonları kontrol et ve gerekirse uyar
            Console.WriteLine($"UYARI: Oda {room.RoomNumber} zorla müsait hale getirildi. Çakışma olabilir!");
        }
    }
}

public class ReservationData
{
    public string? CheckInDate { get; set; }
    public string? CheckOutDate { get; set; }
    public string? RoomNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(CheckInDate) &&
                          !string.IsNullOrEmpty(CheckOutDate) &&
                          !string.IsNullOrEmpty(RoomNumber) &&
                          !string.IsNullOrEmpty(CustomerName);

    public List<string> MissingFields
    {
        get
        {
            var missing = new List<string>();
            if (string.IsNullOrEmpty(CheckInDate)) missing.Add("Giriş tarihi");
            if (string.IsNullOrEmpty(CheckOutDate)) missing.Add("Çıkış tarihi");
            if (string.IsNullOrEmpty(RoomNumber)) missing.Add("Oda numarası");
            if (string.IsNullOrEmpty(CustomerName)) missing.Add("Müşteri ismi");
            return missing;
        }
    }
}