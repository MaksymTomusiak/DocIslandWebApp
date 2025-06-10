import './spinner.css';

interface SpinnerProps {
    size?: 'small' | 'medium' | 'large';
}

const Spinner = ({ size = 'medium' }: SpinnerProps) => (
    <div className={`spinner-overlay spinner-${size}`}>
        <div className="spinner" />
    </div>
);

export default Spinner;
