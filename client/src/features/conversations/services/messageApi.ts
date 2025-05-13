import { HttpClient } from '../../../utils/http/HttpClient';
import { MessageDto, MessageCreateDto } from '../../../types/api';

export class MessageApi {
    private client: HttpClient;

    constructor(baseURL: string, signal: AbortSignal, getToken?: () => Promise<string | null>) {
        this.client = new HttpClient({ baseURL }, signal, getToken);
    }

    async getMessages(): Promise<MessageDto[]> {
        return this.client.get<MessageDto[]>('/messages');
    }

    async getMessage(id: string): Promise<MessageDto> {
        return this.client.get<MessageDto>(`/messages/${id}`);
    }

    async createMessage(data: MessageCreateDto): Promise<MessageDto> {
        return this.client.post<MessageDto>('/messages/add', data);
    }

    async deleteMessage(id: string): Promise<void> {
        await this.client.delete(`/messages/delete/${id}`);
    }
} 