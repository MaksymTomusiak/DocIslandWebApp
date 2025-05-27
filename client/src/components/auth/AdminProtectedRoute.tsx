import { useUser } from '@clerk/clerk-react';
import { Navigate } from 'react-router-dom';
import { useAdmin } from '../../contexts/AdminContext';

interface AdminProtectedRouteProps {
    children: React.ReactNode;
}

const AdminProtectedRoute = ({ children }: AdminProtectedRouteProps) => {
    const { user, isLoaded } = useUser();
    const { isAdmin } = useAdmin();

    if (!isLoaded) {
        return <div>Loading...</div>;
    }

    if (!user || !isAdmin) {
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
};

export default AdminProtectedRoute;
