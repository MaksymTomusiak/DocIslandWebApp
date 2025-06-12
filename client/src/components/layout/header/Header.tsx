import { useUser } from '@clerk/clerk-react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { CustomUserButton } from '../../auth/clerk/CustomClerkComponents';
import { useAdmin } from '../../../contexts/AdminContext';
import { Icon } from '@iconify/react';
import './header.css';

const Header = () => {
    const { isSignedIn, isLoaded: isClerkLoaded } = useUser();
    const navigate = useNavigate();
    const location = useLocation();
    const [showShadow, setShowShadow] = useState(false);
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const { isAdmin, isLoading: isAdminLoading } = useAdmin();

    useEffect(() => {
        const handleScroll = () => {
            if (location.pathname === '/select-chat') {
                setShowShadow(false);
                return;
            }

            if (location.pathname === '/admin/users') {
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

    useEffect(() => {
        if (isClerkLoaded) {
            // Add a small delay to ensure smooth transition
            const timer = setTimeout(() => {}, 300);
            return () => clearTimeout(timer);
        }
    }, [isClerkLoaded]);

    // Close mobile menu when route changes
    useEffect(() => {
        setIsMobileMenuOpen(false);
    }, [location.pathname]);

    const handleAuthClick = () => {
        if (!isSignedIn) {
            navigate('/login');
        }
    };

    const handleNavigation = (path: string) => {
        // Close mobile menu first
        setIsMobileMenuOpen(false);

        if (path === '/') {
            navigate('/');
            window.scrollTo({ top: 0, behavior: 'smooth' });
            return;
        }

        // If we're not on the home page, navigate to home page first
        if (location.pathname !== '/') {
            navigate('/');
            // Wait for navigation to complete before scrolling
            setTimeout(() => {
                const section = document.getElementById(path.substring(1));
                if (section) {
                    section.scrollIntoView({ behavior: 'smooth' });
                }
            }, 100);
        } else {
            // We're already on home page, just scroll
            const section = document.getElementById(path.substring(1));
            if (section) {
                section.scrollIntoView({ behavior: 'smooth' });
            }
        }
    };

    const renderMenuLinks = () => {
        // Wait for both Clerk and admin check to complete before showing content
        if (!isClerkLoaded || (isSignedIn && isAdminLoading)) {
            return <div className="header_menu" />;
        }

        const menuItems = (
            <>
                <p onClick={() => handleNavigation('/')}>Home</p>
                <p onClick={() => handleNavigation('#about')}>About</p>
                <p onClick={() => handleNavigation('#faq')}>Resources</p>
                {isSignedIn && isAdmin && (
                    <p onClick={() => navigate('/admin/users')}>Users</p>
                )}
            </>
        );

        return (
            <>
                <div className="header_menu">{menuItems}</div>
                <div
                    className={`mobile_menu ${isMobileMenuOpen ? 'open' : ''}`}
                    onClick={(e) => e.stopPropagation()}
                >
                    {menuItems}
                </div>
            </>
        );
    };

    const renderAuthSection = () => {
        if (!isClerkLoaded) {
            return (
                <div className="user_container">
                    <div className="user_menu" />
                </div>
            );
        }

        return (
            <div className="user_container">
                {isSignedIn ? (
                    <div className="user_menu">
                        <CustomUserButton />
                    </div>
                ) : (
                    <div className="login_button" onClick={handleAuthClick}>
                        <div>Login</div>
                        <img src="/header/arrowRight.svg" alt="Arrow" />
                    </div>
                )}
            </div>
        );
    };

    // Close mobile menu when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            const target = event.target as HTMLElement;
            if (
                isMobileMenuOpen &&
                !target.closest('.mobile_menu') &&
                !target.closest('.mobile_menu_button')
            ) {
                setIsMobileMenuOpen(false);
            }
        };

        document.addEventListener('click', handleClickOutside);
        return () => document.removeEventListener('click', handleClickOutside);
    }, [isMobileMenuOpen]);

    return (
        <header className={`header ${showShadow ? 'shadow' : ''}`}>
            <div className="logo_container">
                <div className="logo" onClick={() => handleNavigation('/')}>
                    {!isClerkLoaded ? (
                        <div style={{ width: 120, height: 40 }} />
                    ) : (
                        <img src="/header/logo.svg" alt="Logo" />
                    )}
                </div>
            </div>
            <button
                className="mobile_menu_button"
                onClick={(e) => {
                    e.stopPropagation();
                    setIsMobileMenuOpen(!isMobileMenuOpen);
                }}
                aria-label="Toggle mobile menu"
            >
                <Icon
                    icon={isMobileMenuOpen ? 'mdi:close' : 'mdi:menu'}
                    className="mobile_menu_icon"
                />
            </button>
            {renderMenuLinks()}
            {renderAuthSection()}
        </header>
    );
};

export default Header;
