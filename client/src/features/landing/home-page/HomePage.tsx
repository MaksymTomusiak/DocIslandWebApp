import { motion } from 'framer-motion';
import Hero from '../Hero';
import About from '../About';
import FAQ from '../FAQ';
import Footer from '../Footer/Footer';
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
