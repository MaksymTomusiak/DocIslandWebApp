import { useEffect, useState } from 'react';
import { PaginationParameters, UserDto } from '../../types/api';
import { useUsers } from '../../hooks/useUsers';
import { Search } from '../../components/common/Search';
import { Pagination } from '../../components/common/Pagination';
import AdminUsersSkeleton from './AdminUsersSkeleton';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import UnfoldMoreIcon from '@mui/icons-material/UnfoldMore';
import './admin-users.css';

interface AdminUsersTableProps {
    baseURL: string;
}

export default function AdminUsersTable({ baseURL }: AdminUsersTableProps) {
    const {
        users,
        loading,
        error,
        fetchPaginatedUsers,
        toggleAdmin,
        toggleBan,
    } = useUsers(baseURL);
    const [searchTerm, setSearchTerm] = useState('');
    const [sortBy, setSortBy] = useState<string>('username');
    const [sortDescending, setSortDescending] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 3;

    useEffect(() => {
        const params: PaginationParameters = {
            pageNumber: currentPage,
            pageSize,
            searchTerm: searchTerm || undefined,
            sortBy: sortBy || undefined,
            sortDescending,
        };
        fetchPaginatedUsers(params);
    }, [currentPage, searchTerm, sortBy, sortDescending, fetchPaginatedUsers]);

    const handleSearch = (value: string) => {
        setSearchTerm(value);
        setCurrentPage(1);
    };

    const handleSort = (column: string) => {
        if (sortBy === column) {
            setSortDescending(!sortDescending);
        } else {
            setSortBy(column);
            setSortDescending(false);
        }
        setCurrentPage(1);
    };

    const getSortIcon = (column: string) => {
        if (sortBy !== column) return <UnfoldMoreIcon />;
        return sortDescending ? <ArrowDownwardIcon /> : <ArrowUpwardIcon />;
    };

    if (loading && !users) {
        return (
            <div className="users-table">
                <AdminUsersSkeleton />
            </div>
        );
    }

    if (error) {
        return <div className="error">{error}</div>;
    }

    if (!users) {
        return null;
    }

    return (
        <div className="users-table">
            <div className="table-controls">
                <Search
                    value={searchTerm}
                    onChange={handleSearch}
                    placeholder="Search users..."
                    className="search-input"
                />
            </div>

            <table>
                <thead>
                    <tr>
                        <th>
                            <div
                                className="sortable-header"
                                onClick={() => handleSort('username')}
                            >
                                Username
                                <span className="sort-icon">
                                    {getSortIcon('username')}
                                </span>
                            </div>
                        </th>
                        <th>
                            <div
                                className="sortable-header"
                                onClick={() => handleSort('email')}
                            >
                                Email
                                <span className="sort-icon">
                                    {getSortIcon('email')}
                                </span>
                            </div>
                        </th>
                        <th>Admin</th>
                        <th>Banned</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.items.map((user: UserDto) => (
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
                <div className="text-sm text-gray-500">
                    Showing {users.items.length} of {users.totalCount} users
                </div>
                <Pagination
                    currentPage={users.pageNumber}
                    totalPages={users.totalPages}
                    onPageChange={setCurrentPage}
                    className="pagination-controls"
                />
            </div>
        </div>
    );
}
