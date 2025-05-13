import { useState, useCallback, useEffect, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { Icon } from '@iconify/react';
import { useMessages } from '../conversations/hooks/useMessages';
import Spinner from '../../components/common/Spinner';
import './ai-chat.css';

const API_BASE_URL = process.env.VITE_API_BASE_URL || '';

const AiChatPage = () => {
    const { conversationId } = useParams<{ conversationId: string }>();
    const [message, setMessage] = useState('');
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { messages, loading, error, loadMessages, sendMessage } = useMessages(
        API_BASE_URL,
        conversationId || ''
    );

    useEffect(() => {
        if (conversationId) {
            loadMessages();
        }
    }, [conversationId, loadMessages]);

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    const handleSendMessage = useCallback(async () => {
        if (message.trim() && conversationId) {
            try {
                await sendMessage(message.trim());
                setMessage('');
            } catch (err) {
                // Error is handled by the hook
                console.error('Failed to send message:', err);
            }
        }
    }, [message, conversationId, sendMessage]);

    const handleKeyPress = useCallback(
        (e: React.KeyboardEvent) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                handleSendMessage();
            }
        },
        [handleSendMessage]
    );

    if (!conversationId) {
        return <div className="error">No conversation selected</div>;
    }

    return (
        <div className="chat-container">
            <div className="chat-messages">
                {loading && messages.length === 0 ? (
                    <Spinner />
                ) : error ? (
                    <div className="error">{error}</div>
                ) : messages.length === 0 ? (
                    <div className="no-messages">
                        No messages yet. Start the conversation!
                    </div>
                ) : (
                    messages.map((msg) => (
                        <div
                            key={msg.id}
                            className={`message ${
                                msg.role === 'user'
                                    ? 'user-message'
                                    : 'ai-message'
                            }`}
                        >
                            <div className="message-content">
                                <div className="message-header">
                                    <span className="message-role">
                                        {msg.role === 'user'
                                            ? 'You'
                                            : 'AI Assistant'}
                                    </span>
                                    <span className="message-time">
                                        {new Date(
                                            msg.createdAt
                                        ).toLocaleTimeString()}
                                    </span>
                                </div>
                                <p>{msg.content}</p>
                            </div>
                        </div>
                    ))
                )}
                <div ref={messagesEndRef} />
            </div>

            <div className="chat-input">
                <textarea
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    onKeyDown={handleKeyPress}
                    placeholder="Type your message..."
                    disabled={loading}
                />
                <button
                    onClick={handleSendMessage}
                    disabled={!message.trim() || loading}
                    className="send-button"
                >
                    <Icon icon="material-symbols:send" />
                </button>
            </div>
        </div>
    );
};

export default AiChatPage;
