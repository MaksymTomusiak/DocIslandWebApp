import { useState, useCallback, memo } from 'react';
import { Icon } from '@iconify/react';
import { ConversationDto } from '../../../types/types';
import ConversationsSkeleton from './ConversationsSkeleton';
import './conversations-sidebar.css';

interface ConversationsSidebarProps {
    conversations: ConversationDto[];
    onSelectConversation: (id: string) => void;
    onDeleteConversation: (id: string) => void;
    currentConversationId: string | undefined;
    onNewChat: () => void;
    loading?: boolean;
    isExpanded: boolean;
    onToggle: () => void;
}

const ConversationsSidebar = memo(
    ({
        conversations,
        onSelectConversation,
        onDeleteConversation,
        currentConversationId,
        onNewChat,
        loading = false,
        isExpanded,
        onToggle,
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
            <>
                <button
                    className={`sidebar-toggle ${isExpanded ? 'expanded' : ''}`}
                    onClick={onToggle}
                    aria-label="Toggle conversations sidebar"
                >
                    <Icon
                        icon={
                            isExpanded
                                ? 'material-symbols:chevron-left'
                                : 'material-symbols:chevron-right'
                        }
                        width="24"
                        height="24"
                    />
                </button>
                <div
                    className={`conversations-sidebar ${
                        isExpanded ? 'expanded' : ''
                    }`}
                >
                    {loading ? (
                        <>
                            <div className="sidebar-header skeleton-header">
                                <div className="skeleton-title">
                                    <h2>Conversations</h2>
                                </div>
                                <div className="skeleton-button">
                                    <button
                                        className="new-chat-button"
                                        disabled
                                    >
                                        <Icon icon="material-symbols:add" />
                                    </button>
                                </div>
                            </div>
                            <ConversationsSkeleton />
                        </>
                    ) : (
                        <>
                            <div className="sidebar-header">
                                <h2>Conversations</h2>
                                <button
                                    className="new-chat-button"
                                    onClick={onNewChat}
                                    aria-label="New Chat"
                                >
                                    <Icon icon="material-symbols:add" />
                                </button>
                            </div>
                            <div className="conversations-list">
                                {conversations.map(renderConversationItem)}
                            </div>
                        </>
                    )}

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
            </>
        );
    }
);

ConversationsSidebar.displayName = 'ConversationsSidebar';

export default ConversationsSidebar;
