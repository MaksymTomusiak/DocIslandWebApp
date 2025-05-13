import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Typography from '@mui/material/Typography';
import { useNavigate } from 'react-router-dom';
import { useUser } from '@clerk/clerk-react';
import './home-page.css';
import { Icon } from '@iconify/react';

const HomePage = () => {
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
        <>
            <div className="home-page-container">
                <div className="hero">
                    <div className="hero-content">
                        <div className="hero-title-and-text-container">
                            <div className="hero-title">Doc Island</div>
                            <div className="hero-text">
                                Upload your documents and get instant answers
                                with Doc Island's AI. From PDFs to reports, ask
                                any question and receive accurate, context-aware
                                responses in seconds. Start exploring your
                                documents like never before!
                            </div>
                        </div>
                        <div className="hero-buttons-container">
                            <button
                                className="hero-button hero-button-main"
                                onClick={handleStartNowClick}
                            >
                                Start now
                            </button>
                            <button
                                className="hero-button hero-button-secondary"
                                onClick={handleLearnMoreClick}
                            >
                                Learn More
                            </button>
                        </div>
                    </div>
                    <div className="hero-image-container">
                        <img
                            className="hero-image"
                            src="./hero/HeroImage.png"
                            alt="Hero Image"
                        />
                    </div>
                </div>
                <div id="about" className="about">
                    <div className="about-header">
                        <div className="about-header-title">
                            Why Choose Doc Island?
                        </div>
                        <div className="about-header-description">
                            Discover Our Strengths
                        </div>
                    </div>
                    <div className="about-cards-container">
                        <div className="about-cards-row">
                            <div className="about-card">
                                <div className="about-card-header">
                                    <div className="about-card-header-icon">
                                        <Icon
                                            className="about-card-header-icon"
                                            icon="radix-icons:lightning-bolt"
                                        />
                                    </div>
                                    <div className="about-card-header-title">
                                        Instant Answers
                                    </div>
                                </div>
                                <div className="about-card-content">
                                    Get accurate answers from your documents in
                                    seconds with our advanced AI technology.
                                </div>
                            </div>
                            <div className="about-card">
                                <div className="about-card-header">
                                    <div className="about-card-header-icon">
                                        <Icon
                                            className="about-card-header-icon"
                                            icon="system-uicons:document-stack"
                                        />
                                    </div>
                                    <div className="about-card-header-title">
                                        Versatile Compatibility
                                    </div>
                                </div>
                                <div className="about-card-content">
                                    Upload PDFs, reports, and more — Doc Island
                                    is able to work with all your document
                                    formats .
                                </div>
                            </div>
                        </div>
                        <div className="about-cards-row">
                            <div className="about-card">
                                <div className="about-card-header">
                                    <div className="about-card-header-icon">
                                        <Icon
                                            className="about-card-header-icon"
                                            icon="lucide:smile"
                                        />
                                    </div>
                                    <div className="about-card-header-title">
                                        User-Friendly Interface
                                    </div>
                                </div>
                                <div className="about-card-content">
                                    Designed for everyone, from students to
                                    professionals, with a simple and intuitive
                                    interface.
                                </div>
                            </div>
                            <div className="about-card">
                                <div className="about-card-header">
                                    <Icon
                                        className="about-card-header-icon"
                                        icon="material-symbols:shield-outline"
                                    />
                                    <div className="about-card-header-title">
                                        Secure and Reliable
                                    </div>
                                </div>
                                <div className="about-card-content">
                                    Your documents are safe with us, thanks to
                                    top-tier security and privacy measures.
                                </div>
                            </div>
                        </div>
                    </div>
                    <div
                        className="about-image-container"
                        style={{
                            position: 'absolute',
                            top: '60px',
                        }}
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
                    </div>
                </div>
            </div>
            <div id="resources" className="resources">
                {/* Add your resources section content here */}
            </div>
            <div id="faq" className="faq">
                <div className="faq-header">
                    <div className="faq-header-title">
                        Frequently Asked Questions
                    </div>
                    <div className="faq-header-description">
                        Find Answers to Common Questions
                    </div>
                </div>
                <div className="faq-accordeons-container">
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel1-content"
                            id="panel1-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                What is Doc Island, and how does it work?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Doc Island is an AI-powered tool that helps you
                                get instant answers from your documents. Simply
                                upload your PDFs, reports, or other files, ask a
                                question in natural language, and our advanced
                                Large Language Model (LLM) will analyze the
                                content to provide accurate, relevant responses
                                in seconds. It's like having a personal research
                                assistant at your fingertips!
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel2-content"
                            id="panel2-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                What types of documents can I upload to Doc
                                Island?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Doc Island supports a wide range of document
                                types, including PDFs, Word documents (.docx),
                                plain text (.txt), and more. Whether it's a
                                research paper, business report, or study notes,
                                you can upload most common file formats, and our
                                AI will process them seamlessly.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel3-content"
                            id="panel3-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Is my data secure with Doc Island?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Absolutely! We prioritize your privacy and
                                security. Doc Island uses industry-standard
                                encryption to protect your uploaded documents
                                and personal information. Your data is never
                                shared with third parties, and you can delete
                                your files from our system at any time.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel4-content"
                            id="panel4-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Do I need any technical skills to use Doc
                                Island?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                No technical skills are required! Doc Island is
                                designed to be user-friendly for everyone. Our
                                intuitive interface guides you through uploading
                                documents and asking questions, making it easy
                                for students, professionals, and anyone else to
                                use without prior experience.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel5-content"
                            id="panel5-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Can I use Doc Island on my mobile device?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Yes, Doc Island is fully responsive and works
                                seamlessly across devices. Whether you're on a
                                smartphone, tablet, or desktop, you can upload
                                documents, ask questions, and get answers on the
                                go. No app download is needed—just access it
                                through your browser.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel6-content"
                            id="panel6-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                How accurate are the answers provided by Doc
                                Island?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Doc Island's answers are highly accurate, thanks
                                to our advanced Large Language Model (LLM). The
                                AI is trained to understand context and extract
                                precise information from your documents. For
                                best results, ensure your documents are clear
                                and well-structured, but our system is robust
                                enough to handle a variety of content types.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel7-content"
                            id="panel7-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                Is there a limit to the number of documents I
                                can upload?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                The number of documents you can upload depends
                                on your subscription plan. Free users have a
                                limited quota for uploads, while premium plans
                                offer higher limits and additional features.
                                Visit our pricing page to explore the plan that
                                best suits your needs.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                    <Accordion className="accordion">
                        <AccordionSummary
                            expandIcon={
                                <Icon
                                    icon="bxs:chevron-down"
                                    width="24"
                                    height="24"
                                    style={{ color: '#006D77' }}
                                />
                            }
                            aria-controls="panel8-content"
                            id="panel8-header"
                        >
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '18px',
                                    fontFamily: 'secondary-medium',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                What should I do if I encounter an issue with
                                Doc Island?
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Typography
                                component="span"
                                style={{
                                    fontSize: '16px',
                                    fontFamily: 'secondary-regular',
                                    lineHeight: '1.5',
                                    color: '#333',
                                }}
                            >
                                If you run into any issues, we're here to help!
                                Check our FAQs for quick solutions, or reach out
                                to our support team at support@docisland.com.
                                We'll respond promptly to ensure you have a
                                smooth experience with Doc Island.
                            </Typography>
                        </AccordionDetails>
                    </Accordion>
                </div>
            </div>
        </>
    );
};

export default HomePage;
