import { useEffect, useState } from 'react';
import { useUsers } from '../../hooks/useUsers';
import { useAdmin } from '../../contexts/AdminContext';
import { useAuthToken } from '../../hooks/useAuthToken';
import AdminUsersSkeleton from './AdminUsersSkeleton';
import { PaginationParameters } from '../../types/types';
import './admin-users.css';

const AdminUsersPage = () => {
    const {
        users,
        loading,
        error,
        fetchPaginatedUsers,
        toggleAdmin,
        toggleBan,
    } = useUsers(import.meta.env.VITE_API_BASE_URL || '');
    const { isAdmin } = useAdmin();
    const { token, isLoading: isTokenLoading } = useAuthToken();
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');
    const [sortBy, setSortBy] = useState<string>('username');
    const [sortDescending, setSortDescending] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    // Debounce search term
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearchTerm(searchTerm);
        }, 300);

        return () => clearTimeout(timer);
    }, [searchTerm]);

    // Fetch users when parameters change
    useEffect(() => {
        if (isAdmin && token && !isTokenLoading) {
            const params: PaginationParameters = {
                pageNumber: currentPage,
                pageSize,
                searchTerm: debouncedSearchTerm || undefined,
                sortBy: sortBy || undefined,
                sortDescending,
            };
            fetchPaginatedUsers(params);
        }
    }, [
        isAdmin,
        token,
        isTokenLoading,
        currentPage,
        debouncedSearchTerm,
        sortBy,
        sortDescending,
        fetchPaginatedUsers,
    ]);

    // Only show skeleton on initial load
    if (isTokenLoading || (loading && !users)) {
        return (
            <div className="admin-users-container">
                <h1>User Management</h1>
                <AdminUsersSkeleton />
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

    if (!users) {
        return null;
    }

    return (
        <div className="admin-users-container">
            <h1>User Management</h1>
            <div className="users-table">
                <div className="table-controls">
                    <input
                        type="text"
                        placeholder="Search users..."
                        value={searchTerm}
                        onChange={(e) => {
                            setSearchTerm(e.target.value);
                            setCurrentPage(1);
                        }}
                        className="search-input"
                    />
                </div>
                <table>
                    <thead>
                        <tr>
                            <th
                                onClick={() => {
                                    setSortBy('username');
                                    setSortDescending(
                                        sortBy === 'username'
                                            ? !sortDescending
                                            : false
                                    );
                                    setCurrentPage(1);
                                }}
                            >
                                Username{' '}
                                {sortBy === 'username' &&
                                    (sortDescending ? '↓' : '↑')}
                            </th>
                            <th
                                onClick={() => {
                                    setSortBy('email');
                                    setSortDescending(
                                        sortBy === 'email'
                                            ? !sortDescending
                                            : false
                                    );
                                    setCurrentPage(1);
                                }}
                            >
                                Email{' '}
                                {sortBy === 'email' &&
                                    (sortDescending ? '↓' : '↑')}
                            </th>
                            <th>Admin</th>
                            <th>Banned</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.items.map((user) => (
                            <tr key={user.id}>
                                <td>{user.userName}</td>
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
                                                    ? 'secondary'
                                                    : 'primary'
                                            }`}
                                            disabled={loading}
                                        >
                                            {user.isAdmin
                                                ? 'Remove Admin'
                                                : 'Make Admin'}
                                        </button>
                                        <button
                                            onClick={() => toggleBan(user.id)}
                                            className={`action-button ${
                                                user.isBanned
                                                    ? 'secondary'
                                                    : 'primary'
                                            }`}
                                            disabled={loading}
                                        >
                                            {user.isBanned ? 'Unban' : 'Ban'}
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                <div className="table-footer">
                    <div className="pagination-info">
                        Showing {users.items.length} of {users.totalCount} users
                    </div>
                    <div className="pagination-controls">
                        <button
                            onClick={() =>
                                setCurrentPage((prev) => Math.max(1, prev - 1))
                            }
                            disabled={currentPage === 1 || loading}
                            className="pagination-button"
                        >
                            Previous
                        </button>
                        <span className="page-info">
                            Page {currentPage} of {users.totalPages}
                        </span>
                        <button
                            onClick={() =>
                                setCurrentPage((prev) =>
                                    Math.min(users.totalPages, prev + 1)
                                )
                            }
                            disabled={
                                currentPage === users.totalPages || loading
                            }
                            className="pagination-button"
                        >
                            Next
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminUsersPage;
