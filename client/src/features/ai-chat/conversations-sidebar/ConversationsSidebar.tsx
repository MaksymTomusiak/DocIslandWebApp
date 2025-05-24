import { useState, useCallback, memo } from 'react';
import { Icon } from '@iconify/react';
import { ConversationDto } from '../../../types/api';
import './conversations-sidebar.css';

interface ConversationsSidebarProps {
    conversations: ConversationDto[];
    onSelectConversation: (id: string) => void;
    onDeleteConversation: (id: string) => void;
    currentConversationId: string | undefined;
}

const ConversationsSidebar = memo(
    ({
        conversations,
        onSelectConversation,
        onDeleteConversation,
        currentConversationId,
    }: ConversationsSidebarProps) => {
        const [conversationToDelete, setConversationToDelete] = useState<
            string | null
        >(null);

        const handleDeleteClick = useCallback(
            (e: React.MouseEvent, conversationId: string) => {
                e.stopPropagation();
                setConversationToDelete(conversationId);
            },
            []
        );

        const handleConfirmDelete = useCallback(() => {
            if (conversationToDelete) {
                onDeleteConversation(conversationToDelete);
                setConversationToDelete(null);
            }
        }, [conversationToDelete, onDeleteConversation]);

        const handleCancelDelete = useCallback(() => {
            setConversationToDelete(null);
        }, []);

        const renderConversationItem = useCallback(
            (conversation: ConversationDto) => (
                <div
                    key={conversation.id}
                    className={`conversation-item ${
                        conversation.id === currentConversationId
                            ? 'active'
                            : ''
                    }`}
                    onClick={() => onSelectConversation(conversation.id)}
                >
                    <div className="conversation-info">
                        <span className="conversation-title">
                            {conversation.fileName}
                        </span>
                        <span className="conversation-date">
                            {new Date(
                                conversation.createdAt
                            ).toLocaleDateString()}
                        </span>
                    </div>
                    <button
                        className="delete-button"
                        onClick={(e) => handleDeleteClick(e, conversation.id)}
                    >
                        <Icon icon="material-symbols:delete-outline" />
                    </button>
                </div>
            ),
            [currentConversationId, handleDeleteClick, onSelectConversation]
        );

        return (
            <div className="conversations-sidebar">
                <div className="sidebar-header">
                    <h2>Conversations</h2>
                </div>
                <div className="conversations-list">
                    {conversations.map(renderConversationItem)}
                </div>

                {conversationToDelete && (
                    <div className="modal-overlay">
                        <div className="modal-content">
                            <h3>Delete Conversation</h3>
                            <p>
                                Are you sure you want to delete this
                                conversation? This action cannot be undone.
                            </p>
                            <div className="modal-buttons">
                                <button
                                    className="cancel-button"
                                    onClick={handleCancelDelete}
                                >
                                    Cancel
                                </button>
                                <button
                                    className="delete-confirm-button"
                                    onClick={handleConfirmDelete}
                                >
                                    Delete
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        );
    }
);

ConversationsSidebar.displayName = 'ConversationsSidebar';

export default ConversationsSidebar;
