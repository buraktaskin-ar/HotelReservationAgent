using Microsoft.SemanticKernel.ChatCompletion;

namespace HotelReservationAgentChatBot.Models;

public class ChatSession
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public ChatHistory ChatHistory { get; set; } = new();
}
