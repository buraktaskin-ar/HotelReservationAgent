// API configuration and utility functions
const API_CONFIG = {
    // Development URLs
    development: {
        https: 'https://localhost:7041/api/chat',
        http: 'http://localhost:5000/api/chat'
    },
    // Production URL (güncelleyin)
    production: 'https://your-production-api.com/api/chat'
};

// Current environment
const isDevelopment = process.env.NODE_ENV === 'development';

// Get API base URL
export const getApiBaseUrl = () => {
    if (isDevelopment) {
        // Development ortamında HTTPS'i tercih et, başarısız olursa HTTP dene
        return API_CONFIG.development.https;
    }
    return API_CONFIG.production;
};

// API request wrapper with error handling
export const apiRequest = async (endpoint, options = {}) => {
    const baseUrl = getApiBaseUrl();
    const url = `${baseUrl}${endpoint}`;

    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const requestOptions = {
        ...defaultOptions,
        ...options,
        headers: {
            ...defaultOptions.headers,
            ...options.headers,
        },
    };

    try {
        const response = await fetch(url, requestOptions);

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP ${response.status}: ${errorText}`);
        }

        return await response.json();
    } catch (error) {
        // Network error veya HTTPS sorunu durumunda HTTP'yi dene (sadece development'ta)
        if (isDevelopment && error.message.includes('Failed to fetch')) {
            console.warn('HTTPS connection failed, trying HTTP...');
            const httpUrl = url.replace(API_CONFIG.development.https, API_CONFIG.development.http);

            try {
                const httpResponse = await fetch(httpUrl, requestOptions);
                if (!httpResponse.ok) {
                    const errorText = await httpResponse.text();
                    throw new Error(`HTTP ${httpResponse.status}: ${errorText}`);
                }
                return await httpResponse.json();
            } catch (httpError) {
                throw new Error(`Both HTTPS and HTTP failed: ${error.message}, ${httpError.message}`);
            }
        }

        throw error;
    }
};

// Chat API functions
export const chatApi = {
    startSession: () => apiRequest('/start-session', { method: 'POST' }),

    sendMessage: (sessionId, message) =>
        apiRequest(`/${sessionId}`, {
            method: 'POST',
            body: JSON.stringify({ message })
        }),

    getHistory: (sessionId) => apiRequest(`/${sessionId}/history`),

    endSession: (sessionId) =>
        apiRequest(`/${sessionId}`, { method: 'DELETE' })
};

// Connection test function
export const testConnection = async () => {
    try {
        const result = await chatApi.startSession();
        return { success: true, data: result };
    } catch (error) {
        return { success: false, error: error.message };
    }
};

export default {
    getApiBaseUrl,
    apiRequest,
    chatApi,
    testConnection
};