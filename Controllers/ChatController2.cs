//using Microsoft.AspNetCore.Mvc;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;

//namespace HotelReservationAgentChatBot.Controllers
//{
//    [ApiController]
//    [Route("api/chat")]
//    public class ChatController2 : ControllerBase
//    {
//        private readonly Kernel _kernel;
//        private readonly IChatCompletionService _chatService;

//        public ChatController2(
//            Kernel kernel,
//            IChatCompletionService chatService)
//        {
//            _kernel = kernel;
//            _chatService = chatService;
//        }

//        [HttpPost]
//        public async Task<ActionResult<ChatResponse2>> Chat([FromBody] ChatRequest5 request)
//        {
//            try
//            {
//                // Her request için yeni ChatHistory oluştur
//                var chatHistory = new ChatHistory();

//                // System message ekle
//                chatHistory.AddSystemMessage(GetSystemMessage());

//                // Eğer önceki konuşma geçmişi varsa, ekle
//                if (request.ConversationHistory != null && request.ConversationHistory.Any())
//                {
//                    foreach (var message in request.ConversationHistory)
//                    {
//                        if (message.Role.ToLower() == "user")
//                            chatHistory.AddUserMessage(message.Content);
//                        else if (message.Role.ToLower() == "assistant")
//                            chatHistory.AddAssistantMessage(message.Content);
//                    }
//                }

//                // Mevcut kullanıcı mesajını ekle
//                chatHistory.AddUserMessage(request.Message);

//                var executionSettings = new OpenAIPromptExecutionSettings
//                {
//                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
//                    Temperature = 0.7,
//                    MaxTokens = 5000
//                };

//                var response = await _chatService.GetChatMessageContentsAsync(
//                    chatHistory,
//                    executionSettings,
//                    _kernel
//                );

//                var responseContent = response.FirstOrDefault()?.Content ??
//                    "Üzgünüm, isteğinizi işleyemedim.";

//                return Ok(new ChatResponse5
//                {
//                    Message = responseContent,
//                    // Güncellenmiş konuşma geçmişini geri döndür
//                    ConversationHistory = chatHistory
//                        .Where(m => m.Role != AuthorRole.System)
//                        .Select(m => new MessageHistory
//                        {
//                            Role = m.Role.ToString(),
//                            Content = m.Content ?? string.Empty
//                        }).ToList()
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Bir hata oluştu: {ex.Message}" });
//            }
//        }

//        private string GetSystemMessage()
//        {
//            return @"You are an experienced, friendly hotel concierge who genuinely cares about 
//                    guest satisfaction. You're knowledgeable but not overwhelming, professional 
//                    but warm. You speak like a helpful friend who happens to be an expert.

//                    COMMUNICATION STYLE:
//                    - Use warm, welcoming language
//                    - Be thorough but not verbose  
//                    - Show enthusiasm for helping
//                    - Use 'we' when referring to hotel services
//                    - Always end with a helpful question or offer

//                    ROLE RESTRICTIONS:
//                    You are EXCLUSIVELY a hotel concierge assistant. You ONLY help with hotel-related services:
//                    - Hotel searches and reservations
//                    - Room availability and booking
//                    - Travel accommodation advice
//                    - Hotel amenities and services information
        
//                    You do NOT:
//                    - Write poems, songs, stories, or creative content
//                    - Provide general information unrelated to hotels
//                    - Help with non-hotel related tasks
//                    - Answer questions outside hospitality domain
        
//                    If users ask for anything outside hotel services, politely redirect them:
//                    'I'm your hotel concierge assistant, specialized in helping you find and book the perfect accommodation. 
//                    How can I assist you with your hotel needs today?'
                
//                Available functions:
//                - Search hotels by location, price, amenities, and features
//                - Find rooms for date ranges (handles natural language like '12-16 January 2025' or '12-16 ocak 2025')
//                - View all available rooms with detailed information
//                - Search for rooms in a specific hotel by hotel name or ID
//                - Check room availability for specific dates
//                - Create new customer profiles
//                - Make hotel reservations (ONLY for the exact room requested)
//                - View existing reservations
//                - Get alternative room options when requested room is not available
                
//                IMPORTANT RESERVATION RULES:
//                1. NEVER automatically select a different room than requested
//                2. If the requested room is not available, DO NOT make a reservation
//                3. Instead, show available alternative rooms in the same hotel
//                4. Let the user choose which alternative room they want
//                5. Only make reservations for the exact room ID the user specifies
                
//                IMPORTANT: When users provide date ranges in natural language (like '12-16 January 2025' or '12-16 ocak 2025'), 
//                use the FindRoomsForDateRange function first.";
//        }
//    }

//    // Request model - Client'tan gelen veri
//    public class ChatRequest5
//    {
//        public string Message { get; set; } = string.Empty;
//        public List<MessageHistory>? ConversationHistory { get; set; }
//    }

//    // Response model - Client'a dönen veri
//    public class ChatResponse5
//    {
//        public string Message { get; set; } = string.Empty;
//        public List<MessageHistory> ConversationHistory { get; set; } = new();
//    }

//    // Conversation history'deki her mesaj
//    public class MessageHistory5
//    {
//        public string Role { get; set; } = string.Empty; // "user" veya "assistant"
//        public string Content { get; set; } = string.Empty;
//    }
//}