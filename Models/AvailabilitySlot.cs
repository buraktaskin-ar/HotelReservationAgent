 using System.Text.Json.Serialization;

namespace HotelReservationAgentChatBot.Models;

public class AvailabilitySlot
{


    [JsonPropertyName("start_date")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime End { get; set; }

    [JsonPropertyName("current_status")]
    public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Available;

    [JsonPropertyName("note")]
    public string? Note { get; set; }


}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AvailabilityStatus
{
    Available,
    Blocked,
    OutOfService,
    Reserved
}
