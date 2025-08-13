using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using HotelReservationAgentChatBot.Services;

namespace HotelReservationAgentChatBot.Plugins;

public class HotelServicePlugin
{
    private readonly IHotelService _hotelService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public HotelServicePlugin(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [KernelFunction, Description("Get all hotels in the system with their complete information including amenities, ratings, and pricing")]
    public async Task<string> GetAllHotels()
    {
        try
        {
            var hotels = await _hotelService.GetAllHotelsAsync();

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    message = "No hotels found in the system.",
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                address = h.Address,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                currency = "USD",
                description = h.Description,
                amenities = h.Amenities,
                roomTypes = h.RoomTypes,
                cancellationPolicy = h.CancellationPolicy,
                checkInCheckOut = h.CheckInCheckOut,
                houseRules = h.HouseRules,
                nearbyAttractions = h.NearbyAttractions,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                message = "All hotels retrieved successfully.",
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while retrieving hotels.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get detailed information about a specific hotel by its ID")]
    public async Task<string> GetHotelInfo(
        [Description("The hotel ID to retrieve information for")] string hotelId)
    {
        try
        {
            var hotel = await _hotelService.GetHotelAsync(hotelId);

            if (hotel == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = $"Hotel with ID '{hotelId}' not found.",
                    hotelId = hotelId
                }, JsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Hotel information retrieved successfully.",
                hotel = new
                {
                    hotelId = hotel.HotelId,
                    hotelName = hotel.HotelName,
                    city = hotel.City,
                    country = hotel.Country,
                    address = hotel.Address,
                    starRating = hotel.StarRating,
                    pricePerNight = hotel.PricePerNight,
                    currency = "USD",
                    description = hotel.Description,
                    amenities = hotel.Amenities,
                    roomTypes = hotel.RoomTypes,
                    cancellationPolicy = hotel.CancellationPolicy,
                    checkInCheckOut = hotel.CheckInCheckOut,
                    houseRules = hotel.HouseRules,
                    nearbyAttractions = hotel.NearbyAttractions,
                    features = new
                    {
                        hasPool = hotel.HasPool,
                        hasGym = hotel.HasGym,
                        hasSpa = hotel.HasSpa,
                        petFriendly = hotel.PetFriendly,
                        hasParking = hotel.HasParking,
                        hasWifi = hotel.HasWifi
                    }
                }
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while retrieving hotel information.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get hotels in a specific city or country")]
    public async Task<string> GetHotelsByLocation(
        [Description("City or country name to search for hotels")] string location)
    {
        try
        {
            var hotels = await _hotelService.GetHotelsByCityAsync(location);

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No hotels found in '{location}'.",
                    searchLocation = location,
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                address = h.Address,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                description = h.Description,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = $"Found {hotels.Count} hotels in '{location}'.",
                searchLocation = location,
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching hotels by location.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get hotels with a specific star rating (1-5 stars)")]
    public async Task<string> GetHotelsByStarRating(
        [Description("Star rating to filter hotels (1-5)")] int starRating)
    {
        try
        {
            if (starRating < 1 || starRating > 5)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Star rating must be between 1 and 5.",
                    providedRating = starRating
                }, JsonOptions);
            }

            var hotels = await _hotelService.GetHotelsByStarRatingAsync(starRating);

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No {starRating}-star hotels found.",
                    starRating = starRating,
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                description = h.Description,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = $"Found {hotels.Count} hotels with {starRating} stars.",
                starRating = starRating,
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching hotels by star rating.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Search hotels by specific amenities like pool, gym, spa, pet-friendly, parking, or wifi")]
    public async Task<string> GetHotelsByAmenities(
        [Description("Search for hotels with pool")] bool hasPool = false,
        [Description("Search for hotels with gym")] bool hasGym = false,
        [Description("Search for hotels with spa")] bool hasSpa = false,
        [Description("Search for pet-friendly hotels")] bool petFriendly = false,
        [Description("Search for hotels with parking")] bool hasParking = false,
        [Description("Search for hotels with wifi")] bool hasWifi = false)
    {
        try
        {
            var hotels = await _hotelService.GetHotelsWithAmenitiesAsync(hasPool, hasGym, hasSpa, petFriendly, hasParking, hasWifi);

            var searchCriteria = new List<string>();
            if (hasPool) searchCriteria.Add("Pool");
            if (hasGym) searchCriteria.Add("Gym");
            if (hasSpa) searchCriteria.Add("Spa");
            if (petFriendly) searchCriteria.Add("Pet-Friendly");
            if (hasParking) searchCriteria.Add("Parking");
            if (hasWifi) searchCriteria.Add("WiFi");

            var criteriaText = searchCriteria.Any() ? string.Join(", ", searchCriteria) : "Any amenities";

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No hotels found with the specified amenities: {criteriaText}.",
                    searchCriteria = criteriaText,
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                description = h.Description,
                amenities = h.Amenities,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = $"Found {hotels.Count} hotels with amenities: {criteriaText}.",
                searchCriteria = criteriaText,
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching hotels by amenities.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Get hotels within a specific price range per night")]
    public async Task<string> GetHotelsByPriceRange(
        [Description("Minimum price per night")] double minPrice,
        [Description("Maximum price per night")] double maxPrice)
    {
        try
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid price range. Minimum price must be less than or equal to maximum price, and both must be positive.",
                    providedRange = new { minPrice, maxPrice }
                }, JsonOptions);
            }

            var hotels = await _hotelService.GetHotelsByPriceRangeAsync(minPrice, maxPrice);

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No hotels found in price range ${minPrice} - ${maxPrice} per night.",
                    priceRange = new { minPrice, maxPrice },
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                description = h.Description,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = $"Found {hotels.Count} hotels in price range ${minPrice} - ${maxPrice} per night.",
                priceRange = new { minPrice, maxPrice, currency = "USD" },
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching hotels by price range.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    [KernelFunction, Description("Search hotels by name or partial name match")]
    public async Task<string> SearchHotelsByName(
        [Description("Hotel name or partial name to search for")] string hotelName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hotelName))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Hotel name cannot be empty.",
                    providedName = hotelName
                }, JsonOptions);
            }

            var hotels = await _hotelService.SearchHotelsByNameAsync(hotelName);

            if (!hotels.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No hotels found matching '{hotelName}'.",
                    searchTerm = hotelName,
                    totalHotels = 0,
                    hotels = new List<object>()
                }, JsonOptions);
            }

            var hotelsList = hotels.Select(h => new
            {
                hotelId = h.HotelId,
                hotelName = h.HotelName,
                city = h.City,
                country = h.Country,
                starRating = h.StarRating,
                pricePerNight = h.PricePerNight,
                description = h.Description,
                features = new
                {
                    hasPool = h.HasPool,
                    hasGym = h.HasGym,
                    hasSpa = h.HasSpa,
                    petFriendly = h.PetFriendly,
                    hasParking = h.HasParking,
                    hasWifi = h.HasWifi
                }
            }).ToList();

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = $"Found {hotels.Count} hotels matching '{hotelName}'.",
                searchTerm = hotelName,
                totalHotels = hotels.Count,
                hotels = hotelsList
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching hotels by name.",
                details = ex.Message
            }, JsonOptions);
        }
    }
}