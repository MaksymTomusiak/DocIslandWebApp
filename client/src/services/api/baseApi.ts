import { HttpClient } from '../../utils/http/HttpClient';

export class BaseApi {
    protected client: HttpClient;

    constructor(baseURL: string, signal: AbortSignal) {
        this.client = new HttpClient({ baseURL }, signal);
    }
} 