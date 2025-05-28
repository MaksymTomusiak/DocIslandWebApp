import Skeleton from '../../../components/common/skeleton/Skeleton';

const ConversationsSkeleton = () => {
    return (
        <div className="conversations-list" style={{ opacity: 0.7 }}>
            {[...Array(5)].map((_, index) => (
                <div
                    key={index}
                    className="conversation-item"
                    style={{ pointerEvents: 'none' }}
                >
                    <div
                        className="conversation-info"
                        style={{ width: '100%' }}
                    >
                        <div className="skeleton-wrapper">
                            <Skeleton variant="text" width="70%" height={20} />
                            <Skeleton variant="text" width="40%" height={16} />
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default ConversationsSkeleton;
