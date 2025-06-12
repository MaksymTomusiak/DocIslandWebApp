import { useState, useCallback, useMemo } from 'react';
import { MessageApi } from '../services/api/messageApi';
import { MessageDto, MessageCreateDto } from '../types/types';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { useAuthToken } from './useAuthToken';

dayjs.extend(utc);

export const useMessages = (baseURL: string, conversationId: string) => {
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const messageApi = useMemo(
        () =>
            new MessageApi(
                baseURL,
                new AbortController().signal,
                () => Promise.resolve(token)
            ),
        [baseURL, token]
    );

    const loadMessages = useCallback(async () => {
        if (!token) return;

        try {
            setLoading(true);
            setError(null);
            const data = await messageApi.getMessagesByConversationId(
                conversationId
            );
            setMessages(data);
        } catch (err) {
            setError(
                err instanceof Error ? err.message : 'Failed to load messages'
            );
        } finally {
            setLoading(false);
        }
    }, [messageApi, conversationId, token]);

    const sendMessage = useCallback(
        async (content: string) => {
            try {
                setLoading(true);
                setError(null);
                const newMessage: MessageCreateDto = {
                    conversationId,
                    content,
                };

                const tempId = `temp-${Date.now()}`;
                const userMessage: MessageDto = {
                    id: tempId,
                    content: content,
                    isResponse: false,
                    createdAt: dayjs().utc(false).toDate(),
                    conversationId: conversationId,
                };
                setMessages((prev) => [...prev, userMessage]);

                const response = await messageApi.createMessage(newMessage);
                setMessages((prev) => [...prev, response]);
                return response;
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to send message'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [messageApi, conversationId]
    );

    const deleteMessage = useCallback(
        async (messageId: string) => {
            try {
                setLoading(true);
                setError(null);
                await messageApi.deleteMessage(messageId);
                setMessages((prev) =>
                    prev.filter((msg) => msg.id !== messageId)
                );
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to delete message'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [messageApi]
    );

    return {
        messages,
        loading: loading || isTokenLoading,
        error,
        loadMessages,
        sendMessage,
        deleteMessage,
    };
};
