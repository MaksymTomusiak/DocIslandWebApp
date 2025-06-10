import { useNavigate } from 'react-router-dom';
import './not-found.css';

export default function NotFoundPage() {
    const navigate = useNavigate();

    return (
        <div className="not-found-container">
            <div className="not-found-content">
                <h1 className="not-found-title">404</h1>
                <h2 className="not-found-subtitle">Page Not Found</h2>
                <p className="not-found-text">
                    Oops! The page you're looking for doesn't exist or has been
                    moved.
                </p>
                <button
                    className="not-found-button"
                    onClick={() => navigate('/')}
                >
                    Go Back
                </button>
            </div>
        </div>
    );
}
