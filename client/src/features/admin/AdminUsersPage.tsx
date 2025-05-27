import { useEffect } from 'react';
import { useUsers } from '../../hooks/useUsers';
import { useAdmin } from '../../contexts/AdminContext';
import { useAuthToken } from '../../hooks/useAuthToken';
import Spinner from '../../components/common/spinner/Spinner';
import './admin-users.css';

const AdminUsersPage = () => {
    const { users, loading, error, loadUsers, toggleAdmin, toggleBan } =
        useUsers(import.meta.env.VITE_API_BASE_URL || '');
    const { isAdmin } = useAdmin();
    const { token, isLoading: isTokenLoading } = useAuthToken();

    useEffect(() => {
        if (isAdmin && token && !isTokenLoading) {
            loadUsers();
        }
    }, [isAdmin, token, isTokenLoading]);

    if (isTokenLoading || loading) {
        return (
            <div className="admin-users-container">
                <Spinner />
            </div>
        );
    }

    if (error) {
        return (
            <div className="admin-users-container">
                <div className="error">{error}</div>
            </div>
        );
    }

    return (
        <div className="admin-users-container">
            <h1>User Management</h1>
            <div className="users-table">
                <table>
                    <thead>
                        <tr>
                            <th>Username</th>
                            <th>Email</th>
                            <th>Admin</th>
                            <th>Banned</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user) => (
                            <tr key={user.id}>
                                <td>{user.username}</td>
                                <td>{user.email}</td>
                                <td>
                                    <span
                                        className={`status ${
                                            user.isAdmin ? 'active' : 'inactive'
                                        }`}
                                    >
                                        {user.isAdmin ? 'Yes' : 'No'}
                                    </span>
                                </td>
                                <td>
                                    <span
                                        className={`status ${
                                            user.isBanned ? 'banned' : 'active'
                                        }`}
                                    >
                                        {user.isBanned ? 'Yes' : 'No'}
                                    </span>
                                </td>
                                <td>
                                    <div className="actions">
                                        <button
                                            onClick={() => toggleAdmin(user.id)}
                                            className={`action-button ${
                                                user.isAdmin
                                                    ? 'danger'
                                                    : 'primary'
                                            }`}
                                        >
                                            {user.isAdmin
                                                ? 'Remove Admin'
                                                : 'Make Admin'}
                                        </button>
                                        <button
                                            onClick={() => toggleBan(user.id)}
                                            className={`action-button ${
                                                user.isBanned
                                                    ? 'danger'
                                                    : 'warning'
                                            }`}
                                        >
                                            {user.isBanned
                                                ? 'Unban User'
                                                : 'Ban User'}
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default AdminUsersPage;
