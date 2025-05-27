import { createContext, useContext, useState, ReactNode } from 'react';

interface BanContextType {
    isBanned: boolean;
    setIsBanned: (isBanned: boolean) => void;
}

const BanContext = createContext<BanContextType | undefined>(undefined);

interface BanProviderProps {
    children: ReactNode;
    isBanned: boolean;
    setIsBanned: (isBanned: boolean) => void;
}

export const BanProvider = ({
    children,
    isBanned,
    setIsBanned,
}: BanProviderProps) => {
    return (
        <BanContext.Provider value={{ isBanned, setIsBanned }}>
            {children}
        </BanContext.Provider>
    );
};

export const useBan = () => {
    const context = useContext(BanContext);
    if (context === undefined) {
        throw new Error('useBan must be used within a BanProvider');
    }
    return context;
};
