import './ai-chat.css';

const AiChatPage = () => {
    return (
        <div className="ai-chat-container">
            <div className="chat-header">
                <h1>AI Chat</h1>
            </div>
            <div className="chat-messages">
                {/* Messages will be added here */}
            </div>
            <div className="chat-input">
                <textarea
                    placeholder="Type your message here..."
                    className="message-input"
                />
                <button className="send-button">Send</button>
            </div>
        </div>
    );
};

export default AiChatPage;
