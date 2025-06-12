import { HttpClient } from '../../utils/http/HttpClient';
import { UserDto, PaginatedResultDto, PaginationParameters } from '../../types/types';

export class UserApi {
    private client: HttpClient;

    constructor(
        baseURL: string,
        signal: AbortSignal,
        getToken?: () => Promise<string | null>
    ) {
        this.client = new HttpClient({ baseURL }, signal, getToken);
    }

    async getAllUsers(): Promise<UserDto[]> {
        return this.client.get<UserDto[]>('/users');
    }

    async getPaginatedUsers(params: PaginationParameters): Promise<PaginatedResultDto<UserDto>> {
        const searchParams = new URLSearchParams();
        searchParams.append('pageNumber', params.pageNumber.toString());
        searchParams.append('pageSize', params.pageSize.toString());
        if (params.searchTerm) searchParams.append('searchTerm', params.searchTerm);
        if (params.sortBy) searchParams.append('sortBy', params.sortBy);
        if (params.sortDescending) searchParams.append('sortDescending', params.sortDescending.toString());

        return this.client.get<PaginatedResultDto<UserDto>>(`/users?${searchParams.toString()}`);
    }

    async checkAdminStatus(): Promise<boolean> {
        const response = await this.client.get<{ isAdmin: boolean }>('/users/check-admin');
        return response.isAdmin;
    }

    async toggleAdmin(userId: string): Promise<boolean> {
        const response = await this.client.post<{ isAdmin: boolean }>(`/users/${userId}/toggle-admin`, {});
        return response.isAdmin;
    }

    async toggleBan(userId: string): Promise<boolean> {
        const response = await this.client.post<{ isBanned: boolean }>(`/users/${userId}/toggle-ban`, {});
        return response.isBanned;
    }
} 