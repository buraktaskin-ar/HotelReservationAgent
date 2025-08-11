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

    [KernelFunction, Description("Search for hotels based on various criteria including location, price, amenities, and features. Use this when users ask about hotels, accommodations, or lodging.")]
    public async Task<string> SearchHotels(
        [Description("The search query. Can include city names, price ranges (e.g., 'under $300'), amenities (pool, gym, spa, parking, wifi, pet-friendly), star ratings, or any hotel-related criteria")] string query)
    {
        try
        {
            var (searchOptions, useTextSearch) = await _searchService.BuildSmartSearchOptionsAsync(query);

            var searchResponse = await _searchClient.SearchAsync<HotelSearchDocument>(
                useTextSearch ? query : null,
                searchOptions);

            var results = new List<object>();
            await foreach (var hit in searchResponse.Value.GetResultsAsync())
            {
                var hotel = hit.Document;
                results.Add(new
                {
                    hotelName = hotel.HotelName,
                    city = hotel.City,
                    country = hotel.Country,
                    address = hotel.Address,
                    starRating = hotel.StarRating,
                    pricePerNight = hotel.PricePerNight,
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
                    message = "No hotels found matching your criteria.",
                    query = query
                }, JsonOptions);
            }

            return JsonSerializer.Serialize(new
            {
                query = query,
                totalResults = results.Count,
                hotels = results
            }, JsonOptions);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                error = "An error occurred while searching for hotels.",
                details = ex.Message
            }, JsonOptions);
        }
    }

}