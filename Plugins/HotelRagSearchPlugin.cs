using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using HotelReservationAgentChatBot.Models;
using HotelReservationAgentChatBot.Services;
using Microsoft.SemanticKernel.Embeddings;

namespace HotelReservationAgentChatBot.Plugins;

public class HotelRagSearchPlugin
{
    private readonly SearchClient _searchClient;
    private readonly SearchService _searchService;
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public HotelRagSearchPlugin(
        SearchClient searchClient,
        SearchService searchService,
        ITextEmbeddingGenerationService embeddingService)
    {
        _searchClient = searchClient;
        _searchService = searchService;
        _embeddingService = embeddingService;
    }

    [KernelFunction, Description("Search for hotels based on various criteria including location, price, amenities, star rating, and features. Use this when users ask about hotels, accommodations, or lodging with specific requirements.")]
    public async Task<string> SearchHotels(
        [Description("The search query. Can include city names, price ranges (e.g., 'under $300'), amenities (pool, gym, spa, parking, wifi, pet-friendly), star ratings (e.g., '4 star', '5 yıldız'), or any hotel-related criteria")] string query)
    {
        try
        {
            var (searchOptions, useTextSearch) = await _searchService.BuildSmartSearchOptionsAsync(query);

            // Yıldız sayısı filtrelemesi ekle
            DetectAndAddStarRatingFilter(query, searchOptions);

            var searchResponse = await _searchClient.SearchAsync<HotelSearchDocument>(
                useTextSearch ? query : null,
                searchOptions);

            var results = new List<object>();
            await foreach (var hit in searchResponse.Value.GetResultsAsync())
            {
                var hotel = hit.Document;
                results.Add(new
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
                    nearbyAttractions = hotel.NearbyAttractions,
                    features = new
                    {
                        hasPool = hotel.HasPool,
                        hasGym = hotel.HasGym,
                        hasSpa = hotel.HasSpa,
                        petFriendly = hotel.PetFriendly,
                        hasParking = hotel.HasParking,
                        hasWifi = hotel.HasWifi
                    },
                    relevanceScore = hit.Score
                });
            }

            if (!results.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No hotels found matching your criteria.",
                    query = query,
                    suggestion = "Try adjusting your search criteria, such as changing the location, price range, or amenities."
                }, JsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                query = query,
                totalResults = results.Count,
                hotels = results,
                searchTips = new
                {
                    priceFilter = "Use phrases like 'under $300', 'less than $200', 'below $500'",
                    starFilter = "Use phrases like '4 star', '5 yıldız', '3 stars'",
                    amenityFilter = "Mention specific amenities like 'pool', 'gym', 'spa', 'pet-friendly', 'parking', 'wifi'",
                    locationFilter = "Include city names like 'New York', 'Miami', 'Paris'"
                }
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "An error occurred while searching for hotels.",
                details = ex.Message
            }, JsonOptions);
        }
    }

    private void DetectAndAddStarRatingFilter(string query, SearchOptions searchOptions)
    {
        var queryLower = query.ToLower();

        // İngilizce yıldız sayısı tespiti
        var starPatterns = new[]
        {
            @"(\d+)\s*star(?:s?)",          // "4 star", "5 stars"
            @"(\d+)\s*yıldız(?:lı?)",       // "4 yıldız", "5 yıldızlı"
            @"(\d+)\s*\*",                  // "4*", "5*"
        };

        foreach (var pattern in starPatterns)
        {
            var match = System.Text.RegularExpressions.Regex.Match(queryLower, pattern);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var starRating))
            {
                if (starRating >= 1 && starRating <= 5)
                {
                    var starFilter = $"{nameof(HotelSearchDocument.StarRating)} eq {starRating}";
                    CombineFilters(searchOptions, starFilter);
                    break; // İlk eşleşmeyi kullan
                }
            }
        }

        // Özel kelime kombinasyonları
        if (queryLower.Contains("luxury") || queryLower.Contains("lüks"))
        {
            var luxuryFilter = $"{nameof(HotelSearchDocument.StarRating)} ge 4";
            CombineFilters(searchOptions, luxuryFilter);
        }
        else if (queryLower.Contains("budget") || queryLower.Contains("ekonomik") || queryLower.Contains("ucuz"))
        {
            var budgetFilter = $"{nameof(HotelSearchDocument.StarRating)} le 3";
            CombineFilters(searchOptions, budgetFilter);
        }
    }

    private void CombineFilters(SearchOptions searchOptions, string newFilter)
    {
        if (string.IsNullOrEmpty(searchOptions.Filter))
        {
            searchOptions.Filter = newFilter;
        }
        else
        {
            searchOptions.Filter = $"{searchOptions.Filter} and {newFilter}";
        }
    }
}