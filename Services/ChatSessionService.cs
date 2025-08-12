using Microsoft.SemanticKernel.ChatCompletion;
using HotelReservationAgentChatBot.Models;
using System.Collections.Concurrent;

namespace HotelReservationAgentChatBot.Services;

public interface IChatSessionService
{
    ChatSession CreateSession();
    ChatSession? GetSession(string sessionId);
    void UpdateSessionAccess(string sessionId);
    bool DeleteSession(string sessionId);
    void CleanupExpiredSessions();
}

public class ChatSessionService : IChatSessionService
{
    private readonly ConcurrentDictionary<string, ChatSession> _sessions = new();
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(30); // 30 dakika timeout
    private readonly Timer _cleanupTimer;

    public ChatSessionService()
    {
        _cleanupTimer = new Timer(
            _ => CleanupExpiredSessions(),
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5)
        );
    }

    public ChatSession CreateSession()
    {
        var session = new ChatSession
        {
            Id = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow,
            ChatHistory = new ChatHistory()
        };

        _sessions[session.Id] = session;
        return session;
    }

    public ChatSession? GetSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            // Check if session is expired
            if (DateTime.UtcNow - session.LastAccessedAt > _sessionTimeout)
            {
                _sessions.TryRemove(sessionId, out _);
                return null;
            }
            return session;
        }
        return null;
    }

    public void UpdateSessionAccess(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.LastAccessedAt = DateTime.UtcNow;
        }
    }

    public bool DeleteSession(string sessionId)
    {
        return _sessions.TryRemove(sessionId, out _);
    }

    public void CleanupExpiredSessions()
    {
        var expiredSessions = _sessions
            .Where(kvp => DateTime.UtcNow - kvp.Value.LastAccessedAt > _sessionTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var sessionId in expiredSessions)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        if (expiredSessions.Any())
        {
            Console.WriteLine($"Cleaned up {expiredSessions.Count} expired sessions.");
        }
    }
}