using Microsoft.Extensions.VectorData;

namespace HotelReservationAgentChatBot.Models;

public class Hotel
{
    [VectorStoreKey]
    public string HotelId { get; set; } = default!;

    [VectorStoreData(IsIndexed = true)]
    public string HotelName { get; set; } = default!;

    [VectorStoreData(IsIndexed = true)]
    public string City { get; set; } = default!;

    [VectorStoreData(IsIndexed = true)]
    public string Country { get; set; } = default!;

    [VectorStoreData]
    public string Address { get; set; } = default!;

    [VectorStoreData(IsIndexed = true)]
    public int StarRating { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public double PricePerNight { get; set; }

    [VectorStoreData]
    public string Description { get; set; } = default!;

    [VectorStoreData]
    public string Amenities { get; set; } = default!;

    [VectorStoreData]
    public string RoomTypes { get; set; } = default!;

    [VectorStoreData]
    public string CancellationPolicy { get; set; } = default!;

    [VectorStoreData]
    public string CheckInCheckOut { get; set; } = default!;

    [VectorStoreData]
    public string HouseRules { get; set; } = default!;

    [VectorStoreData]
    public string NearbyAttractions { get; set; } = default!;

    [VectorStoreData(IsIndexed = true)]
    public bool HasPool { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public bool HasGym { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public bool HasSpa { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public bool PetFriendly { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public bool HasParking { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public bool HasWifi { get; set; }

    [VectorStoreVector(1536)]

    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }

    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> AmenitiesEmbedding { get; set; }
}
