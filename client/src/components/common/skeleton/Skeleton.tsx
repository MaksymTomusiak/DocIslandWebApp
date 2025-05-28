import './skeleton.css';

interface SkeletonProps {
    variant?: 'text' | 'rectangular' | 'circular';
    width?: string | number;
    height?: string | number;
    className?: string;
}

const Skeleton = ({
    variant = 'text',
    width,
    height,
    className = '',
}: SkeletonProps) => {
    const style = {
        width: width
            ? typeof width === 'number'
                ? `${width}px`
                : width
            : undefined,
        height: height
            ? typeof height === 'number'
                ? `${height}px`
                : height
            : undefined,
    };

    return (
        <div
            className={`skeleton skeleton-${variant} ${className}`}
            style={style}
        />
    );
};

export default Skeleton;
