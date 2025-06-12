import { useUser } from '@clerk/clerk-react';
import { useEffect } from 'react';
import { useBan } from '../../../contexts/BanContext';
import { useAuthToken } from '../../../hooks/useAuthToken';

const BanStatusChecker = () => {
    const { user, isLoaded } = useUser();
    const { setIsBanned } = useBan();
    const { token, isLoading: isTokenLoading } = useAuthToken();

    useEffect(() => {
        const checkBanStatus = async () => {
            if (!user || !token) {
                setIsBanned(false);
                return;
            }

            try {
                const response = await fetch(
                    `${import.meta.env.VITE_API_BASE_URL}/users/check-ban`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                if (response.ok) {
                    const data = await response.json();
                    setIsBanned(data.isBanned);
                } else {
                    console.error(
                        'Failed to check ban status:',
                        response.status
                    );
                    setIsBanned(false);
                }
            } catch (error) {
                console.error('Failed to check ban status:', error);
                setIsBanned(false);
            }
        };

        if (isLoaded && !isTokenLoading && token) {
            checkBanStatus();
        }
    }, [user, isLoaded, token, isTokenLoading, setIsBanned]);

    return null;
};

export default BanStatusChecker;
