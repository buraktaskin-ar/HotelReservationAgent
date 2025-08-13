using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;

namespace HotelReservationAgentChatBot.Plugins;

public class ReservationPlugin
{
    private readonly IReservationService _reservationService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public ReservationPlugin(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [KernelFunction, Description("Create a new hotel reservation for a person. Checks room availability before creating the reservation. Will NOT automatically select alternative rooms - user must choose.")]
    public async Task<string> CreateReservation(
        [Description("The ID of the person making the reservation")] int personId,
        [Description("The hotel ID where the reservation will be made")] string hotelId,
        [Description("The room ID to be reserved")] int roomId,
        [Description("Check-in date in YYYY-MM-DD format")] string checkInDate,
        [Description("Check-out date in YYYY-MM-DD format")] string checkOutDate)
    {
        try
        {
            // Tarih dönüşümü
            if (!DateTime.TryParse(checkInDate, out var checkIn))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid check-in date format. Please use YYYY-MM-DD format.",
                    providedDate = checkInDate
                }, JsonOptions);
            }

            if (!DateTime.TryParse(checkOutDate, out var checkOut))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid check-out date format. Please use YYYY-MM-DD format.",
                    providedDate = checkOutDate
                }, JsonOptions);
            }

            // Tarih mantık kontrolü
            if (checkIn >= checkOut)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Check-out date must be after check-in date.",
                    checkIn = checkIn.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.ToString("yyyy-MM-dd")
                }, JsonOptions);
            }

            if (checkIn < DateTime.Today)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Check-in date cannot be in the past.",
                    checkIn = checkIn.ToString("yyyy-MM-dd"),
                    today = DateTime.Today.ToString("yyyy-MM-dd")
                }, JsonOptions);
            }

            // İstenen odayı kontrol et
            var requestedRoom = await _reservationService.GetRoomAsync(roomId);
            if (requestedRoom == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Room with ID {roomId} not found.",
                    roomId = roomId
                }, JsonOptions);
            }

            // Oda müsaitlik kontrolü
            var isAvailable = await _reservationService.IsRoomAvailableAsync(roomId, checkIn, checkOut);

            if (!isAvailable)
            {
                // İstenen oda müsait değil - alternatif odaları göster
                return await GetAlternativeRooms(requestedRoom, checkIn, checkOut, personId);
            }

            // Oda müsait - rezervasyon yap
            var reservation = await _reservationService.CreateReservationAsync(personId, hotelId, roomId, checkIn, checkOut);

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Reservation created successfully!",
                reservation = new
                {
                    reservationId = reservation.Id,
                    person = new
                    {
                        id = reservation.Person.Id,
                        name = $"{reservation.Person.FirstName} {reservation.Person.LastName}",
                        email = reservation.Person.Email
                    },
                    hotel = new
                    {
                        id = reservation.Hotel.HotelId,
                        name = reservation.Hotel.HotelName,
                        city = reservation.Hotel.City
                    },
                    room = new
                    {
                        id = reservation.Room.Id,
                        number = reservation.Room.RoomNumber,
                        floor = reservation.Room.Floor,
                        capacity = reservation.Room.Capacity
                    },
                    dates = new
                    {
                        checkIn = reservation.CheckIn.ToString("yyyy-MM-dd"),
                        checkOut = reservation.CheckOut.ToString("yyyy-MM-dd"),
                        nights = (reservation.CheckOut - reservation.CheckIn).Days
                    },
                    totalPrice = reservation.TotalPrice,
                    currency = "TRY"
                }
            }, JsonOptions);
        }
        catch (InvalidOperationException ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An unexpected error occurred while creating the reservation.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get alternative available rooms in the same hotel when the requested room is not available")]
    public async Task<string> GetAlternativeRoomsForReservation(
        [Description("The hotel name or ID")] string hotelIdentifier,
        [Description("Check-in date in YYYY-MM-DD format")] string checkInDate,
        [Description("Check-out date in YYYY-MM-DD format")] string checkOutDate,
        [Description("The ID of the person making the reservation (optional)")] int? personId = null)
    {
        try
        {
            if (!DateTime.TryParse(checkInDate, out var checkIn) || !DateTime.TryParse(checkOutDate, out var checkOut))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid date format. Please use YYYY-MM-DD format."
                }, JsonOptions);
            }

            var allRooms = await _reservationService.GetAllRoomsAsync();

            // Otele ait odaları bul
            var hotelRooms = allRooms.Where(r =>
                r.Hotel.HotelName.ToLower().Contains(hotelIdentifier.ToLower()) ||
                r.Hotel.HotelId.Equals(hotelIdentifier, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (!hotelRooms.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Hotel '{hotelIdentifier}' not found.",
                    hotelIdentifier = hotelIdentifier
                }, JsonOptions);
            }

            var availableAlternatives = new List<object>();
            var hotelName = hotelRooms.First().Hotel.HotelName;

            foreach (var room in hotelRooms)
            {
                var isAvailable = await _reservationService.IsRoomAvailableAsync(room.Id, checkIn, checkOut);
                if (isAvailable)
                {
                    var nights = (checkOut - checkIn).Days;
                    var totalPrice = room.TotalPrice * nights;

                    availableAlternatives.Add(new
                    {
                        roomId = room.Id,
                        roomNumber = room.RoomNumber,
                        floor = room.Floor,
                        capacity = room.Capacity,
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
                message = $"{hotelName} otelinde belirtilen tarihlerde müsait alternatif odalar:",
                hotel = hotelName,
                requestedDates = new
                {
                    checkIn = checkIn.ToString("yyyy-MM-dd"),
                    checkOut = checkOut.ToString("yyyy-MM-dd"),
                    nights = (checkOut - checkIn).Days
                },
                availableRooms = availableAlternatives,
                totalAlternatives = availableAlternatives.Count,
                note = availableAlternatives.Any()
                    ? "Bu alternatif odalardan birini seçerek rezervasyon yapabilirsiniz."
                    : "Maalesef bu tarihlerde hiçbir oda müsait değil."
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching for alternative rooms.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    private async Task<string> GetAlternativeRooms(Models.Room requestedRoom, DateTime checkIn, DateTime checkOut, int personId)
    {
        var allRooms = await _reservationService.GetAllRoomsAsync();

        // Aynı oteldeki diğer odaları bul
        var sameHotelRooms = allRooms.Where(r =>
            r.Hotel.HotelId == requestedRoom.Hotel.HotelId &&
            r.Id != requestedRoom.Id
        ).ToList();

        var availableAlternatives = new List<object>();

        foreach (var room in sameHotelRooms)
        {
            var isAvailable = await _reservationService.IsRoomAvailableAsync(room.Id, checkIn, checkOut);
            if (isAvailable)
            {
                var nights = (checkOut - checkIn).Days;
                var totalPrice = room.TotalPrice * nights;

                availableAlternatives.Add(new
                {
                    roomId = room.Id,
                    roomNumber = room.RoomNumber,
                    floor = room.Floor,
                    capacity = room.Capacity,
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
            success = false,
            message = $"Room {requestedRoom.RoomNumber} is not available for the requested dates.",
            requestedRoom = new
            {
                roomId = requestedRoom.Id,
                roomNumber = requestedRoom.RoomNumber,
                hotel = requestedRoom.Hotel.HotelName
            },
            requestedDates = new
            {
                checkIn = checkIn.ToString("yyyy-MM-dd"),
                checkOut = checkOut.ToString("yyyy-MM-dd"),
                nights = (checkOut - checkIn).Days
            },
            reason = "The room is already reserved, blocked, or out of service for these dates.",
            availableAlternatives = availableAlternatives,
            totalAlternatives = availableAlternatives.Count,
            suggestion = availableAlternatives.Any()
                ? $"However, there are {availableAlternatives.Count} alternative rooms available in {requestedRoom.Hotel.HotelName}. Please choose one of the alternatives to make your reservation."
                : $"Unfortunately, no rooms are available in {requestedRoom.Hotel.HotelName} for these dates. Please try different dates or a different hotel."
        }, JsonOptions);
    }

   
}