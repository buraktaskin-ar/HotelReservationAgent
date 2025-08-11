import React, { useState, useEffect, useRef } from 'react';
import {
    Send,
    MessageCircle,
    Clock,
    MapPin,
    Star,
    Wifi,
    Car,
    Waves,
    Dumbbell,
    Heart,
    Dog,
    RefreshCw,
    AlertCircle
} from 'lucide-react';

const HotelChatBot = () => {
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState('');
    const [sessionId, setSessionId] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState('');
    const messagesEndRef = useRef(null);

    // API base URL - Kendi backend URL'nizi buraya yazın
    const API_BASE_URL = 'https://localhost:7154/api/chat';

    // Alternative URLs for different environments
    // const API_BASE_URL = 'http://localhost:5000/api/chat'; // HTTP version
    // const API_BASE_URL = 'https://your-production-url.com/api/chat'; // Production

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    useEffect(() => {
        startNewSession();
    }, []);

    const startNewSession = async () => {
        try {
            setIsLoading(true);
            setError('');

            const response = await fetch(`${API_BASE_URL}/start-session`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            setSessionId(data.sessionId);
            setMessages([{
                role: 'assistant',
                content: data.message,
                timestamp: new Date()
            }]);
            setIsConnected(true);
        } catch (error) {
            console.error('Error starting session:', error);
            setError('API sunucusuna bağlanılamıyor');
            setMessages([{
                role: 'system',
                content: `Bağlantı hatası: API sunucusuna ulaşılamıyor. 
        
Lütfen kontrol edin:
• Backend sunucusunun çalıştığından emin olun (${API_BASE_URL})
• CORS ayarlarının doğru olduğundan emin olun
• SSL sertifikası sorunları için tarayıcı ayarlarını kontrol edin`,
                timestamp: new Date()
            }]);
            setIsConnected(false);
        } finally {
            setIsLoading(false);
        }
    };

    const sendMessage = async () => {
        if (!input.trim() || isLoading || !sessionId) return;

        const userMessage = {
            role: 'user',
            content: input,
            timestamp: new Date()
        };
        setMessages(prev => [...prev, userMessage]);

        const currentInput = input;
        setInput('');
        setIsLoading(true);
        setError('');

        try {
            const response = await fetch(`${API_BASE_URL}/${sessionId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ message: currentInput }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP ${response.status}: ${errorText}`);
            }

            const data = await response.json();

            setMessages(prev => [...prev, {
                role: 'assistant',
                content: data.message,
                timestamp: new Date()
            }]);
        } catch (error) {
            console.error('Error sending message:', error);
            setError('Mesaj gönderilemedi');
            setMessages(prev => [...prev, {
                role: 'system',
                content: `Mesaj gönderilemedi: ${error.message}
        
Lütfen tekrar deneyin veya yeni bir oturum başlatın.`,
                timestamp: new Date()
            }]);
        } finally {
            setIsLoading(false);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    };

    const formatMessage = (content) => {
        return content.split('\n').map((line, index) => (
            <div key={index} className="mb-1">
                {line || '\u00A0'} {/* Non-breaking space for empty lines */}
            </div>
        ));
    };

    const getMessageIcon = (role) => {
        switch (role) {
            case 'assistant':
                return (
                    <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                        <MessageCircle className="w-4 h-4 text-white" />
                    </div>
                );
            case 'user':
                return (
                    <div className="w-8 h-8 bg-gray-600 rounded-full flex items-center justify-center text-white text-sm font-bold">
                        U
                    </div>
                );
            case 'system':
                return (
                    <div className="w-8 h-8 bg-red-500 rounded-full flex items-center justify-center">
                        <AlertCircle className="w-4 h-4 text-white" />
                    </div>
                );
            default:
                return (
                    <div className="w-8 h-8 bg-gray-400 rounded-full flex items-center justify-center">
                        <MessageCircle className="w-4 h-4 text-white" />
                    </div>
                );
        }
    };

    const handleSampleQuery = (query) => {
        if (!isLoading && isConnected) {
            setInput(query);
        }
    };

    const sampleQueries = [
        "İstanbul'da havuzlu otel arıyorum",
        "Antalya'da 5 yıldızlı spa'lı otel",
        "Kapadokya'da evcil hayvan dostu otel",
        "Bodrum'da deniz manzaralı otel",
        "Ankara'da iş seyahati için otel",
        "Pamukkale yakınında otel öner"
    ];

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-indigo-50">
            {/* Header */}
            <div className="bg-white shadow-lg border-b border-gray-200">
                <div className="max-w-6xl mx-auto px-4 sm:px-6 py-4">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-4">
                            <div className="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-xl p-3">
                                <MessageCircle className="w-6 h-6 text-white" />
                            </div>
                            <div>
                                <h1 className="text-2xl font-bold text-gray-900">
                                    Hotel Rezervasyon Asistanı
                                </h1>
                                <p className="text-sm text-gray-600">
                                    AI destekli akıllı otel arama ve rezervasyon sistemi
                                </p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-4">
                            <div className="flex items-center space-x-2">
                                <div className={`w-3 h-3 rounded-full ${isConnected ? 'bg-green-500' : 'bg-red-500'
                                    }`}></div>
                                <span className="text-sm text-gray-600 hidden sm:inline">
                                    {isConnected ? 'Bağlı' : 'Bağlantı yok'}
                                </span>
                            </div>
                            {error && (
                                <button
                                    onClick={startNewSession}
                                    className="flex items-center space-x-1 text-blue-600 hover:text-blue-700 text-sm"
                                >
                                    <RefreshCw className="w-4 h-4" />
                                    <span className="hidden sm:inline">Yeniden Bağlan</span>
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            </div>

            <div className="max-w-6xl mx-auto px-4 sm:px-6 py-6">
                <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
                    {/* Chat Area */}
                    <div className="lg:col-span-3">
                        <div className="bg-white rounded-2xl shadow-xl overflow-hidden border border-gray-200">
                            {/* Messages Area */}
                            <div className="h-96 lg:h-[500px] overflow-y-auto p-6 bg-gray-50/50">
                                <div className="space-y-4">
                                    {messages.map((message, index) => (
                                        <div
                                            key={index}
                                            className={`flex items-start space-x-3 ${message.role === 'user' ? 'justify-end' : 'justify-start'
                                                }`}
                                        >
                                            {message.role !== 'user' && (
                                                <div className="flex-shrink-0 mt-1">
                                                    {getMessageIcon(message.role)}
                                                </div>
                                            )}

                                            <div
                                                className={`max-w-xs sm:max-w-md lg:max-w-lg px-4 py-3 rounded-2xl ${message.role === 'user'
                                                        ? 'bg-gradient-to-r from-blue-600 to-indigo-600 text-white ml-auto'
                                                        : message.role === 'system'
                                                            ? 'bg-red-50 text-red-800 border border-red-200'
                                                            : 'bg-white text-gray-800 border border-gray-200 shadow-sm'
                                                    }`}
                                            >
                                                <div className="text-sm leading-relaxed whitespace-pre-wrap">
                                                    {formatMessage(message.content)}
                                                </div>
                                                <div className={`text-xs mt-2 flex items-center ${message.role === 'user' ? 'text-blue-200' : 'text-gray-500'
                                                    }`}>
                                                    <Clock className="inline w-3 h-3 mr-1" />
                                                    {message.timestamp?.toLocaleTimeString('tr-TR', {
                                                        hour: '2-digit',
                                                        minute: '2-digit'
                                                    })}
                                                </div>
                                            </div>

                                            {message.role === 'user' && (
                                                <div className="flex-shrink-0 mt-1">
                                                    {getMessageIcon(message.role)}
                                                </div>
                                            )}
                                        </div>
                                    ))}

                                    {isLoading && (
                                        <div className="flex items-start space-x-3">
                                            <div className="flex-shrink-0 mt-1">
                                                <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center animate-pulse">
                                                    <MessageCircle className="w-4 h-4 text-white" />
                                                </div>
                                            </div>
                                            <div className="bg-white border border-gray-200 rounded-2xl px-4 py-3 shadow-sm">
                                                <div className="flex space-x-1">
                                                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                                                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
                                                        style={{ animationDelay: '0.1s' }}></div>
                                                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
                                                        style={{ animationDelay: '0.2s' }}></div>
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                </div>
                                <div ref={messagesEndRef} />
                            </div>

                            {/* Input Area */}
                            <div className="border-t bg-white p-4">
                                <div className="flex space-x-3">
                                    <div className="flex-1 relative">
                                        <textarea
                                            value={input}
                                            onChange={(e) => setInput(e.target.value)}
                                            onKeyPress={handleKeyPress}
                                            placeholder="Otel aramak için mesajınızı yazın... (örn: 'İstanbul'da 4 yıldızlı spa'lı otel arıyorum')"
                                            className="w-full px-4 py-3 border border-gray-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none transition-all duration-200"
                                            rows="2"
                                            disabled={isLoading || !isConnected}
                                        />
                                    </div>
                                    <button
                                        onClick={sendMessage}
                                        disabled={!input.trim() || isLoading || !isConnected}
                                        className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 disabled:from-gray-400 disabled:to-gray-400 text-white p-3 rounded-xl transition-all duration-200 flex items-center justify-center shadow-lg disabled:shadow-none"
                                    >
                                        <Send className="w-5 h-5" />
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Sidebar */}
                    <div className="space-y-6">
                        {/* Connection Status */}
                        <div className="bg-white rounded-xl p-4 shadow-lg border border-gray-200">
                            <h3 className="font-semibold text-gray-800 mb-3 flex items-center">
                                <div className={`w-2 h-2 rounded-full mr-2 ${isConnected ? 'bg-green-500' : 'bg-red-500'
                                    }`}></div>
                                Bağlantı Durumu
                            </h3>
                            <div className="text-sm text-gray-600">
                                {isConnected ? (
                                    <span className="text-green-600">✓ API ile bağlantı aktif</span>
                                ) : (
                                    <div>
                                        <span className="text-red-600">✗ Bağlantı kurulamadı</span>
                                        <br />
                                        <span className="text-xs">{API_BASE_URL}</span>
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Sample Queries */}
                        <div className="bg-white rounded-xl p-4 shadow-lg border border-gray-200">
                            <h3 className="font-semibold text-gray-800 mb-3">Örnek Sorular:</h3>
                            <div className="space-y-2">
                                {sampleQueries.map((query, index) => (
                                    <button
                                        key={index}
                                        onClick={() => handleSampleQuery(query)}
                                        disabled={isLoading || !isConnected}
                                        className="w-full text-left px-3 py-2 text-sm bg-blue-50 text-blue-700 rounded-lg hover:bg-blue-100 transition-colors duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        {query}
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Features */}
                        <div className="bg-white rounded-xl p-4 shadow-lg border border-gray-200">
                            <h3 className="font-semibold text-gray-800 mb-3">Özellikler:</h3>
                            <div className="space-y-3">
                                <div className="flex items-center space-x-2">
                                    <MapPin className="w-4 h-4 text-blue-600" />
                                    <span className="text-sm text-gray-600">Lokasyon Arama</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Star className="w-4 h-4 text-yellow-500" />
                                    <span className="text-sm text-gray-600">Yıldız Filtreleme</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Waves className="w-4 h-4 text-cyan-500" />
                                    <span className="text-sm text-gray-600">Havuz/Spa Arama</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Car className="w-4 h-4 text-gray-600" />
                                    <span className="text-sm text-gray-600">Otopark Bilgisi</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Dog className="w-4 h-4 text-brown-600" />
                                    <span className="text-sm text-gray-600">Pet-Friendly</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Heart className="w-4 h-4 text-red-500" />
                                    <span className="text-sm text-gray-600">AI Önerileri</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default HotelChatBot;