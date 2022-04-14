export interface IHttpClient {
    get<T>(url: string): Promise<T>;

    post<T>(
        url: string,
        data?: any,
        appendHeaders?: Record<string, string>
    ): Promise<T>;

    postAxios<T>(
        url: string,
        data?: any,
        appendHeaders?: Record<string, string>
    ): Promise<T>;

    put<T>(
        url: string,
        data?: any,
        appendHeaders?: Record<string, string>
    ): Promise<T>;

    delete<T>(
        url: string
    ): Promise<T>;
}