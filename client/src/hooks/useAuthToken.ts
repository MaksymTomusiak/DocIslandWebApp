import { useAuth } from '@clerk/clerk-react';
import { useState, useEffect } from 'react';

export const useAuthToken = () => {
    const { getToken } = useAuth();
    const [token, setToken] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    useEffect(() => {
        const fetchToken = async () => {
            try {
                const newToken = await getToken({
                    template: import.meta.env.VITE_CLERK_JWT_TEMPLATE || '',
                });
                setToken(newToken);
                setError(null);
            } catch (err) {
                setError(err instanceof Error ? err : new Error('Failed to get token'));
            } finally {
                setIsLoading(false);
            }
        };

        fetchToken();
    }, [getToken]);

    return { token, isLoading, error };
}; 