import { useState, useCallback, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Icon } from '@iconify/react';
import { useMessages } from '../../hooks/useMessages';
import Spinner from '../../components/common/spinner/Spinner';
import ConversationsSidebar from '../ai-chat/conversations-sidebar/ConversationsSidebar';
import { useConversation } from '../../hooks/useConversation';
import { useAuthToken } from '../../hooks/useAuthToken';
import './ai-chat.css';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

const AiChatPage = () => {
    const [message, setMessage] = useState('');
    const { conversationId } = useParams<{ conversationId: string }>();
    const navigate = useNavigate();
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { token, isLoading: isTokenLoading } = useAuthToken();

    const {
        conversations,
        loadConversations,
        getConversationById,
        deleteConversation,
    } = useConversation(API_BASE_URL);

    const {
        messages,
        loading: messagesLoading,
        error: messagesError,
        loadMessages,
        sendMessage,
    } = useMessages(API_BASE_URL, conversationId || '');

    // Initial load and validation
    useEffect(() => {
        const initializeChat = async () => {
            if (!token) return;

            // Load conversations first
            await loadConversations();

            // Then validate the current conversation
            if (conversationId) {
                // First check if conversation exists in our loaded conversations
                const existingConversation = conversations.find(
                    (conv) => conv.id === conversationId
                );
                if (existingConversation) {
                    loadMessages();
                    return;
                }

                // If not found in loaded conversations, try to fetch it
                try {
                    const conversation = await getConversationById(
                        conversationId
                    );
                    if (!conversation) {
                        navigate('/select-chat', { replace: true });
                        return;
                    }
                    loadMessages();
                } catch (err) {
                    console.error('Failed to validate conversation:', err);
                    navigate('/select-chat', { replace: true });
                }
            }
        };

        if (!isTokenLoading) {
            initializeChat();
        }
    }, [
        conversationId,
        loadConversations,
        getConversationById,
        loadMessages,
        navigate,
        token,
        isTokenLoading,
    ]);

    // Handle message loading errors
    useEffect(() => {
        if (messagesError) {
            navigate('/select-chat', { replace: true });
        }
    }, [messagesError, navigate]);

    // Scroll to bottom when messages change
    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSendMessage = useCallback(async () => {
        if (!message.trim() || !conversationId || !token) return;

        try {
            await sendMessage(message.trim());
            setMessage('');
        } catch (err) {
            console.error('Failed to send message:', err);
        }
    }, [message, conversationId, sendMessage, token]);

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    const handleSelectConversation = (id: string) => {
        navigate(`/chat/${id}`);
    };

    const handleDeleteConversation = async (id: string) => {
        if (!token) return;

        try {
            await deleteConversation(id);
        } catch (err) {
            console.error('Failed to delete conversation:', err);
        }
        if (id === conversationId) {
            navigate('/select-chat', { replace: true });
        }
    };

    if (!conversationId) {
        return <div className="error">No conversation selected</div>;
    }

    if (isTokenLoading) {
        return <Spinner />;
    }

    const renderMessages = () => {
        if (messagesLoading && messages.length === 0) {
            return <Spinner />;
        }

        if (messagesError) {
            return <div className="error">{messagesError}</div>;
        }

        if (messages.length === 0) {
            return (
                <div className="no-messages">
                    No messages yet. Start the conversation!
                </div>
            );
        }

        return messages.map((msg) => (
            <div
                key={msg.id}
                className={`message ${
                    msg.isResponse ? 'ai-message' : 'user-message'
                }`}
            >
                <div className="message-content">
                    <div className="message-header">
                        <span className="message-role">
                            {msg.isResponse ? 'AI Assistant' : 'You'}
                        </span>
                        <span className="message-time">
                            {new Date(msg.createdAt).toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}
                        </span>
                    </div>
                    <p>{msg.content}</p>
                </div>
            </div>
        ));
    };

    return (
        <div className="chat-page-container">
            <ConversationsSidebar
                conversations={conversations}
                onSelectConversation={handleSelectConversation}
                onDeleteConversation={handleDeleteConversation}
                currentConversationId={conversationId}
            />
            <div className="chat-container">
                <div className="messages-wrapper">
                    <div className="chat-messages">
                        {renderMessages()}
                        <div ref={messagesEndRef} />
                    </div>
                </div>
                <div className="chat-input">
                    <textarea
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        onKeyDown={handleKeyPress}
                        placeholder="Type your message..."
                        disabled={messagesLoading}
                    />
                    <button
                        onClick={handleSendMessage}
                        disabled={!message.trim() || messagesLoading || !token}
                        className="send-button"
                    >
                        <Icon icon="material-symbols:send" />
                    </button>
                </div>
            </div>
        </div>
    );
};

export default AiChatPage;
