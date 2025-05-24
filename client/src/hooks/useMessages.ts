import { useState, useCallback, useMemo } from 'react';
import { useAuth } from '@clerk/clerk-react';
import { MessageApi } from '../services/messageApi';
import { MessageDto, MessageCreateDto } from '../types/api';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);

export const useMessages = (baseURL: string, conversationId: string) => {
    const { getToken } = useAuth();
    const getTokenWithTemplate = useCallback(
        () => getToken({ template: process.env.VITE_CLERK_JWT_TEMPLATE || '' }),
        [getToken]
    );
    const [messages, setMessages] = useState<MessageDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const messageApi = useMemo(
        () =>
            new MessageApi(
                baseURL,
                new AbortController().signal,
                getTokenWithTemplate
            ),
        [baseURL, getTokenWithTemplate]
    );

    const loadMessages = useCallback(async () => {
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
    }, [messageApi, conversationId]);

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
        loading,
        error,
        loadMessages,
        sendMessage,
        deleteMessage,
    };
};
