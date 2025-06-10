import { Icon } from '@iconify/react/dist/iconify.js';
import { useNavigate, useLocation } from 'react-router-dom';
import './footer.css';

const Footer = () => {
    const navigate = useNavigate();
    const location = useLocation();

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
        <div className="footer">
            <div className="content">
                <div className="text-group">
                    <div
                        className="text-group-title"
                        style={{ justifyContent: 'left' }}
                        onClick={() => handleNavigation('/')}
                    >
                        <img src="./header/logo.svg" alt="logo" />
                        Doc Island
                    </div>
                    <div className="text-group-content">
                        Doc Island is your go-to AI tool for unlocking insights
                        from documents. Upload your PDFs, ask questions, and get
                        instant, accurate answers with ease.
                    </div>
                </div>

                <div className="text-group">
                    <div className="text-group-title">Explore</div>
                    <div className="text-group-content">
                        Discover how Doc Island can help you. Learn about our
                        features, pricing plans, and the benefits of using AI to
                        manage your documents efficiently.
                    </div>
                    <div
                        className="text-group-link"
                        onClick={() => handleNavigation('#about')}
                    >
                        Learn More About Features
                    </div>
                </div>

                <div className="text-group">
                    <div className="text-group-title">Support</div>
                    <div className="text-group-content">
                        Need help with Doc Island? Access our FAQs, read our
                        terms of service, or reach out to our support team for
                        assistance with your queries.
                    </div>
                    <div
                        className="text-group-link"
                        onClick={() => handleNavigation('#faq')}
                    >
                        Visit Our FAQs
                    </div>
                </div>

                <div className="text-group">
                    <div className="text-group-title">Connect</div>
                    <div className="text-group-content">
                        Stay connected with Doc Island. Reach out via email or
                        follow us on social media for updates, tips, and more.
                    </div>
                    <div className="text-group-link">
                        <a href="mailto:support@docisland.com">
                            Email: support@docisland.com
                        </a>
                    </div>
                    <div className="social-media-icons">
                        <a href="https://www.instagram.com/docisland">
                            <Icon
                                className="social-media-icon"
                                icon="mdi:instagram"
                            />
                        </a>
                        <a href="https://www.twitter.com/docisland">
                            <Icon
                                className="social-media-icon"
                                icon="prime:twitter"
                            />
                        </a>
                        <a href="https://www.facebook.com/docisland">
                            <Icon
                                className="social-media-icon"
                                icon="ic:baseline-facebook"
                            />
                        </a>
                    </div>
                </div>
            </div>
            <div className="copyright">
                © 2025 Doc Island. All rights reserved.
            </div>
        </div>
    );
};

export default Footer;
