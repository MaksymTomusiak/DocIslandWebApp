import { useState, useCallback, useMemo } from 'react';
import { ConversationApi } from '../services/api/conversationApi';
import { ConversationDto } from '../types/types';
import { useAuthToken } from './useAuthToken';

export const useConversation = (baseURL: string) => {
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
    const [isLoadingConversations, setIsLoadingConversations] = useState(false);
    const [isCreatingChat, setIsCreatingChat] = useState(false);
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

    const loadRecentConversations = useCallback(async () => {
        if (!token) return;

        try {
            setIsLoadingConversations(true);
            setError(null);
            await new Promise(resolve => setTimeout(resolve, 700));
            const data = await conversationApi.getRecentConversationsByUserId()
            setConversations(data);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Failed to load conversations'
            );
        } finally {
            setIsLoadingConversations(false);
        }
    }, [conversationApi, token])

    const loadConversations = useCallback(async () => {
        if (!token) return;

        try {
            setIsLoadingConversations(true);
            setError(null);
            await new Promise(resolve => setTimeout(resolve, 700));
            const data = await conversationApi.getConversationsByUserId();
            setConversations(data);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Failed to load conversations'
            );
        } finally {
            setIsLoadingConversations(false);
        }
    }, [conversationApi, token]);

    const getConversationById = useCallback(
        async (id: string) => {
            try {
                setIsLoadingConversations(true);
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
                setIsLoadingConversations(false);
            }
        },
        [conversationApi]
    );

    const createConversation = useCallback(
        async (file: File) => {
            try {
                setIsCreatingChat(true);
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
                setIsCreatingChat(false);
            }
        },
        [conversationApi]
    );

    const deleteConversation = useCallback(
        async (id: string) => {
            try {
                setIsLoadingConversations(true);
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
                setIsLoadingConversations(false);
            }
        },
        [conversationApi]
    );

    return {
        conversations,
        loading: isLoadingConversations || isTokenLoading,
        isCreatingChat,
        error,
        loadRecentConversations,
        loadConversations,
        createConversation,
        deleteConversation,
        getConversationById,
    };
};
