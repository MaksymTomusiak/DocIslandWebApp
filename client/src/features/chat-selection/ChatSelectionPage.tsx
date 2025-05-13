import { useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Icon } from '@iconify/react';
import './chat-selection.css';

interface ChatHistory {
    id: string;
    title: string;
    lastMessage: string;
    date: string;
}

const ChatSelectionPage = () => {
    const navigate = useNavigate();
    const [isDragging, setIsDragging] = useState(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);

    // Mock chat history - replace with actual data from your backend
    const chatHistory: ChatHistory[] = [
        {
            id: '1',
            title: 'Project Documentation',
            lastMessage: 'What are the main features?',
            date: '2024-03-20',
        },
        {
            id: '2',
            title: 'Research Paper',
            lastMessage: 'Can you summarize the methodology?',
            date: '2024-03-19',
        },
    ];

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

    const handleStartChat = useCallback(() => {
        if (selectedFile) {
            // Here you would typically upload the file and get a chat ID
            // For now, we'll use a mock ID
            const chatId = 'new-' + Date.now();
            navigate(`/chat/${chatId}`);
        }
    }, [selectedFile, navigate]);

    const handleContinueChat = useCallback(
        (chatId: string) => {
            navigate(`/chat/${chatId}`);
        },
        [navigate]
    );

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
                        <div className="selected-file">
                            <Icon icon="material-symbols:description" />
                            <span>{selectedFile.name}</span>
                        </div>
                    )}
                </div>

                <div className="chat-history">
                    <h2>Recent Chats</h2>
                    {chatHistory.map((chat) => (
                        <div
                            key={chat.id}
                            className="chat-history-item"
                            onClick={() => handleContinueChat(chat.id)}
                        >
                            <div className="chat-history-content">
                                <h3>{chat.title}</h3>
                                <p>{chat.lastMessage}</p>
                                <span className="chat-date">{chat.date}</span>
                            </div>
                            <Icon icon="material-symbols:chevron-right" />
                        </div>
                    ))}
                </div>
            </div>

            {selectedFile && (
                <div className="start-chat-button-container">
                    <button
                        className="start-chat-button"
                        onClick={handleStartChat}
                    >
                        Start Chat
                    </button>
                </div>
            )}
        </div>
    );
};

export default ChatSelectionPage;
