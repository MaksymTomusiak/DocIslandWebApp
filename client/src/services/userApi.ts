import { HttpClient } from '../utils/http/HttpClient';
import { UserDto } from '../types/api';

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