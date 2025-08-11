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

    [KernelFunction, Description("Create a new hotel reservation for a person. Checks room availability before creating the reservation.")]
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

            // Oda müsaitlik kontrolü
            var isAvailable = await _reservationService.IsRoomAvailableAsync(roomId, checkIn, checkOut);

            if (!isAvailable)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Room {roomId} is not available between {checkIn:yyyy-MM-dd} and {checkOut:yyyy-MM-dd}. The room might be blocked, reserved, or out of service during this period.",
                    roomId = roomId,
                    requestedDates = new
                    {
                        checkIn = checkIn.ToString("yyyy-MM-dd"),
                        checkOut = checkOut.ToString("yyyy-MM-dd")
                    }
                }, JsonOptions);
            }

            // Rezervasyon oluştur
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

    [KernelFunction, Description("Get all hotel reservations in the system")]
    public async Task<string> GetAllReservations()
    {
        try
        {
            var reservations = await _reservationService.GetAllReservationsAsync();

            if (!reservations.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    message = "No reservations found in the system.",
                    totalReservations = 0,
                    reservations = new List<object>()
                }, JsonOptions);
            }

            var reservationsList = reservations.Select(r => new
            {
                reservationId = r.Id,
                person = new
                {
                    id = r.Person.Id,
                    name = $"{r.Person.FirstName} {r.Person.LastName}",
                    email = r.Person.Email,
                    phone = r.Person.Phone
                },
                hotel = new
                {
                    id = r.Hotel.HotelId,
                    name = r.Hotel.HotelName,
                    city = r.Hotel.City,
                    country = r.Hotel.Country,
                    starRating = r.Hotel.StarRating
                },
                room = new
                {
                    id = r.Room.Id,
                    number = r.Room.RoomNumber,
                    floor = r.Room.Floor,
                    capacity = r.Room.Capacity,
                    isSeaView = r.Room.IsSeaView,
                    isCityView = r.Room.IsCityView
                },
                dates = new
                {
                    checkIn = r.CheckIn.ToString("yyyy-MM-dd"),
                    checkOut = r.CheckOut.ToString("yyyy-MM-dd"),
                    nights = (r.CheckOut - r.CheckIn).Days
                },
                totalPrice = r.TotalPrice,
                currency = "TRY"
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                message = "All reservations retrieved successfully.",
                totalReservations = reservations.Count,
                reservations = reservationsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                error = "An error occurred while retrieving reservations.",
                details = ex.Message
            }, JsonOptions);
        }
    }
}