import Skeleton from '../../components/common/skeleton/Skeleton';

const ChatMessagesSkeleton = () => {
    return (
        <div className="chat-messages">
            {[...Array(3)].map((_, index) => (
                <div
                    key={index}
                    className={`message ${
                        index % 2 === 0 ? 'user-message' : 'ai-message'
                    }`}
                >
                    <div className="message-content">
                        <div className="message-header">
                            <Skeleton variant="text" width={80} height={16} />
                            <Skeleton variant="text" width={60} height={16} />
                        </div>
                        <div style={{ marginTop: '12px' }}>
                            <Skeleton variant="text" width="90%" height={16} />
                            <Skeleton variant="text" width="75%" height={16} />
                            {index % 2 !== 0 && (
                                <Skeleton
                                    variant="text"
                                    width="60%"
                                    height={16}
                                />
                            )}
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default ChatMessagesSkeleton;
