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
            <Outlet />
        </div>
    );
};

export default Layout;
