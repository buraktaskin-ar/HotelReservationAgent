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
    AlertCircle,
    Plus,
    X
} from 'lucide-react';

const HotelChatBot = () => {
    const [chats, setChats] = useState([]);
    const [activeChat, setActiveChat] = useState(null);
    const [input, setInput] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const messagesEndRef = useRef(null);

    // API base URL
    const API_BASE_URL = 'https://localhost:7154/api/chat';

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [activeChat?.messages]);

    useEffect(() => {
        createNewChat();
    }, []);

    const generateChatId = () => {
        return 'chat_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    };

    const createNewChat = async () => {
        const newChatId = generateChatId();

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

            const newChat = {
                id: newChatId,
                sessionId: data.sessionId,
                title: `Sohbet ${chats.length + 1}`,
                createdAt: new Date(),
                messages: [{
                    role: 'assistant',
                    content: data.message,
                    timestamp: new Date()
                }],
                isConnected: true
            };

            const updatedChats = [newChat, ...chats];
            setChats(updatedChats);
            setActiveChat(newChat);

        } catch (error) {
            console.error('Error starting session:', error);
            setError('API sunucusuna bağlanılamıyor');

            const newChat = {
                id: newChatId,
                sessionId: null,
                title: `Sohbet ${chats.length + 1}`,
                createdAt: new Date(),
                messages: [{
                    role: 'system',
                    content: `Bağlantı hatası: API sunucusuna ulaşılamıyor.`,
                    timestamp: new Date()
                }],
                isConnected: false
            };

            const updatedChats = [newChat, ...chats];
            setChats(updatedChats);
            setActiveChat(newChat);

        } finally {
            setIsLoading(false);
        }
    };

    const deleteChat = (chatId, event) => {
        event.stopPropagation();
        const updatedChats = chats.filter(chat => chat.id !== chatId);
        setChats(updatedChats);

        if (activeChat?.id === chatId) {
            if (updatedChats.length > 0) {
                setActiveChat(updatedChats[0]);
            } else {
                createNewChat();
            }
        }
    };

    const sendMessage = async () => {
        if (!input.trim() || isLoading || !activeChat?.sessionId) return;

        const userMessage = {
            role: 'user',
            content: input,
            timestamp: new Date()
        };

        // Update active chat messages
        const updatedActiveChat = {
            ...activeChat,
            messages: [...activeChat.messages, userMessage]
        };
        setActiveChat(updatedActiveChat);

        // Update chats array
        const updatedChats = chats.map(chat =>
            chat.id === activeChat.id ? updatedActiveChat : chat
        );
        setChats(updatedChats);

        const currentInput = input;
        setInput('');
        setIsLoading(true);
        setError('');

        // Auto-update title if it's the first user message
        if (activeChat.messages.length === 1 && activeChat.title.startsWith('Sohbet')) {
            const shortTitle = currentInput.length > 20 ?
                currentInput.substring(0, 20) + '...' : currentInput;

            const titleUpdatedChat = { ...updatedActiveChat, title: shortTitle };
            setActiveChat(titleUpdatedChat);

            const titleUpdatedChats = chats.map(chat =>
                chat.id === activeChat.id ? titleUpdatedChat : chat
            );
            setChats(titleUpdatedChats);
        }

        try {
            const response = await fetch(`${API_BASE_URL}/${activeChat.sessionId}`, {
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

            const assistantMessage = {
                role: 'assistant',
                content: data.message,
                timestamp: new Date()
            };

            // Update messages with assistant response
            const finalActiveChat = {
                ...activeChat,
                messages: [...updatedActiveChat.messages, assistantMessage],
                title: activeChat.messages.length === 1 && activeChat.title.startsWith('Sohbet')
                    ? (currentInput.length > 20 ? currentInput.substring(0, 20) + '...' : currentInput)
                    : activeChat.title
            };
            setActiveChat(finalActiveChat);

            const finalChats = chats.map(chat =>
                chat.id === activeChat.id ? finalActiveChat : chat
            );
            setChats(finalChats);

        } catch (error) {
            console.error('Error sending message:', error);
            setError('Mesaj gönderilemedi');

            const errorMessage = {
                role: 'system',
                content: `Mesaj gönderilemedi: ${error.message}`,
                timestamp: new Date()
            };

            const errorActiveChat = {
                ...updatedActiveChat,
                messages: [...updatedActiveChat.messages, errorMessage]
            };
            setActiveChat(errorActiveChat);

            const errorChats = chats.map(chat =>
                chat.id === activeChat.id ? errorActiveChat : chat
            );
            setChats(errorChats);

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
                {line || '\u00A0'}
            </div>
        ));
    };

    const getMessageIcon = (role) => {
        switch (role) {
            case 'assistant':
                return (
                    <div className="w-8 h-8 bg-sky-600 rounded-full flex items-center justify-center">
                        <MessageCircle className="w-4 h-4 text-white" />
                    </div>
                );
            case 'user':
                return (
                    <div className="w-8 h-8 bg-sky-500 rounded-full flex items-center justify-center text-white text-sm font-bold">
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
        if (!isLoading && activeChat?.isConnected) {
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

    const formatChatTime = (date) => {
        if (!date) return '';
        const now = new Date();
        const diff = now - date;
        const minutes = Math.floor(diff / 60000);

        if (minutes < 1) return 'Şimdi';
        if (minutes < 60) return `${minutes}dk önce`;
        if (minutes < 1440) return `${Math.floor(minutes / 60)}sa önce`;
        return date.toLocaleDateString('tr-TR');
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-sky-50 via-blue-50 to-cyan-50">
            {/* Header */}
            <div className="bg-white shadow-lg border-b border-sky-200">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 py-4">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-4">
                            <div className="bg-gradient-to-r from-sky-600 to-cyan-600 rounded-xl p-3">
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
                            <button
                                onClick={createNewChat}
                                className="flex items-center space-x-2 bg-sky-600 hover:bg-sky-700 text-white px-4 py-2 rounded-lg transition-colors duration-200"
                            >
                                <Plus className="w-4 h-4" />
                                <span>Yeni Sohbet</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div className="max-w-7xl mx-auto px-4 sm:px-6 py-6">
                <div className="grid grid-cols-1 lg:grid-cols-5 gap-6">
                    {/* Chat History Sidebar */}
                    <div className="lg:col-span-1">
                        <div className="bg-white rounded-xl shadow-lg border border-sky-200 overflow-hidden">
                            <div className="p-4 border-b border-sky-100 bg-sky-50">
                                <h3 className="font-semibold text-gray-800 flex items-center">
                                    <MessageCircle className="w-4 h-4 mr-2 text-sky-600" />
                                    Sohbetler
                                </h3>
                            </div>
                            <div className="max-h-[600px] overflow-y-auto">
                                {chats.map((chat) => (
                                    <div
                                        key={chat.id}
                                        className={`p-3 border-b border-sky-50 cursor-pointer hover:bg-sky-50 transition-colors group relative ${activeChat?.id === chat.id ? 'bg-sky-100 border-l-4 border-l-sky-500' : ''
                                            }`}
                                        onClick={() => setActiveChat(chat)}
                                    >
                                        <div className="pr-8">
                                            <div className="text-sm font-medium text-gray-900 truncate mb-1">
                                                {chat.title}
                                            </div>
                                            <div className="text-xs text-gray-500 mb-1">
                                                {formatChatTime(chat.createdAt)}
                                            </div>
                                            <div className="flex items-center">
                                                <div className={`w-2 h-2 rounded-full mr-2 ${chat.isConnected ? 'bg-green-500' : 'bg-red-500'
                                                    }`}></div>
                                                <span className="text-xs text-gray-500">
                                                    {chat.messages.length} mesaj
                                                </span>
                                            </div>
                                        </div>
                                        {chats.length > 1 && (
                                            <button
                                                onClick={(e) => deleteChat(chat.id, e)}
                                                className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 text-red-500 hover:text-red-700 p-1 transition-all"
                                            >
                                                <X className="w-4 h-4" />
                                            </button>
                                        )}
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>

                    {/* Chat Area */}
                    <div className="lg:col-span-3">
                        <div className="bg-white rounded-2xl shadow-xl overflow-hidden border border-sky-200">
                            {/* Chat Header */}
                            <div className="bg-gradient-to-r from-sky-50 to-cyan-50 p-4 border-b border-sky-200">
                                <div className="flex items-center justify-between">
                                    <div>
                                        <h3 className="font-semibold text-gray-800">
                                            {activeChat?.title || 'Sohbet Seçin'}
                                        </h3>
                                        <div className="flex items-center mt-1">
                                            <div className={`w-2 h-2 rounded-full mr-2 ${activeChat?.isConnected ? 'bg-green-500' : 'bg-red-500'
                                                }`}></div>
                                            <span className="text-xs text-gray-500">
                                                {activeChat?.messages.length || 0} mesaj
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Messages Area */}
                            <div className="h-[480px] overflow-y-auto p-6 bg-gradient-to-b from-sky-50/30 to-blue-50/30">
                                <div className="space-y-4">
                                    {activeChat?.messages.map((message, index) => (
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
                                                        ? 'bg-gradient-to-r from-sky-600 to-cyan-600 text-white ml-auto'
                                                        : message.role === 'system'
                                                            ? 'bg-red-50 text-red-800 border border-red-200'
                                                            : 'bg-white text-gray-800 border border-sky-200 shadow-sm'
                                                    }`}
                                            >
                                                <div className="text-sm leading-relaxed whitespace-pre-wrap">
                                                    {formatMessage(message.content)}
                                                </div>
                                                <div className={`text-xs mt-2 flex items-center ${message.role === 'user' ? 'text-sky-200' : 'text-gray-500'
                                                    }`}>
                                                    <Clock className="inline w-3 h-3 mr-1" />
                                                    {message.timestamp && message.timestamp.toLocaleTimeString ?
                                                        message.timestamp.toLocaleTimeString('tr-TR', {
                                                            hour: '2-digit',
                                                            minute: '2-digit'
                                                        }) :
                                                        new Date().toLocaleTimeString('tr-TR', {
                                                            hour: '2-digit',
                                                            minute: '2-digit'
                                                        })
                                                    }
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
                                                <div className="w-8 h-8 bg-sky-600 rounded-full flex items-center justify-center animate-pulse">
                                                    <MessageCircle className="w-4 h-4 text-white" />
                                                </div>
                                            </div>
                                            <div className="bg-white border border-sky-200 rounded-2xl px-4 py-3 shadow-sm">
                                                <div className="flex space-x-1">
                                                    <div className="w-2 h-2 bg-sky-400 rounded-full animate-bounce"></div>
                                                    <div className="w-2 h-2 bg-sky-400 rounded-full animate-bounce"
                                                        style={{ animationDelay: '0.1s' }}></div>
                                                    <div className="w-2 h-2 bg-sky-400 rounded-full animate-bounce"
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
                                            placeholder="Otel aramak için mesajınızı yazın..."
                                            className="w-full px-4 py-3 border border-sky-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent resize-none transition-all duration-200"
                                            rows="2"
                                            disabled={isLoading || !activeChat?.isConnected}
                                        />
                                    </div>
                                    <button
                                        onClick={sendMessage}
                                        disabled={!input.trim() || isLoading || !activeChat?.isConnected}
                                        className="bg-gradient-to-r from-sky-600 to-cyan-600 hover:from-sky-700 hover:to-cyan-700 disabled:from-gray-400 disabled:to-gray-400 text-white p-3 rounded-xl transition-all duration-200 flex items-center justify-center shadow-lg disabled:shadow-none"
                                    >
                                        <Send className="w-5 h-5" />
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Sidebar */}
                    <div className="lg:col-span-1 space-y-6">
                        {/* Sample Queries */}
                        <div className="bg-white rounded-xl p-4 shadow-lg border border-sky-200">
                            <h3 className="font-semibold text-gray-800 mb-3 flex items-center">
                                <div className="w-2 h-2 rounded-full mr-2 bg-sky-500"></div>
                                Örnek Sorular
                            </h3>
                            <div className="space-y-2">
                                {sampleQueries.map((query, index) => (
                                    <button
                                        key={index}
                                        onClick={() => handleSampleQuery(query)}
                                        disabled={isLoading || !activeChat?.isConnected}
                                        className="w-full text-left px-3 py-2 text-sm bg-sky-50 text-sky-700 rounded-lg hover:bg-sky-100 transition-colors duration-200 disabled:opacity-50 disabled:cursor-not-allowed border border-sky-200"
                                    >
                                        {query}
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Features */}
                        <div className="bg-white rounded-xl p-4 shadow-lg border border-sky-200">
                            <h3 className="font-semibold text-gray-800 mb-3 flex items-center">
                                <div className="w-2 h-2 rounded-full mr-2 bg-sky-500"></div>
                                Özellikler
                            </h3>
                            <div className="space-y-3">
                                <div className="flex items-center space-x-2">
                                    <MapPin className="w-4 h-4 text-sky-600" />
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
                                    <Dog className="w-4 h-4 text-amber-600" />
                                    <span className="text-sm text-gray-600">Pet-Friendly</span>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <Heart className="w-4 h-4 text-sky-500" />
                                    <span className="text-sm text-gray-600">AI Önerileri</span>
                                </div>
                            </div>
                        </div>

                        {/* Tips */}
                        <div className="bg-gradient-to-br from-sky-50 to-cyan-50 rounded-xl p-4 border border-sky-200">
                            <h3 className="font-semibold text-sky-800 mb-2">💡 İpucu</h3>
                            <p className="text-sm text-sky-700">
                                Daha iyi sonuçlar için şehir adı, tarih aralığı ve tercihlerinizi belirtin.
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default HotelChatBot;