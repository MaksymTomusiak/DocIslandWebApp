import { useState, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Icon } from '@iconify/react';
import { useConversation } from '../../hooks/useConversation';
import Spinner from '../../components/common/spinner/Spinner';
import './chat-selection.css';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';

console.log(API_BASE_URL);

const ChatSelectionPage = () => {
    const navigate = useNavigate();
    const [isDragging, setIsDragging] = useState(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const {
        conversations,
        loading,
        isCreatingChat,
        error,
        loadRecentConversations,
        createConversation,
    } = useConversation(API_BASE_URL);

    useEffect(() => {
        loadRecentConversations();
    }, [loadRecentConversations]);

    const handleDragOver = useCallback((e: React.DragEvent) => {
        e.preventDefault();
        setIsDragging(true);
    }, []);

    const handleDragLeave = useCallback((e: React.DragEvent) => {
        e.preventDefault();
        setIsDragging(false);
    }, []);

    const handleDrop = useCallback((e: React.DragEvent) => {
        e.preventDefault();
        setIsDragging(false);

        const files = e.dataTransfer.files;
        if (files.length > 0) {
            setSelectedFile(files[0]);
        }
    }, []);

    const handleFileSelect = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            const files = e.target.files;
            if (files && files.length > 0) {
                setSelectedFile(files[0]);
            }
        },
        []
    );

    const handleStartChat = useCallback(async () => {
        if (selectedFile) {
            try {
                const conversation = await createConversation(selectedFile);
                navigate(`/chat/${conversation.id}`);
            } catch (err) {
                console.error('Failed to create conversation:', err);
            }
        }
    }, [selectedFile, createConversation, navigate]);

    const handleContinueChat = (chatId: string) => {
        navigate(`/chat/${chatId}`);
    };

    return (
        <div className="chat-selection-container">
            <div className="selection-header">
                <h1>Start a New Chat</h1>
                <p>Upload a document or continue an existing chat</p>
            </div>
            <div className="selection-content">
                <div
                    className={`file-drop-zone ${isDragging ? 'dragging' : ''}`}
                    onDragOver={handleDragOver}
                    onDragLeave={handleDragLeave}
                    onDrop={handleDrop}
                >
                    <Icon
                        icon="material-symbols:upload-file"
                        className="upload-icon"
                    />
                    <p>Drag and drop your file here</p>
                    <p className="or-text">or</p>
                    <label className="file-input-label">
                        Choose File
                        <input
                            type="file"
                            className="file-input"
                            onChange={handleFileSelect}
                            accept=".pdf,.doc,.docx,.txt"
                        />
                    </label>
                    {selectedFile && (
                        <>
                            <div className="selected-file">
                                <Icon icon="material-symbols:description" />
                                <span>{selectedFile.name}</span>
                            </div>
                            <div className="start-chat-button-container">
                                <button
                                    className="start-chat-button"
                                    onClick={handleStartChat}
                                    disabled={isCreatingChat}
                                >
                                    {isCreatingChat ? (
                                        <Spinner size="small" />
                                    ) : (
                                        <>
                                            <span>Start Chat</span>
                                            <Icon icon="material-symbols:arrow-forward" />
                                        </>
                                    )}
                                </button>
                            </div>
                        </>
                    )}
                </div>
                <div className="chat-history-section">
                    <h2>Recent Chats</h2>
                    <div className="chat-history-list">
                        {loading ? (
                            <div className="loading-conversations">
                                <Spinner size="small" />
                                <span>Loading conversations...</span>
                            </div>
                        ) : error ? (
                            <p className="error">{error}</p>
                        ) : conversations.length === 0 ? (
                            <div className="no-conversations">
                                <Icon
                                    icon="mdi:chat-remove-outline"
                                    className="no-conv-icon"
                                />
                                No conversations yet
                            </div>
                        ) : (
                            conversations.map((chat) => (
                                <div
                                    key={chat.id}
                                    className="chat-history-item"
                                    onClick={() => handleContinueChat(chat.id)}
                                >
                                    <div className="chat-history-content">
                                        <h3>{chat.fileName}</h3>
                                        <p>
                                            Created:{' '}
                                            {new Date(
                                                chat.createdAt
                                            ).toLocaleDateString()}
                                        </p>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ChatSelectionPage;
