import Router from './router/Router';
import './App.css';
import './features/ai-chat/ai-chat/ai-chat.css';
import './features/ai-chat/conversations-sidebar/conversations-sidebar.css';
import './features/chat-selection/chat-selection.css';
import './components/common/spinner.css';
import './components/common/not-found.css';
import './components/layout/header/header.css';
import './features/homePage/home-page.css';
import './features/auth/sign-in/sign-in.css';
import './components/auth/custom-clerk-components.css';

function App() {
    return (
        <>
            <Router />
        </>
    );
}

export default App;
