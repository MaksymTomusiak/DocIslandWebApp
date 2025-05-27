import { useUser } from '@clerk/clerk-react';
import { Navigate } from 'react-router-dom';
import { useBan } from '../../contexts/BanContext';
import Spinner from '../common/spinner/Spinner';

interface BanProtectedRouteProps {
    children: React.ReactNode;
}

const BanProtectedRoute = ({ children }: BanProtectedRouteProps) => {
    const { user, isLoaded } = useUser();
    const { isBanned } = useBan();

    if (!isLoaded) {
        return <Spinner />;
    }

    if (!user) {
        return <Navigate to="/sign-in" replace />;
    }

    if (isBanned) {
        return <Navigate to="/banned" replace />;
    }

    return <>{children}</>;
};

export default BanProtectedRoute;
