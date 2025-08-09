using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using HotelReservationAgentChatBot.Services;
using HotelReservationAgentChatBot.Models;

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

            // System prompt that guides the LLM to use the RAG search plugin
            session.ChatHistory.AddSystemMessage(
                @"You are a helpful hotel reservation assistant with access to a comprehensive hotel database through the search function.
                When users ask about hotels, accommodations, or any hotel-related queries, you should use the available search functions to find relevant information.
                Always provide accurate information based on the search results.
                Be conversational and helpful, and if you need more details to provide better recommendations, ask the user.
                Format your responses in a clear and friendly manner."
            );

            return Ok(new SessionResponse
            {
                SessionId = session.Id,
                Message = "Merhaba! Size otel bulma konusunda yardımcı olabilirim. Hangi şehirde, ne tür özelliklere sahip bir otel arıyorsunuz?"
            });
        }

        [HttpPost("{sessionId}")]
        public async Task<ActionResult<ChatResponse>> Chat(string sessionId, [FromBody] ChatRequest request)
        {
            // Get session
            var session = _sessionService.GetSession(sessionId);
            if (session == null)
            {
                return NotFound(new { error = "Session bulunamadı. Lütfen önce /api/chat/start-session ile yeni bir oturum başlatın." });
            }

            try
            {
                // Update session access time
                _sessionService.UpdateSessionAccess(sessionId);

                // Add user message to history
                session.ChatHistory.AddUserMessage(request.Message);

                // Configure execution settings to enable auto function calling
                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    Temperature = 0.7,
                    MaxTokens = 1000
                };

                // Get response from LLM with function calling
                var response = await _chatService.GetChatMessageContentsAsync(
                    session.ChatHistory,
                    executionSettings,
                    _kernel
                );

                var responseContent = response.FirstOrDefault()?.Content ?? "Üzgünüm, isteğinizi işleyemedim.";

                // Add assistant response to history
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

    // Request/Response Models
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