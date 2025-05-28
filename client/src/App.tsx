import Router from './router/Router';
import './features/ai-chat/ai-chat.css';
import './features/ai-chat/conversations-sidebar/conversations-sidebar.css';
import './features/chat-selection/chat-selection.css';
import './components/common/spinner/spinner.css';
import './components/common/not-found/not-found.css';
import './components/layout/header/header.css';
import './features/homePage/home-page.css';
import './components/auth/custom-clerk-components.css';
import { useState } from 'react';
import { AdminProvider } from './contexts/AdminContext';
import { BanProvider } from './contexts/BanContext';

function App() {
    const [isAdmin, setIsAdmin] = useState(false);
    const [isAdminLoading, setIsAdminLoading] = useState(true);
    const [isBanned, setIsBanned] = useState(false);

    return (
        <AdminProvider
            isAdmin={isAdmin}
            setIsAdmin={setIsAdmin}
            isLoading={isAdminLoading}
            setIsLoading={setIsAdminLoading}
        >
            <BanProvider isBanned={isBanned} setIsBanned={setIsBanned}>
                <Router />
            </BanProvider>
        </AdminProvider>
    );
}

export default App;
