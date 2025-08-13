using HotelReservationAgentChatBot.Data;
using HotelReservationAgentChatBot.Models;

namespace HotelReservationAgentChatBot.Services;

public interface IRoomService
{
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomAsync(int roomId);
    Task<List<Room>> GetRoomsByHotelAsync(string hotelIdentifier);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
}

public class RoomService : IRoomService
{
    private readonly List<Room> _rooms;

    public RoomService()
    {
        var roomSeeder = new RoomDataSeeder();
        _rooms = roomSeeder.GetRooms();
    }

    public Task<List<Room>> GetAllRoomsAsync()
    {
        return Task.FromResult(_rooms);
    }

    public Task<Room?> GetRoomAsync(int roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        return Task.FromResult(room);
    }

    public Task<List<Room>> GetRoomsByHotelAsync(string hotelIdentifier)
    {
        var rooms = _rooms.Where(r =>
            r.Hotel.HotelName.Contains(hotelIdentifier, StringComparison.OrdinalIgnoreCase) ||
            r.Hotel.HotelId.Equals(hotelIdentifier, StringComparison.OrdinalIgnoreCase) ||
            r.Hotel.City.Contains(hotelIdentifier, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        return Task.FromResult(rooms);
    }

    public Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null) return Task.FromResult(false);

        foreach (var availability in room.Availabilities)
        {
            var slot = availability.AvailabilitySlot;

            // Tarih aralığı çakışıyor mu kontrol et
            if (checkIn < slot.End && checkOut > slot.Start)
            {
                if (slot.Status != AvailabilityStatus.Available)
                {
                    return Task.FromResult(false);
                }
            }
        }

        return Task.FromResult(true);
    }
}