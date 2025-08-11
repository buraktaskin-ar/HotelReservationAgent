using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.SemanticKernel.Embeddings;
using System.Text.RegularExpressions;
using HotelReservationAgentChatBot.Models;
namespace HotelReservationAgentChatBot.Services;


public class SearchService
{
    private readonly SearchClient _searchClient;
    private readonly ITextEmbeddingGenerationService _embeddingService;

    public SearchService(SearchClient searchClient, ITextEmbeddingGenerationService embeddingService)
    {
        _searchClient = searchClient;
        _embeddingService = embeddingService;
    }
    
    public async Task<(SearchOptions Options, bool UseTextSearch)> BuildSmartSearchOptionsAsync(string query)
    {
        var options = new SearchOptions { Size = 5 };
        var useTextSearch = true;

        // Price filter detection
        DetectPriceFilter(query, options);

        // Amenity filters
        var amenityFilters = DetectAmenityFilters(query);
        if (amenityFilters.Any())
        {
            CombineFilters(options, string.Join(" and ", amenityFilters));
        }

        // Location search
        if (query.ToLower().Contains("in "))
        {
            options.SearchFields.Add(nameof(HotelSearchDocument.City));
            options.SearchFields.Add(nameof(HotelSearchDocument.Country));
        }

        // Vector search
        await AddVectorSearchAsync(query, options);

        // Text search fields
        ConfigureTextSearchFields(query, options);

        options.QueryType = SearchQueryType.Full;

        return (options, useTextSearch);
    }

    private void DetectPriceFilter(string query, SearchOptions options)
    {
        if (query.Contains("under $") || query.Contains("less than $") || query.Contains("below $"))
        {
            var priceMatch = Regex.Match(query, @"\$?(\d+)");
            if (priceMatch.Success)
            {
                var maxPrice = int.Parse(priceMatch.Groups[1].Value);
                options.Filter = $"{nameof(HotelSearchDocument.PricePerNight)} le {maxPrice}";
            }
        }
    }

    private List<string> DetectAmenityFilters(string query)
    {
        var filters = new List<string>();
        var queryLower = query.ToLower();

        if (queryLower.Contains("gym")) filters.Add($"{nameof(HotelSearchDocument.HasGym)} eq true");
        if (queryLower.Contains("pool")) filters.Add($"{nameof(HotelSearchDocument.HasPool)} eq true");
        if (queryLower.Contains("spa")) filters.Add($"{nameof(HotelSearchDocument.HasSpa)} eq true");
        if (queryLower.Contains("pet")) filters.Add($"{nameof(HotelSearchDocument.PetFriendly)} eq true");
        if (queryLower.Contains("parking")) filters.Add($"{nameof(HotelSearchDocument.HasParking)} eq true");
        if (queryLower.Contains("wifi")) filters.Add($"{nameof(HotelSearchDocument.HasWifi)} eq true");

        return filters;
    }

    private void CombineFilters(SearchOptions options, string newFilter)
    {
        options.Filter = string.IsNullOrEmpty(options.Filter)
            ? newFilter
            : $"{options.Filter} and {newFilter}";
    }

    private async Task AddVectorSearchAsync(string query, SearchOptions options)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(query);
        options.VectorSearch = new VectorSearchOptions
        {
            Queries = {
            new VectorizedQuery(embedding.ToArray())
            {
                KNearestNeighborsCount = 5,
                Fields = {
                    nameof(HotelSearchDocument.DescriptionVector),
                    nameof(HotelSearchDocument.AmenitiesVector)
                },
                Weight = 0.4f
            }
        }
        };
    }

    private void ConfigureTextSearchFields(string query, SearchOptions options)
    {
        if (query.ToLower().Contains("cancel"))
        {
            options.SearchFields.Add(nameof(HotelSearchDocument.CancellationPolicy));
        }
        else
        {
            options.SearchFields.Add(nameof(HotelSearchDocument.HotelName));
            options.SearchFields.Add(nameof(HotelSearchDocument.Description));
            options.SearchFields.Add(nameof(HotelSearchDocument.Amenities));
        }
    }

























    // iptal politikası sorularını hem full-text hem de vektör bazlı hibrit aramayla yanıtlamak için tasarlanmıştır
    //public async Task<SearchResults<HotelSearchDocument>> SearchCancellationPolicyAsync(string query)
    //{
    //    var embedding = await _embeddingService.GenerateEmbeddingAsync(query);

    //    var options = new SearchOptions
    //    {
    //        Size = 3,
    //        QueryType = SearchQueryType.Full,
    //        SearchFields = { nameof(HotelSearchDocument.CancellationPolicy) },
    //        VectorSearch = new VectorSearchOptions
    //        {
    //            Queries = {
    //            new VectorizedQuery(embedding.ToArray())
    //            {
    //                KNearestNeighborsCount = 3,
    //                Fields = { nameof(HotelSearchDocument.DescriptionVector) },
    //                Weight = 0.3f
    //            }
    //        }
    //        }
    //    };

    //    return await _searchClient.SearchAsync<HotelSearchDocument>(query, options);
    //}

    //doğrudan filtre ardından sıralama yapacak, vektör veya tam metin kullanmayacak bir senaryoya odaklıdır.
    //public async Task<SearchResults<HotelSearchDocument>> SearchWithAmenitiesFilterAsync(double maxPrice)
    //{
    //    var filter = $"{nameof(HotelSearchDocument.HasGym)} eq true and " +
    //                $"{nameof(HotelSearchDocument.HasPool)} eq true and " +
    //                $"{nameof(HotelSearchDocument.PricePerNight)} le {maxPrice}";

    //    var options = new SearchOptions
    //    {
    //        Size = 5,
    //        Filter = filter,
    //        OrderBy = { $"{nameof(HotelSearchDocument.PricePerNight)} asc" }
    //    };

    //    return await _searchClient.SearchAsync<HotelSearchDocument>("*", options);
    //}



    //Bu metot, yalnızca vektör sinyaline dayalı bir semantik arama senaryosudur
    //public async Task<SearchResults<HotelSearchDocument>> SearchSemanticAsync(string query)
    //{
    //    var embedding = await _embeddingService.GenerateEmbeddingAsync(query);

    //    var options = new SearchOptions
    //    {
    //        Size = 3,
    //        VectorSearch = new VectorSearchOptions
    //        {
    //            Queries = {
    //            new VectorizedQuery(embedding.ToArray())
    //            {
    //                KNearestNeighborsCount = 3,
    //                Fields = {
    //                    nameof(HotelSearchDocument.DescriptionVector),
    //                    nameof(HotelSearchDocument.AmenitiesVector)
    //                }
    //            }
    //        }
    //        }
    //    };

    //    return await _searchClient.SearchAsync<HotelSearchDocument>(null, options);
    //}




}
