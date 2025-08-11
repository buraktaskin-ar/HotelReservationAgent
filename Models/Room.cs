using System.Text.Json.Serialization;

namespace HotelReservationAgentChatBot.Models;
public class Room
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("room_number")]
    public string RoomNumber { get; set; } = string.Empty;

    [JsonPropertyName("floor")]
    public int Floor { get; set; }

    [JsonPropertyName("Hotel")]
    public Hotel Hotel { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }

    [JsonPropertyName("is_sea_view")]
    public bool IsSeaView { get; set; }

    [JsonPropertyName("is_city_view")]
    public bool IsCityView { get; set; }


    [JsonPropertyName("sea_view_surcharge")]
    public decimal SeaViewSurcharge { get; set; }

    [JsonPropertyName("city_view_surcharge")]
    public decimal CityViewSurcharge { get; set; }



    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    // NEW: room-scoped availability ledger
    [JsonPropertyName("availabilities")]
    public List<RoomAvailability> Availabilities { get; set; } = new();


    [JsonPropertyName("base_price")]
    public decimal BasePrice { get; set; }


    [JsonPropertyName("total_price")]
    public decimal TotalPrice
    {
        get
        {
            decimal price = BasePrice;

            if (IsSeaView) price += SeaViewSurcharge;
            if (IsCityView) price += CityViewSurcharge;
           

            return price;
        }
    }
}