using Azure.Search.Documents.Indexes.Models;
using HotelReservationAgentChatBot.Models;
namespace HotelReservationAgentChatBot.Services;


public class IndexDefinition
{
    private const string IndexName = "hotels-hybrid-index";
    private const string VectorProfileName = "vector-profile";
    private const string HnswConfigName = "hnsw-config";
    private const int VectorDimensions = 1536;

    public SearchIndex CreateHotelIndex()
    {
        var index = new SearchIndex(IndexName)
        {
            Fields = CreateFields(),
            VectorSearch = CreateVectorSearchConfiguration(),
            ScoringProfiles = { CreateScoringProfile() },
            DefaultScoringProfile = "hotelBoost"
        };

        return index;
    }

    private IList<SearchField> CreateFields()
    {
        return new List<SearchField>
{
    // Key field
    new SimpleField(nameof(HotelSearchDocument.HotelId), SearchFieldDataType.String)
    {
        IsKey = true,
        IsFilterable = true
    },
    
    // Searchable text fields
    CreateSearchableField(nameof(HotelSearchDocument.HotelName), true, true, true),
    CreateSearchableField(nameof(HotelSearchDocument.City), true, true, true),
    CreateSearchableField(nameof(HotelSearchDocument.Country), true, true, true),
    CreateSearchableField(nameof(HotelSearchDocument.Address)),
    CreateSearchableField(nameof(HotelSearchDocument.Description)),
    CreateSearchableField(nameof(HotelSearchDocument.Amenities)),
    CreateSearchableField(nameof(HotelSearchDocument.RoomTypes)),
    CreateSearchableField(nameof(HotelSearchDocument.CancellationPolicy)),
    CreateSearchableField(nameof(HotelSearchDocument.CheckInCheckOut)),
    CreateSearchableField(nameof(HotelSearchDocument.HouseRules)),
    CreateSearchableField(nameof(HotelSearchDocument.NearbyAttractions)),
    
    // Numeric fields
    CreateNumericField(nameof(HotelSearchDocument.StarRating), SearchFieldDataType.Int32),
    CreateNumericField(nameof(HotelSearchDocument.PricePerNight), SearchFieldDataType.Double),
    
    // Boolean fields
    CreateBooleanField(nameof(HotelSearchDocument.HasPool)),
    CreateBooleanField(nameof(HotelSearchDocument.HasGym)),
    CreateBooleanField(nameof(HotelSearchDocument.HasSpa)),
    CreateBooleanField(nameof(HotelSearchDocument.PetFriendly)),
    CreateBooleanField(nameof(HotelSearchDocument.HasParking)),
    CreateBooleanField(nameof(HotelSearchDocument.HasWifi)),
    
    // Vector fields
    CreateVectorField(nameof(HotelSearchDocument.DescriptionVector)),
    CreateVectorField(nameof(HotelSearchDocument.AmenitiesVector))
};
    }

    private SearchableField CreateSearchableField(string name, bool isFilterable = false, bool isSortable = false, bool isFacetable = false)
    {
        return new SearchableField(name)
        {
            IsFilterable = isFilterable,
            IsSortable = isSortable,
            IsFacetable = isFacetable
        };
    }

    private SimpleField CreateNumericField(string name, SearchFieldDataType dataType)
    {
        return new SimpleField(name, dataType)
        {
            IsFilterable = true,
            IsSortable = true,
            IsFacetable = true
        };
    }

    private SimpleField CreateBooleanField(string name)
    {
        return new SimpleField(name, SearchFieldDataType.Boolean)
        {
            IsFilterable = true
        };
    }

    private SearchField CreateVectorField(string name)
    {
        return new SearchField(name, SearchFieldDataType.Collection(SearchFieldDataType.Single))
        {
            IsSearchable = true,
            VectorSearchDimensions = VectorDimensions,
            VectorSearchProfileName = VectorProfileName
        };
    }

    private VectorSearch CreateVectorSearchConfiguration()
    {
        return new VectorSearch
        {
            Profiles = { new VectorSearchProfile(VectorProfileName, HnswConfigName) },
            Algorithms =
    {
        new HnswAlgorithmConfiguration(HnswConfigName)
        {
            Parameters = new HnswParameters
            {
                M = 4,
                EfConstruction = 400,
                EfSearch = 500,
                Metric = VectorSearchAlgorithmMetric.Cosine
            }
        }
    }
        };
    }

    private ScoringProfile CreateScoringProfile()
    {
        return new ScoringProfile("hotelBoost")
        {
            TextWeights = new TextWeights(new Dictionary<string, double>
            {
                [nameof(HotelSearchDocument.HotelName)] = 3.0,
                [nameof(HotelSearchDocument.Description)] = 2.0,
                [nameof(HotelSearchDocument.Amenities)] = 1.5,
                [nameof(HotelSearchDocument.CancellationPolicy)] = 1.0
            })
        };
    }
}
