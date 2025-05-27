import { useUser } from '@clerk/clerk-react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { CustomUserButton } from '../../auth/CustomClerkComponents';
import { useAdmin } from '../../../contexts/AdminContext';
import './header.css';

const Header = () => {
    const { isSignedIn, isLoaded: isClerkLoaded } = useUser();
    const navigate = useNavigate();
    const location = useLocation();
    const [showShadow, setShowShadow] = useState(false);
    const { isAdmin } = useAdmin();

    useEffect(() => {
        const handleScroll = () => {
            if (location.pathname === '/select-chat') {
                setShowShadow(false);
                return;
            }

            if (location.pathname !== '/') {
                setShowShadow(true);
                return;
            }
            // On home page, show shadow only when scrolled
            setShowShadow(window.scrollY > 0);
        };

        // Initial check
        handleScroll();

        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, [location.pathname]);

    const handleAuthClick = () => {
        if (!isSignedIn) {
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

    // Return a placeholder with the same height during loading
    if (!isClerkLoaded) {
        return <div style={{ height: '80px' }} />;
    }

    return (
        <header className={`header ${showShadow ? 'shadow' : ''}`}>
            <div className="logo" onClick={() => handleNavigation('#hero')}>
                <img src="/header/logo.svg" alt="Logo" />
            </div>
            <div className="header_menu">
                <p onClick={() => handleNavigation('#hero')}>Home</p>
                <p onClick={() => handleNavigation('#about')}>About</p>
                <p onClick={() => handleNavigation('#resources')}>Resources</p>
                {isSignedIn && isAdmin && (
                    <p onClick={() => navigate('/admin/users')}>Users</p>
                )}
            </div>
            {isSignedIn ? (
                <div className="user_menu">
                    <CustomUserButton />
                </div>
            ) : (
                <div className="login_button" onClick={handleAuthClick}>
                    <div>Login</div>
                    <img src="/header/arrowRightBlack.svg" alt="Arrow" />
                </div>
            )}
        </header>
    );
};

export default Header;
