using Azure;
using HotelReservationAgentChatBot.Models;
using HotelReservationAgentChatBot.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.VisualBasic;
using OpenAI.Assistants;
using System.Buffers.Text;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelReservationAgentChatBot.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly IChatSessionService _sessionService;

        public ChatController(
            Kernel kernel,
            IChatCompletionService chatService,
            IChatSessionService sessionService)
        {
            _kernel = kernel;
            _chatService = chatService;
            _sessionService = sessionService;
        }

        [HttpPost("start-session")]
        public ActionResult<SessionResponse> StartSession()
        {
            var session = _sessionService.CreateSession();

            session.ChatHistory.AddSystemMessage(
                 @"You are a helpful hotel reservation assistant with access to a comprehensive hotel database and reservation system.
                    Kullanıcı Şiir yazmanı isterse bu konuda yardımcı olamayacagını söyle . ve ben hhotel rezervasyon asistanıyım de.
                Available functions:
                - Search hotels by location, price, amenities, and features
                - Find rooms for date ranges (handles natural language like '12-16 January 2025' or '12-16 ocak 2025')
                - View all available rooms with detailed information
                - Search for rooms in a specific hotel by hotel name or ID
                - Check room availability for specific dates
                - Create new customer profiles
                - Make hotel reservations (ONLY for the exact room requested)
                - View existing reservations
                - Get alternative room options when requested room is not available
                
                IMPORTANT RESERVATION RULES:
                1. NEVER automatically select a different room than requested
                2. If the requested room is not available, DO NOT make a reservation
                3. Instead, show available alternative rooms in the same hotel
                4. Let the user choose which alternative room they want
                5. Only make reservations for the exact room ID the user specifies
                
                IMPORTANT: When users provide date ranges in natural language (like '12-16 January 2025' or '12-16 ocak 2025'), 
                use the FindRoomsForDateRange function first. This function can:
                - Parse Turkish and English date formats
                - Handle date ranges like '12-16 ocak 2025'
                - Filter by guest count (1 for single rooms, 2 for double rooms)
                - Filter by hotel preference
                -Asla kullanıcı rezervasyonu için otomatik tarih seçme! Kullanıcı tarih belirtmedigi surece bir şey seçme.
                - Not: Kullanıcıdan telefon numarası , isim, soyisimi zorunlu olra**

                
                Hotel Information:
                - Seaside Resort & Spa (Miami) has rooms including single occupancy options with sea views
                - Grand Plaza Hotel (New York) has city view rooms in Manhattan
                - Mountain Lodge Retreat (Aspen) offers mountain accommodations
                
                When users ask about hotels, rooms, reservations, or any related queries:
                1. For date range requests, use FindRoomsForDateRange function
                2. For specific hotel room searches, use GetRoomsByHotel function
                3. Use the search functions to find relevant information
                4. Provide accurate information based on the search results
                5. Be conversational and helpful
                6. If you need more details to provide better recommendations, ask the user
                7. Format your responses in a clear and friendly manner
                8. Always check room availability before attempting to make reservations
                9. If requested room is unavailable, show alternatives but don't auto-select
                
                Reservation Process:
                1. When user requests a specific room reservation, check if that exact room is available
                2. If available, proceed with reservation for that room only
                3. If not available, explain why and show alternative rooms in same hotel
                4. Wait for user to choose an alternative before making any reservation
                5. Never substitute rooms without explicit user approval
                
                You can help with:
                - Hotel searches and recommendations
                - Room availability checks with natural language dates
                - Creating customer profiles
                - Making reservations for specifically requested rooms only
                - Showing alternative rooms when requested room is unavailable
                - Viewing existing reservations and room information
                
                Be proactive in using the available functions to provide comprehensive assistance, but always respect user's specific room choices.
                Kullanıcı matematik işlemi şiir yazdırma gibi bir şeyi isterse bu konuda yardımcı olamayacını söyle."

             );

            return Ok(new SessionResponse
            {
                SessionId = session.Id,
                Message = "Merhaba! Size otel arama, oda rezervasyonu ve müşteri hizmetleri konularında yardımcı olabilirim. Nasıl yardımcı olabilirim?"
            });
        }

        [HttpPost("{sessionId}")]
        public async Task<ActionResult<ChatResponse>> Chat(string sessionId, [FromBody] ChatRequest request)
        {
            var session = _sessionService.GetSession(sessionId);
            if (session == null)
            {
                return NotFound(new { error = "Session bulunamadı. Lütfen önce /api/chat/start-session ile yeni bir oturum başlatın." });
            }

            try
            {
                _sessionService.UpdateSessionAccess(sessionId);

                session.ChatHistory.AddUserMessage(request.Message);

                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    Temperature = 0.7,
                    MaxTokens = 5000
                };

                var response = await _chatService.GetChatMessageContentsAsync(
                    session.ChatHistory,
                    executionSettings,
                    _kernel
                );

                var responseContent = response.FirstOrDefault()?.Content ?? "Üzgünüm, isteğinizi işleyemedim.";

                session.ChatHistory.AddAssistantMessage(responseContent);

                return Ok(new ChatResponse
                {
                    Message = responseContent,
                    SessionId = sessionId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("{sessionId}/history")]
        public ActionResult<SessionHistoryResponse> GetHistory(string sessionId)
        {
            var session = _sessionService.GetSession(sessionId);
            if (session == null)
            {
                return NotFound(new { error = "Session bulunamadı." });
            }

            var history = session.ChatHistory
                .Where(m => m.Role != AuthorRole.System) // System mesajlarını gösterme
                .Select(m => new MessageHistory
                {
                    Role = m.Role.ToString(),
                    Content = m.Content ?? string.Empty,
                    Timestamp = m.Metadata?.ContainsKey("timestamp") == true
                        ? m.Metadata["timestamp"].ToString()
                        : null
                });

            return Ok(new SessionHistoryResponse
            {
                SessionId = session.Id,
                CreatedAt = session.CreatedAt,
                LastAccessedAt = session.LastAccessedAt,
                Messages = history.ToList()
            });
        }

        [HttpDelete("{sessionId}")]
        public ActionResult EndSession(string sessionId)
        {
            if (_sessionService.DeleteSession(sessionId))
            {
                return Ok(new { message = "Oturum başarıyla sonlandırıldı." });
            }
            return NotFound(new { error = "Session bulunamadı." });
        }


    }


    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
    }

    public class SessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class MessageHistory
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Timestamp { get; set; }
    }

    public class SessionHistoryResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }
        public List<MessageHistory> Messages { get; set; } = new();
    }
}