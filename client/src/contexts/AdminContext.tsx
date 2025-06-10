import { createContext, useContext, ReactNode } from 'react';

interface AdminContextType {
    isAdmin: boolean;
    setIsAdmin: (isAdmin: boolean) => void;
    isLoading: boolean;
    setIsLoading: (isLoading: boolean) => void;
}

const AdminContext = createContext<AdminContextType | undefined>(undefined);

export const AdminProvider = ({
    children,
    isAdmin,
    setIsAdmin,
    isLoading,
    setIsLoading,
}: AdminContextType & { children: ReactNode }) => {
    return (
        <AdminContext.Provider
            value={{ isAdmin, setIsAdmin, isLoading, setIsLoading }}
        >
            {children}
        </AdminContext.Provider>
    );
};

export const useAdmin = () => {
    const context = useContext(AdminContext);
    if (context === undefined) {
        throw new Error('useAdmin must be used within an AdminProvider');
    }
    return context;
};
