import { useUser, useClerk } from '@clerk/clerk-react';
import { useNavigate, useLocation } from 'react-router-dom';
import './header.css';

const Header = () => {
    const { user, isSignedIn } = useUser();
    const { signOut } = useClerk();
    const navigate = useNavigate();
    const location = useLocation();

    const handleAuthClick = async () => {
        if (isSignedIn) {
            try {
                await signOut();
            } catch (error) {
                console.error('Error signing out:', error);
            }
        } else {
            navigate('/login');
        }
    };

    const handleNavigation = (path: string) => {
        if (path === '/') {
            navigate('/');
        } else {
            // If we're not on the home page, navigate to home page first
            if (location.pathname !== '/') {
                navigate('/');
            }
            // Then scroll to the appropriate section
            setTimeout(() => {
                const section = document.getElementById(path.substring(1));
                if (section) {
                    section.scrollIntoView({ behavior: 'smooth' });
                }
            }, 100);
        }
    };

    return (
        <div className="header">
            <div className="logo" onClick={() => handleNavigation('/')}>
                <img src="./header/logo.svg" alt="Logo" />
            </div>
            <div className="header_menu">
                <p onClick={() => handleNavigation('/')}>Home</p>
                <p onClick={() => handleNavigation('#about')}>About</p>
                <p onClick={() => handleNavigation('#resources')}>Resources</p>
            </div>
            {isSignedIn ? (
                <div className="user_menu" onClick={handleAuthClick}>
                    <img
                        src={user?.imageUrl}
                        alt="User avatar"
                        className="user_avatar"
                    />
                    <span>{user?.firstName || user?.username}</span>
                </div>
            ) : (
                <div className="login_button" onClick={handleAuthClick}>
                    <div>Login</div>
                    <img src="./header/arrowRightBlack.svg" alt="Arrow" />
                </div>
            )}
        </div>
    );
};

export default Header;
