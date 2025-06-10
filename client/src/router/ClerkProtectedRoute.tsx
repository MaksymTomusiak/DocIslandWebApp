import {
    SignedIn,
    SignedOut,
    RedirectToSignIn,
    useUser,
} from '@clerk/clerk-react';
import { ReactNode } from 'react';
import Spinner from '../components/common/spinner/Spinner';

interface ClerkProtectedRouteProps {
    children: ReactNode;
    allowedRoles?: string[];
}

const ClerkProtectedRoute = ({
    children,
    allowedRoles,
}: ClerkProtectedRouteProps) => {
    const { user, isLoaded } = useUser();

    if (!isLoaded) return <Spinner />;

    if (!user)
        return (
            <>
                <SignedOut>
                    <RedirectToSignIn />
                </SignedOut>
            </>
        );

    // If allowedRoles is provided, check user roles
    if (allowedRoles && allowedRoles.length > 0) {
        const userRoles = Array.isArray(user.unsafeMetadata?.roles)
            ? (user.unsafeMetadata.roles as string[])
            : [];
        const isAuthorized = allowedRoles.some((role) =>
            userRoles.includes(role)
        );
        if (!isAuthorized) {
            return <h1>Unauthorized</h1>;
        }
    }

    return <SignedIn>{children}</SignedIn>;
};

export default ClerkProtectedRoute;
