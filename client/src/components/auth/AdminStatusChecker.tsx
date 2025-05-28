import { useUser } from '@clerk/clerk-react';
import { useEffect } from 'react';
import { useAdmin } from '../../contexts/AdminContext';
import { useAuthToken } from '../../hooks/useAuthToken';

const AdminStatusChecker = () => {
    const { user, isLoaded } = useUser();
    const { setIsAdmin, setIsLoading } = useAdmin();
    const { token, isLoading: isTokenLoading } = useAuthToken();

    useEffect(() => {
        const checkAdminStatus = async () => {
            if (!user || !token) {
                setIsAdmin(false);
                setIsLoading(false);
                return;
            }

            try {
                setIsLoading(true);
                const response = await fetch(
                    `${import.meta.env.VITE_API_BASE_URL}/users/check-admin`,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );

                if (response.ok) {
                    const data = await response.json();
                    setIsAdmin(data.isAdmin);
                } else {
                    setIsAdmin(false);
                }
            } catch (error) {
                console.error('Failed to check admin status:', error);
                setIsAdmin(false);
            } finally {
                setIsLoading(false);
            }
        };

        if (isLoaded && !isTokenLoading) {
            checkAdminStatus();
        }
    }, [user, isLoaded, token, isTokenLoading, setIsAdmin, setIsLoading]);

    return null;
};

export default AdminStatusChecker;
