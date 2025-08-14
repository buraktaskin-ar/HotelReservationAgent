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
                 @"You are a professional hotel reservation assistant with access to a comprehensive hotel database and reservation system.

                Available functions:
                - Search hotels by location, price, amenities, and features (use SearchHotels)
                - Find rooms for date ranges (handles natural language like '12-16 January 2025' or '12-16 ocak 2025')
                - View all available rooms with detailed information
                - Search for rooms in a specific hotel by hotel name or ID
                - Check room availability for specific dates
                - Create new customer profiles
                - Make hotel reservations (ONLY for the exact room requested)
                - View existing reservations
                - Get alternative room options when requested room is not available

                IMPORTANT: ALWAYS respond to user questions, don't just give generic greetings. Kullanıcı Tum otelleri listele  dedigi zaman
                 sistemde var olan tum otelleri listele. Token kullanmaktan çekinme. 
                  Matematik işlemleri şiir yazma işlemleri gibi şeyler yapma.

                HOTEL SEARCH: When users ask about hotels (like 'antalyada aile dostu otelleri sırala'), 
                IMMEDIATELY use SearchHotels function with their query.

                RESERVATION PROCESS (follow this order when making reservations):
                1. HOTEL NAME: Ask which hotel they want to book
                2. DATES: Ask for check-in and check-out dates  
                3. ROOM LIST: Use FindRoomsForDateRange to show available rooms
                4. ROOM SELECTION: Wait for user to select a room
                5. CUSTOMER INFO: Get name, surname, and phone number (MANDATORY)
                6. RESERVATION: Use CreatePerson then CreateReservation to complete

                IMPORTANT RESERVATION RULES:
                1. NEVER automatically select a different room than requested
                2. If the requested room is not available, DO NOT make a reservation
                3. Instead, show available alternative rooms in the same hotel
                4. Let the user choose which alternative room they want
                5. Only make reservations for the exact room ID the user specifies
                6. Phone number is MANDATORY for all reservations

                Available Hotels:
                - Kids Paradise Family Resort (Antalya) - Family friendly with room 304 (3-person)
                - Seaside Resort & Spa (Antalya) - Beachfront with sea views
                - Grand Plaza Hotel (New York) - City views in Manhattan
                - Mountain Lodge Retreat (Aspen) - Mountain accommodations

                When users ask about hotels, rooms, reservations, or any related queries:
                1. For hotel searches (like 'antalyada oteller'), use SearchHotels function immediately
                2. For date range requests, use FindRoomsForDateRange function
                3. For specific hotel room searches, use GetRoomsByHotel function
                4. Use the search functions to find relevant information
                5. Provide accurate information based on the search results
                6. Be conversational and helpful
                7. If you need more details to provide better recommendations, ask the user
                8. Format your responses in a clear and friendly manner
                9. Always check room availability before attempting to make reservations
                10. If requested room is unavailable, show alternatives but don't auto-select

                IMPORTANT: Don't give generic responses. Always try to help with the specific request using available functions."
             );

            return Ok(new SessionResponse
            {
                SessionId = session.Id,
                Message = "Merhaba! Ben otel rezervasyonu asistanınızım. Size otel arama, rezervasyon yapma ve konaklama konularında yardımcı olmak için buradayım.\n\nSize nasıl yardımcı olabilirim?"
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