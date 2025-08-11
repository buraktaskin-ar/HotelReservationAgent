using System.ComponentModel;
using System.Text.Json.Serialization;


namespace HotelReservationAgentChatBot.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomType
{
    [Description("Standart Tek Kişilik")]
    StandardSingle = 1,

    [Description("Standart Çift Kişilik")]
    StandardDouble = 2,

    [Description("Deluxe Tek Kişilik")]
    DeluxeSingle = 3,

    [Description("Deluxe Çift Kişilik")]
    DeluxeDouble = 4,

 
}
