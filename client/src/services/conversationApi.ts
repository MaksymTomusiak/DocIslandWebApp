import { HttpClient } from '../utils/http/HttpClient';
import { ConversationDto, ConversationCreateDto } from '../types/api';

export class ConversationApi {
    private client: HttpClient;

    constructor(
        baseURL: string,
        signal: AbortSignal,
        getToken?: () => Promise<string | null>
    ) {
        this.client = new HttpClient({ baseURL }, signal, getToken);
    }

    async getConversations(): Promise<ConversationDto[]> {
        return this.client.get<ConversationDto[]>('/conversations');
    }

    async getConversationsByUserId(): Promise<ConversationDto[]> {
        return this.client.get<ConversationDto[]>(`/conversations/user`);
    }

    async getConversation(id: string): Promise<ConversationDto> {
        return this.client.get<ConversationDto>(`/conversations/${id}`);
    }

    async createConversation(
        data: ConversationCreateDto
    ): Promise<ConversationDto> {
        const formData = new FormData();
        formData.append('File', data.file);
        return this.client.post<ConversationDto>(
            '/conversations/add',
            formData,
            {
                headers: { 'Content-Type': 'multipart/form-data' },
            }
        );
    }

    async deleteConversation(id: string): Promise<void> {
        await this.client.delete(`/conversations/delete/${id}`);
    }
}
