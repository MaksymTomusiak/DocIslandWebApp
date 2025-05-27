import { useState, useCallback, useMemo } from 'react';
import { UserApi } from '../services/userApi';
import { UserDto } from '../types/api';
import { useAuthToken } from './useAuthToken';

export const useUsers = (baseURL: string) => {
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [users, setUsers] = useState<UserDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const userApi = useMemo(
        () =>
            new UserApi(
                baseURL,
                new AbortController().signal,
                () => Promise.resolve(token)
            ),
        [baseURL, token]
    );

    const loadUsers = useCallback(async () => {
        if (!token) return;

        try {
            setLoading(true);
            setError(null);
            const data = await userApi.getAllUsers();
            setUsers(data);
        } catch (err) {
            setError(
                err instanceof Error ? err.message : 'Failed to load users'
            );
        } finally {
            setLoading(false);
        }
    }, [userApi, token]);

    const toggleAdmin = useCallback(
        async (userId: string) => {
            try {
                setLoading(true);
                setError(null);
                const newAdminStatus = await userApi.toggleAdmin(userId);
                setUsers((prev) =>
                    prev.map((user) =>
                        user.id === userId
                            ? { ...user, isAdmin: newAdminStatus }
                            : user
                    )
                );
                return newAdminStatus;
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to toggle admin status'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [userApi]
    );

    const toggleBan = useCallback(
        async (userId: string) => {
            try {
                setLoading(true);
                setError(null);
                const newBanStatus = await userApi.toggleBan(userId);
                setUsers((prev) =>
                    prev.map((user) =>
                        user.id === userId
                            ? { ...user, isBanned: newBanStatus }
                            : user
                    )
                );
                return newBanStatus;
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Failed to toggle ban status'
                );
                throw err;
            } finally {
                setLoading(false);
            }
        },
        [userApi]
    );

    return {
        users,
        loading: loading || isTokenLoading,
        error,
        loadUsers,
        toggleAdmin,
        toggleBan,
    };
}; 