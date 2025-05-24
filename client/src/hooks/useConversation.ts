import { useState, useCallback, useMemo } from 'react';
import { useAuth } from '@clerk/clerk-react';
import { ConversationApi } from '../services/conversationApi';
import { ConversationDto } from '../types/api';

export const useConversation = (baseURL: string) => {
    const { getToken } = useAuth();
    const getTokenWithTemplate = useCallback(
        () => getToken({ template: process.env.VITE_CLERK_JWT_TEMPLATE || '' }),
        [getToken]
    );
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const conversationApi = useMemo(
        () =>
            new ConversationApi(
                baseURL,
                new AbortController().signal,
                getTokenWithTemplate
            ),
        [baseURL, getTokenWithTemplate]
    );

    const loadConversations = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await conversationApi.getConversationsByUserId();
            setConversations(data);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Failed to load conversations'
            );
        } finally {
            setLoading(false);
        }
    }, [conversationApi]);

    const getConversationById = useCallback(
        async (id: string) => {
            try {
                setLoading(true);
                setError(null);
                const conversation = await conversationApi.getConversation(id);
                return conversation;
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to create conversation'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [conversationApi]
    );

    const createConversation = useCallback(
        async (file: File) => {
            try {
                setLoading(true);
                setError(null);
                const newConversation =
                    await conversationApi.createConversation({ file });
                setConversations((prev) => [newConversation, ...prev]);
                return newConversation;
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to create conversation'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [conversationApi]
    );

    const deleteConversation = useCallback(
        async (id: string) => {
            try {
                setLoading(true);
                setError(null);
                await conversationApi.deleteConversation(id);
                setConversations((prev) =>
                    prev.filter((conv) => conv.id !== id)
                );
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to delete conversation'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [conversationApi]
    );

    return {
        conversations,
        loading,
        error,
        loadConversations,
        createConversation,
        deleteConversation,
        getConversationById,
    };
};
