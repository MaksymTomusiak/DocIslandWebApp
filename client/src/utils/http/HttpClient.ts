import axios, {
    AxiosError,
    AxiosInstance,
    AxiosRequestConfig,
} from "axios";

export class HttpClient {
    private axiosInstance: AxiosInstance;
    private signal: AbortSignal;
    private getToken?: () => Promise<string | null>;

    constructor(configs: AxiosRequestConfig, signal: AbortSignal, getToken?: () => Promise<string | null>) {
        this.axiosInstance = axios.create({
            baseURL: configs.baseURL,
            headers: {
                "Content-Type": "application/json",
                Accept: "application/json",
                ...configs.headers,
            },
            ...configs,
        });

        this.signal = signal;
        this.getToken = getToken;
        this.initInterceptors();
    }

    async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
        return this.request<T>({ method: "GET", url, ...config });
    }

    async post<T>(
        url: string,
        data: unknown,
        config?: AxiosRequestConfig
    ): Promise<T> {
        return this.request<T>({ method: "POST", url, data, ...config });
    }

    async put<T>(
        url: string,
        data: unknown,
        config?: AxiosRequestConfig
    ): Promise<T> {
        return this.request<T>({ method: "PUT", url, data, ...config });
    }

    async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
        return this.request<T>({ method: "DELETE", url, ...config });
    }

    async request<T>(config: AxiosRequestConfig): Promise<T> {
        try {
            const response = await this.axiosInstance.request<T>({
                ...config,
                signal: this.signal,
            });

            if (!response || !response.data) {
                throw new Error("Empty response or invalid data format");
            }

            return response.data;
        } catch (error) {
            if (axios.isCancel(error)) {
                throw new Error("Request was cancelled");
            }
            throw error;
        }
    }

    initInterceptors() {
        this.axiosInstance.interceptors.request.use(
            async (config) => {
                if (this.getToken) {
                    const token = await this.getToken();
                    if (token) {
                        if (config.headers && typeof config.headers.set === 'function') {
                            config.headers.set('Authorization', `Bearer ${token}`);
                        } else {
                            config.headers = config.headers || {};
                            (config.headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
                        }
                    }
                }
                return config;
            },
            (error) => Promise.reject(error)
        );

        this.axiosInstance.interceptors.response.use(
            (response) => response,
            (error: AxiosError) => {
                return Promise.reject(error);
            }
        );
    }
}
