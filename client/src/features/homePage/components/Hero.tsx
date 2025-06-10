import { useNavigate } from 'react-router-dom';
import { useUser } from '@clerk/clerk-react';
import { motion } from 'framer-motion';

const Hero = () => {
    const navigate = useNavigate();
    const { isSignedIn } = useUser();

    const handleStartNowClick = () => {
        if (isSignedIn) {
            navigate('/select-chat');
        } else {
            navigate('/login');
        }
    };

    const handleLearnMoreClick = () => {
        const aboutSection = document.getElementById('about');
        if (aboutSection) {
            aboutSection.scrollIntoView({ behavior: 'smooth' });
        }
    };

    return (
        <motion.div
            className="hero"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.6 }}
            style={{ position: 'relative' }}
        >
            <div className="hero-content">
                <motion.div
                    className="hero-title-and-text-container"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.6, delay: 0.2 }}
                >
                    <motion.div
                        className="hero-title"
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ duration: 0.4, delay: 0.4 }}
                    >
                        Doc Island
                    </motion.div>
                    <motion.div
                        className="hero-text"
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ duration: 0.6, delay: 0.6 }}
                    >
                        Upload your documents and get instant answers with Doc
                        Island's AI. From PDFs to reports, ask any question and
                        receive accurate, context-aware responses in seconds.
                        Start exploring your documents like never before!
                    </motion.div>
                </motion.div>
                <motion.div
                    className="hero-buttons-container"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.6, delay: 0.8 }}
                >
                    <motion.button
                        className="hero-button hero-button-main"
                        onClick={handleStartNowClick}
                        whileHover={{ scale: 1.05 }}
                        whileTap={{ scale: 0.95 }}
                    >
                        Start now
                    </motion.button>
                    <motion.button
                        className="hero-button hero-button-secondary"
                        onClick={handleLearnMoreClick}
                        whileHover={{ scale: 1.05 }}
                        whileTap={{ scale: 0.95 }}
                    >
                        Learn More
                    </motion.button>
                </motion.div>
            </div>
            <motion.div
                className="hero-image-container"
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: 0.6, delay: 0.4 }}
            >
                <img
                    className="hero-image"
                    src="/hero/HeroImage.png"
                    alt="Hero Image"
                />
            </motion.div>
        </motion.div>
    );
};

export default Hero;
