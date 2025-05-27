import { useState, useCallback, useMemo } from 'react';
import { ConversationApi } from '../services/conversationApi';
import { ConversationDto } from '../types/api';
import { useAuthToken } from './useAuthToken';

export const useConversation = (baseURL: string) => {
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const conversationApi = useMemo(
        () =>
            new ConversationApi(
                baseURL,
                new AbortController().signal,
                () => Promise.resolve(token)
            ),
        [baseURL, token]
    );

    const loadConversations = useCallback(async () => {
        if (!token) return;

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
    }, [conversationApi, token]);

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
        loading: loading || isTokenLoading,
        error,
        loadConversations,
        createConversation,
        deleteConversation,
        getConversationById,
    };
};
