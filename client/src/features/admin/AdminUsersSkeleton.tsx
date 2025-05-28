import Skeleton from '../../components/common/skeleton/Skeleton';

const AdminUsersSkeleton = () => {
    return (
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
                    {[...Array(5)].map((_, index) => (
                        <tr key={index}>
                            <td>
                                <Skeleton
                                    variant="text"
                                    width="80%"
                                    height={16}
                                />
                            </td>
                            <td>
                                <Skeleton
                                    variant="text"
                                    width="90%"
                                    height={16}
                                />
                            </td>
                            <td>
                                <Skeleton
                                    variant="rectangular"
                                    width={60}
                                    height={24}
                                />
                            </td>
                            <td>
                                <Skeleton
                                    variant="rectangular"
                                    width={60}
                                    height={24}
                                />
                            </td>
                            <td>
                                <div className="actions" style={{ gap: '8px' }}>
                                    <Skeleton
                                        variant="rectangular"
                                        width={100}
                                        height={32}
                                    />
                                    <Skeleton
                                        variant="rectangular"
                                        width={100}
                                        height={32}
                                    />
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default AdminUsersSkeleton;
