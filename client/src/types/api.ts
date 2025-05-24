export interface MessageDto {
    id: string;
    content: string;
    isResponse: boolean;
    createdAt: Date;
    conversationId: string;
}

export interface MessageCreateDto {
    content: string;
    conversationId: string;
}

export interface ConversationDto {
    id: string;
    fileName: string;
    createdAt: string;
    updatedAt: string;
}

export interface ConversationCreateDto {
    file: File;
}

export interface ApiError {
    message: string;
    statusCode: number;
} 