import { Skeleton } from '@mui/material';
import './admin-users.css';

const AdminUsersSkeleton = () => {
    return (
        <div className="users-table">
            <div className="table-controls">
                <Skeleton
                    variant="rectangular"
                    width={800}
                    height={40}
                    className="search-input"
                />
            </div>
            <div className="table-wrapper">
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
                        {[1, 2, 3, 4, 5].map((index) => (
                            <tr key={index}>
                                <td>
                                    <div className="truncate-cell">
                                        <Skeleton variant="text" width="80%" />
                                    </div>
                                </td>
                                <td>
                                    <div className="truncate-cell">
                                        <Skeleton variant="text" width="90%" />
                                    </div>
                                </td>
                                <td>
                                    <Skeleton
                                        variant="rectangular"
                                        width={60}
                                        height={24}
                                        className="status"
                                    />
                                </td>
                                <td>
                                    <Skeleton
                                        variant="rectangular"
                                        width={60}
                                        height={24}
                                        className="status"
                                    />
                                </td>
                                <td>
                                    <div className="actions">
                                        <Skeleton
                                            variant="rectangular"
                                            width={120}
                                            height={36}
                                            className="action-button"
                                        />
                                        <Skeleton
                                            variant="rectangular"
                                            width={120}
                                            height={36}
                                            className="action-button"
                                        />
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="table-footer">
                <Skeleton variant="text" width={200} height={20} />
                <div className="pagination-controls">
                    <Skeleton variant="rectangular" width={80} height={36} />
                    <Skeleton variant="text" width={100} height={20} />
                    <Skeleton variant="rectangular" width={80} height={36} />
                </div>
            </div>
        </div>
    );
};

export default AdminUsersSkeleton;
