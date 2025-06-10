import { motion } from 'framer-motion';
import Hero from './components/Hero';
import About from './components/About';
import FAQ from './components/FAQ';
import Footer from './Footer/Footer';
import './home-page.css';

const HomePage = () => {
    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.5 }}
        >
            <div className="home-page-container">
                <Hero />
                <About />
            </div>
            <FAQ />
            <Footer />
        </motion.div>
    );
};

export default HomePage;
