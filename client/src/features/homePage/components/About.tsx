import { Icon } from '@iconify/react';
import { motion } from 'framer-motion';

const About = () => {
    const cardVariants = {
        hidden: { opacity: 0, y: 20 },
        visible: (i: number) => ({
            opacity: 1,
            y: 0,
            transition: {
                delay: 0.2 * i,
                duration: 0.5,
            },
        }),
    };

    return (
        <motion.div
            id="about"
            className="about"
            initial={{ opacity: 0 }}
            whileInView={{ opacity: 1 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6 }}
        >
            <motion.div
                className="about-header"
                initial={{ opacity: 0, y: -20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ duration: 0.6 }}
            >
                <motion.div
                    className="about-header-title"
                    initial={{ scale: 0.9 }}
                    whileInView={{ scale: 1 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.4 }}
                >
                    Why Choose Doc Island?
                </motion.div>
                <motion.div
                    className="about-header-description"
                    initial={{ opacity: 0 }}
                    whileInView={{ opacity: 1 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.6, delay: 0.2 }}
                >
                    Discover Our Strengths
                </motion.div>
            </motion.div>
            <div className="about-cards-container">
                <div className="about-cards-row">
                    {[
                        {
                            icon: 'radix-icons:lightning-bolt',
                            title: 'Instant Answers',
                            content:
                                'Get accurate answers from your documents in seconds with our advanced AI technology.',
                        },
                        {
                            icon: 'system-uicons:document-stack',
                            title: 'Versatile Compatibility',
                            content:
                                'Upload PDFs, reports, and more — Doc Island is able to work with all your document formats.',
                        },
                    ].map((card, index) => (
                        <motion.div
                            key={index}
                            className="about-card"
                            custom={index}
                            variants={cardVariants}
                            initial="hidden"
                            whileInView="visible"
                            viewport={{ once: true }}
                        >
                            <div className="about-card-header">
                                <motion.div
                                    className="about-card-header-icon"
                                    whileHover={{ scale: 1.1 }}
                                    transition={{
                                        type: 'spring',
                                        stiffness: 400,
                                        damping: 10,
                                    }}
                                >
                                    <Icon
                                        className="about-card-header-icon"
                                        icon={card.icon}
                                    />
                                </motion.div>
                                <div className="about-card-header-title">
                                    {card.title}
                                </div>
                            </div>
                            <div className="about-card-content">
                                {card.content}
                            </div>
                        </motion.div>
                    ))}
                </div>
                <div className="about-cards-row">
                    {[
                        {
                            icon: 'lucide:smile',
                            title: 'User-Friendly Interface',
                            content:
                                'Designed for everyone, from students to professionals, with a simple and intuitive interface.',
                        },
                        {
                            icon: 'material-symbols:shield-outline',
                            title: 'Secure and Reliable',
                            content:
                                'Your documents are safe with us, thanks to top-tier security and privacy measures.',
                        },
                    ].map((card, index) => (
                        <motion.div
                            key={index}
                            className="about-card"
                            custom={index + 2}
                            variants={cardVariants}
                            initial="hidden"
                            whileInView="visible"
                            viewport={{ once: true }}
                        >
                            <div className="about-card-header">
                                <motion.div
                                    className="about-card-header-icon"
                                    whileHover={{ scale: 1.1 }}
                                    transition={{
                                        type: 'spring',
                                        stiffness: 400,
                                        damping: 10,
                                    }}
                                >
                                    <Icon
                                        className="about-card-header-icon"
                                        icon={card.icon}
                                    />
                                </motion.div>
                                <div className="about-card-header-title">
                                    {card.title}
                                </div>
                            </div>
                            <div className="about-card-content">
                                {card.content}
                            </div>
                        </motion.div>
                    ))}
                </div>
            </div>
            <motion.div
                className="about-image-container"
                style={{
                    position: 'absolute',
                    top: '60px',
                }}
                initial={{ opacity: 0, scale: 0.9 }}
                whileInView={{ opacity: 1, scale: 1 }}
                viewport={{ once: true }}
                transition={{ duration: 0.8 }}
            >
                <img
                    style={{
                        height: '100%',
                        width: '100%',
                    }}
                    className="about-image"
                    src="./home-page/about/IslandBorder.svg"
                    alt="IslandBorder"
                />
            </motion.div>
        </motion.div>
    );
};

export default About;
