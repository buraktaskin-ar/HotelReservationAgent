namespace HotelReservationAgentChatBot.Models;

public static class RoomPricing
{


    public static readonly Dictionary<RoomType, decimal> BasePrices = new()
    {
       
        { RoomType.StandardSingle, 150m },
        { RoomType.StandardDouble, 200m },
        { RoomType.DeluxeSingle, 250m },
        { RoomType.DeluxeDouble, 350m },
       
    };
    public static readonly Dictionary<RoomType, int> Capacities = new()
    {
       
        { RoomType.StandardSingle, 1 },
        { RoomType.StandardDouble, 2 },
        { RoomType.DeluxeSingle, 1 },
        { RoomType.DeluxeDouble, 2 },
      
    };

    public const decimal SeaViewSurcharge = 50m;
    public const decimal CityViewSurcharge = 30m;
 
    public static decimal CalculatePrice(RoomType roomType, bool isSeaView = false,
        bool isCityView = false, bool hasBalcony = false, bool hasJacuzzi = false)
    {
        decimal basePrice = BasePrices.GetValueOrDefault(roomType, 150m);

        if (isSeaView) basePrice += SeaViewSurcharge;
        if (isCityView) basePrice += CityViewSurcharge;
     

        return basePrice;
    }


}
