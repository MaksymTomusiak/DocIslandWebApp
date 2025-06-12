import { useUser } from '@clerk/clerk-react';
import { SignOutButton } from '@clerk/clerk-react';
import './banned-page.css';

const BannedPage = () => {
    const { user } = useUser();

    return (
        <div className="banned-page">
            <div className="banned-content">
                <h1>Account Banned</h1>
                <p>
                    Hello {user?.username || 'User'}, your account has been
                    banned from using DocIsland.
                </p>
                <p>
                    If you believe this is a mistake, please contact the
                    administrator.
                </p>
                <SignOutButton>
                    <button className="sign-out-button">Sign Out</button>
                </SignOutButton>
            </div>
        </div>
    );
};

export default BannedPage;
