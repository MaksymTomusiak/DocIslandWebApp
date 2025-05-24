import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Typography from '@mui/material/Typography';
import { Icon } from '@iconify/react';
import { motion } from 'framer-motion';

const FAQ = () => {
    const faqItems = [
        {
            question: 'What is Doc Island, and how does it work?',
            answer: "Doc Island is an AI-powered tool that helps you get instant answers from your documents. Simply upload your PDFs, reports, or other files, ask a question in natural language, and our advanced Large Language Model (LLM) will analyze the content to provide accurate, relevant responses in seconds. It's like having a personal research assistant at your fingertips!",
        },
        {
            question: 'What types of documents can I upload to Doc Island?',
            answer: "Doc Island supports a wide range of document types, including PDFs, Word documents (.docx), plain text (.txt), and more. Whether it's a research paper, business report, or study notes, you can upload most common file formats, and our AI will process them seamlessly.",
        },
        {
            question: 'Is my data secure with Doc Island?',
            answer: 'Absolutely! We prioritize your privacy and security. Doc Island uses industry-standard encryption to protect your uploaded documents and personal information. Your data is never shared with third parties, and you can delete your files from our system at any time.',
        },
        {
            question: 'Do I need any technical skills to use Doc Island?',
            answer: 'No technical skills are required! Doc Island is designed to be user-friendly for everyone. Our intuitive interface guides you through uploading documents and asking questions, making it easy for students, professionals, and anyone else to use without prior experience.',
        },
        {
            question: 'Can I use Doc Island on my mobile device?',
            answer: "Yes, Doc Island is fully responsive and works seamlessly across devices. Whether you're on a smartphone, tablet, or desktop, you can upload documents, ask questions, and get answers on the go. No app download is needed—just access it through your browser.",
        },
        {
            question: 'How accurate are the answers provided by Doc Island?',
            answer: "Doc Island's answers are highly accurate, thanks to our advanced Large Language Model (LLM). The AI is trained to understand context and extract precise information from your documents. For best results, ensure your documents are clear and well-structured, but our system is robust enough to handle a variety of content types.",
        },
        {
            question:
                'Is there a limit to the number of documents I can upload?',
            answer: 'The number of documents you can upload depends on your subscription plan. Free users have a limited quota for uploads, while premium plans offer higher limits and additional features. Visit our pricing page to explore the plan that best suits your needs.',
        },
        {
            question:
                'What should I do if I encounter an issue with Doc Island?',
            answer: "If you run into any issues, we're here to help! Check our FAQs for quick solutions, or reach out to our support team at support@docisland.com. We'll respond promptly to ensure you have a smooth experience with Doc Island.",
        },
    ];

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: {
                staggerChildren: 0.1,
            },
        },
    };

    const itemVariants = {
        hidden: { opacity: 0, y: 20 },
        visible: {
            opacity: 1,
            y: 0,
            transition: {
                duration: 0.5,
            },
        },
    };

    return (
        <motion.div
            id="faq"
            className="faq"
            initial="hidden"
            whileInView="visible"
            viewport={{ once: true }}
            variants={containerVariants}
        >
            <motion.div
                className="faq-header"
                initial={{ opacity: 0, y: -20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ duration: 0.6 }}
            >
                <motion.div
                    className="faq-header-title"
                    initial={{ scale: 0.9 }}
                    whileInView={{ scale: 1 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.4 }}
                >
                    Frequently Asked Questions
                </motion.div>
                <motion.div
                    className="faq-header-description"
                    initial={{ opacity: 0 }}
                    whileInView={{ opacity: 1 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.6, delay: 0.2 }}
                >
                    Find Answers to Common Questions
                </motion.div>
            </motion.div>
            <motion.div
                className="faq-accordeons-container"
                variants={containerVariants}
            >
                {faqItems.map((item, index) => (
                    <motion.div key={index} variants={itemVariants}>
                        <Accordion className="accordion">
                            <AccordionSummary
                                expandIcon={
                                    <motion.div
                                        whileHover={{ scale: 1.1 }}
                                        whileTap={{ scale: 0.9 }}
                                    >
                                        <Icon
                                            icon="bxs:chevron-down"
                                            width="24"
                                            height="24"
                                            style={{ color: '#006D77' }}
                                        />
                                    </motion.div>
                                }
                                aria-controls={`panel${index + 1}-content`}
                                id={`panel${index + 1}-header`}
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
                                    {item.question}
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
                                    {item.answer}
                                </Typography>
                            </AccordionDetails>
                        </Accordion>
                    </motion.div>
                ))}
            </motion.div>
        </motion.div>
    );
};

export default FAQ;
