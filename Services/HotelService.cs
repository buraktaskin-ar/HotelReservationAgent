using HotelReservationAgentChatBot.Models;
using HotelReservationAgentChatBot.Data;

namespace HotelReservationAgentChatBot.Services;

public interface IHotelService
{
    Task<List<Hotel>> GetAllHotelsAsync();
    Task<Hotel?> GetHotelAsync(string hotelId);
    Task<List<Hotel>> GetHotelsByCityAsync(string city);
    Task<List<Hotel>> GetHotelsByStarRatingAsync(int starRating);
    Task<List<Hotel>> GetHotelsWithAmenitiesAsync(bool hasPool = false, bool hasGym = false, bool hasSpa = false, bool petFriendly = false, bool hasParking = false, bool hasWifi = false);
    Task<List<Hotel>> SearchHotelsByNameAsync(string hotelName);
    Task<List<Hotel>> GetHotelsByPriceRangeAsync(double minPrice, double maxPrice);
}

public class HotelService : IHotelService
{
    private readonly List<Hotel> _hotels;

    public HotelService()
    {
        // Seed verileri yükle
        var hotelSeeder = new HotelDataSeeder();
        _hotels = hotelSeeder.GetHotels().ToList();
    }

    public async Task<List<Hotel>> GetAllHotelsAsync()
    {
        return await Task.FromResult(_hotels.ToList());
    }

    public async Task<Hotel?> GetHotelAsync(string hotelId)
    {
        var hotel = _hotels.FirstOrDefault(h => h.HotelId.Equals(hotelId, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(hotel);
    }

    public async Task<List<Hotel>> GetHotelsByCityAsync(string city)
    {
        var hotels = _hotels
            .Where(h => h.City.ToLower().Contains(city.ToLower()) ||
                       h.Country.ToLower().Contains(city.ToLower()))
            .ToList();

        return await Task.FromResult(hotels);
    }

    public async Task<List<Hotel>> GetHotelsByStarRatingAsync(int starRating)
    {
        var hotels = _hotels
            .Where(h => h.StarRating == starRating)
            .ToList();

        return await Task.FromResult(hotels);
    }

    public async Task<List<Hotel>> GetHotelsWithAmenitiesAsync(
        bool hasPool = false,
        bool hasGym = false,
        bool hasSpa = false,
        bool petFriendly = false,
        bool hasParking = false,
        bool hasWifi = false)
    {
        var hotels = _hotels.Where(h =>
            (!hasPool || h.HasPool) &&
            (!hasGym || h.HasGym) &&
            (!hasSpa || h.HasSpa) &&
            (!petFriendly || h.PetFriendly) &&
            (!hasParking || h.HasParking) &&
            (!hasWifi || h.HasWifi)
        ).ToList();

        return await Task.FromResult(hotels);
    }

    public async Task<List<Hotel>> SearchHotelsByNameAsync(string hotelName)
    {
        var hotels = _hotels
            .Where(h => h.HotelName.ToLower().Contains(hotelName.ToLower()))
            .ToList();

        return await Task.FromResult(hotels);
    }

    public async Task<List<Hotel>> GetHotelsByPriceRangeAsync(double minPrice, double maxPrice)
    {
        var hotels = _hotels
            .Where(h => h.PricePerNight >= minPrice && h.PricePerNight <= maxPrice)
            .OrderBy(h => h.PricePerNight)
            .ToList();

        return await Task.FromResult(hotels);
    }
}