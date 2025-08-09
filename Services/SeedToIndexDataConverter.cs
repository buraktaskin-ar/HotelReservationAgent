using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Microsoft.SemanticKernel.Embeddings;
using HotelReservationAgentChatBot.Data;

using HotelReservationAgentChatBot.Models;


namespace HotelReservationAgentChatBot.Services;

public class SeedToIndexDataConverter
{
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient _searchClient;
    private readonly ITextEmbeddingGenerationService _embeddingService;
    private readonly IndexDefinition _indexDefinition;
    private readonly HotelDataSeeder _dataSeeder;

    public SeedToIndexDataConverter(
        SearchIndexClient indexClient,
        SearchClient searchClient,
        ITextEmbeddingGenerationService embeddingService)
    {
        _indexClient = indexClient;
        _searchClient = searchClient;
        _embeddingService = embeddingService;
        _indexDefinition = new IndexDefinition();
        _dataSeeder = new HotelDataSeeder();
    }

    public async Task InitializeAsync()
    {
        await CreateIndexAsync();
        await SeedDataAsync();
    }




    private async Task CreateIndexAsync()
    {
        var index = _indexDefinition.CreateHotelIndex();
        await _indexClient.CreateOrUpdateIndexAsync(index);
        Console.WriteLine($"Hotel index '{index.Name}' created successfully.");
    }

    private async Task SeedDataAsync()
    {
        Console.WriteLine("Generating embeddings for hotel documents...");

        var hotels = _dataSeeder.GetHotels().ToList();
        var documentsWithVectors = new List<HotelSearchDocument>();

        foreach (var hotel in hotels)
        {
            var document = await CreateDocumentWithEmbeddingsAsync(hotel);
            documentsWithVectors.Add(document);
            Console.WriteLine($"Generated embeddings for: {hotel.HotelName}");
        }

        var uploadResult = await _searchClient.UploadDocumentsAsync(documentsWithVectors);
        var succeededCount = uploadResult.Value.Results.Count(r => r.Succeeded);
        Console.WriteLine($"Upload complete - Succeeded: {succeededCount}/{documentsWithVectors.Count}");
    }

    private async Task<HotelSearchDocument> CreateDocumentWithEmbeddingsAsync(Hotel hotel)
    {
        var descriptionEmbedding = await _embeddingService.GenerateEmbeddingAsync(
            $"{hotel.HotelName} {hotel.Description}");

        var amenitiesEmbedding = await _embeddingService.GenerateEmbeddingAsync(
            hotel.Amenities);

        return new HotelSearchDocument
        {
            HotelId = hotel.HotelId,
            HotelName = hotel.HotelName,
            City = hotel.City,
            Country = hotel.Country,
            Address = hotel.Address,
            StarRating = hotel.StarRating,
            PricePerNight = hotel.PricePerNight,
            Description = hotel.Description,
            Amenities = hotel.Amenities,
            RoomTypes = hotel.RoomTypes,
            CancellationPolicy = hotel.CancellationPolicy,
            CheckInCheckOut = hotel.CheckInCheckOut,
            HouseRules = hotel.HouseRules,
            NearbyAttractions = hotel.NearbyAttractions,
            HasPool = hotel.HasPool,
            HasGym = hotel.HasGym,
            HasSpa = hotel.HasSpa,
            PetFriendly = hotel.PetFriendly,
            HasParking = hotel.HasParking,
            HasWifi = hotel.HasWifi,
            DescriptionVector = descriptionEmbedding.ToArray(),
            AmenitiesVector = amenitiesEmbedding.ToArray()
        };
    }
}