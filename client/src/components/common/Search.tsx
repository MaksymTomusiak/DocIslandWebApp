import { ChangeEvent, useState, useEffect } from 'react';

interface SearchProps {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
    className?: string;
}

export function Search({
    value,
    onChange,
    placeholder = 'Search...',
    className = '',
}: SearchProps) {
    const [inputValue, setInputValue] = useState(value);

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (inputValue !== value) {
                onChange(inputValue);
            }
        }, 150);

        return () => clearTimeout(timeoutId);
    }, [inputValue, onChange, value]);

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInputValue(e.target.value);
    };

    return (
        <input
            type="text"
            value={inputValue}
            onChange={handleChange}
            placeholder={placeholder}
            className={`search-input ${className}`}
        />
    );
}
