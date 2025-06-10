import { Outlet } from 'react-router-dom';
import Header from './header/Header';

const Layout = () => {
    return (
        <div
            style={{
                display: 'flex',
                flexDirection: 'column',
                height: '100vh',
            }}
        >
            <Header />
            <main style={{ flex: 1, marginTop: '80px' }}>
                <Outlet />
            </main>
        </div>
    );
};

export default Layout;
