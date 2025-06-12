import { useState, useCallback, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Icon } from '@iconify/react';
import { useMessages } from '../../hooks/useMessages';
import Spinner from '../../components/common/spinner/Spinner';
import ConversationsSidebar from './conversations-sidebar/ConversationsSidebar';
import ChatMessagesSkeleton from './skeletons/ChatMessagesSkeleton';
import { useConversation } from '../../hooks/useConversation';
import { useAuthToken } from '../../hooks/useAuthToken';
import './ai-chat.css';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

const AiChatPage = () => {
    const [message, setMessage] = useState('');
    const [isInitialLoad, setIsInitialLoad] = useState(true);
    const [isSidebarExpanded, setIsSidebarExpanded] = useState(true);
    const { conversationId } = useParams<{ conversationId: string }>();
    const navigate = useNavigate();
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [retryCount, setRetryCount] = useState(0);
    const MAX_RETRIES = 3;

    const {
        conversations,
        loading: conversationsLoading,
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

            try {
                // Load conversations first
                await loadConversations();

                // Then validate the current conversation
                if (conversationId) {
                    // First check if conversation exists in our loaded conversations
                    const existingConversation = conversations.find(
                        (conv) => conv.id === conversationId
                    );
                    if (existingConversation) {
                        await loadMessages();
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
                        await loadMessages();
                    } catch (err) {
                        console.error('Failed to validate conversation:', err);
                        navigate('/select-chat', { replace: true });
                    }
                }
            } finally {
                // Only set initial load to false after everything is loaded
                setIsInitialLoad(false);
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
            if (retryCount < MAX_RETRIES) {
                // Retry loading messages after a delay
                const timer = setTimeout(() => {
                    setRetryCount((prev) => prev + 1);
                    loadMessages();
                }, 2000); // Wait 2 seconds before retrying

                return () => clearTimeout(timer);
            } else {
                // Only redirect after max retries
                navigate('/select-chat', { replace: true });
            }
        } else {
            // Reset retry count when messages load successfully
            setRetryCount(0);
        }
    }, [messagesError, navigate, retryCount, loadMessages]);

    // Scroll to bottom when messages change
    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSendMessage = useCallback(async () => {
        if (!message.trim() || !conversationId || !token) return;

        const trimmedMessage = message.trim();
        setMessage(''); // Clear input immediately
        try {
            await sendMessage(trimmedMessage);
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
        // Don't do anything if selecting the same conversation
        if (id === conversationId) return;

        setIsInitialLoad(true); // Reset loading state when switching conversations
        navigate(`/chat/${id}`);
    };

    const handleDeleteConversation = async (id: string) => {
        if (!token) return;

        try {
            await deleteConversation(id);
            // Find the next conversation to navigate to
            const currentIndex = conversations.findIndex(
                (conv) => conv.id === id
            );
            if (currentIndex !== -1) {
                setIsInitialLoad(true); // Reset loading state when switching conversations
                // If there's a next conversation, navigate to it
                if (currentIndex < conversations.length - 1) {
                    navigate(`/chat/${conversations[currentIndex + 1].id}`);
                } else if (currentIndex > 0) {
                    // If we're at the end, go to the previous one
                    navigate(`/chat/${conversations[currentIndex - 1].id}`);
                } else {
                    // If this was the last conversation, go to select chat
                    navigate('/select-chat');
                }
            }
        } catch (err) {
            console.error('Failed to delete conversation:', err);
        }
    };

    if (!conversationId) {
        return <div className="error">No conversation selected</div>;
    }

    const isLoading =
        isInitialLoad ||
        isTokenLoading ||
        conversationsLoading ||
        (messagesLoading && messages.length === 0);

    const renderMessages = () => {
        if (isLoading) {
            return <ChatMessagesSkeleton />;
        }

        if (messagesError) {
            return (
                <div className="error">
                    {retryCount < MAX_RETRIES
                        ? `Connection issue. Retrying... (${
                              retryCount + 1
                          }/${MAX_RETRIES})`
                        : 'Failed to load messages. Please try again later.'}
                </div>
            );
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
                    <p>
                        {msg.content ||
                            'No response was received from the AI. Please try again later.'}
                    </p>
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
                onNewChat={() => navigate('/select-chat')}
                loading={isLoading}
                isExpanded={isSidebarExpanded}
                onToggle={() => setIsSidebarExpanded((prev) => !prev)}
            />
            <div
                className={`chat-container ${
                    isSidebarExpanded ? 'with-sidebar' : ''
                }`}
            >
                <div className="messages-wrapper">
                    <div className="messages-content">
                        <div className="chat-messages">
                            {renderMessages()}
                            <div ref={messagesEndRef} />
                        </div>
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
                        {messagesLoading ? (
                            <Spinner size="small" />
                        ) : (
                            <Icon icon="material-symbols:send" />
                        )}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default AiChatPage;
