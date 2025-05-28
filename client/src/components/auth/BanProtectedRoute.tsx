import { useUser } from '@clerk/clerk-react';
import { Navigate } from 'react-router-dom';
import { useBan } from '../../contexts/BanContext';
import Spinner from '../common/spinner/Spinner';
import BannedPage from '../../pages/BannedPage';

interface BanProtectedRouteProps {
    children: React.ReactNode;
}

const BanProtectedRoute = ({ children }: BanProtectedRouteProps) => {
    const { user, isLoaded } = useUser();
    const { isBanned } = useBan();
    const isBannedRoute = window.location.pathname === '/banned';

    if (!isLoaded) {
        return <Spinner />;
    }

    if (!user) {
        return <Navigate to="/sign-in" replace />;
    }

    // If we're on the banned page
    if (isBannedRoute) {
        return isBanned ? <BannedPage /> : <Navigate to="/" replace />;
    }

    // If we're on any other page
    if (isBanned) {
        return <Navigate to="/banned" replace />;
    }

    return <>{children}</>;
};

export default BanProtectedRoute;
