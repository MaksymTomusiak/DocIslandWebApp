import { useUser, RedirectToSignIn } from '@clerk/clerk-react';
import { JSX } from '@emotion/react/jsx-runtime';
import { ReactNode, useEffect } from 'react';

interface ClerkProtectedRouteProps {
    children: ReactNode;
    allowedRoles: string[];
}

const ClerkProtectedRoute = ({
    children,
    allowedRoles,
}: ClerkProtectedRouteProps): JSX.Element => {
    const { user, isLoaded } = useUser();

    useEffect(() => {
        const setupUserData = async () => {
            if (user && !user.unsafeMetadata?.roles) {
                try {
                    await user.update({
                        unsafeMetadata: {
                            roles: ['user'],
                        },
                    });
                } catch (error) {
                    console.error('Error setting up user roles:', error);
                }
            }
        };

        setupUserData();
    }, [user]);
    //TODO: add loading state and isUserBanned
    if (!isLoaded) {
        return <div>Loading...</div>;
    }

    if (!user) {
        return <RedirectToSignIn />;
    }

    // For new users or users without roles, allow access
    if (!user.unsafeMetadata?.roles) {
        return <>{children}</>;
    }

    const userRoles = Array.isArray(user.unsafeMetadata?.roles)
        ? (user.unsafeMetadata.roles as string[])
        : [];

    const isAuthorized = allowedRoles.some((role) => userRoles.includes(role));

    if (!isAuthorized) {
        return <h1>Unauthorized</h1>;
    }

    return <>{children}</>;
};

export default ClerkProtectedRoute;
