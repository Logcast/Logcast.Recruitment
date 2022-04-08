import {IHttpClient} from "./httpClient";
import axios from 'axios';

export function WebApiClient(): IHttpClient {
    const webApiClient: IHttpClient = {
        async get<T>(url: string): Promise<T> {
            const response = await fetch(url, {
                credentials: "same-origin",
            });
            if (!response.ok) throw response;

            return parseResponse<T>(response);
        },

        async postAxios<T>(
        url: string,
        data?: any,
        appendHeaders?: Record<string, string>
        ): Promise<T> {
            const header = {
                "Enctype": "multipart/form-data",
                "Accept": "text/plain",
            };
            
            const response = await axios({
                method: 'post',
                url: url,
                data: data,
                headers: header
            });

            if (response.status !== 200) throw response;
            
            return await response.data;
        },

        async post<T>(
        url: string,
        data?: any,
        appendHeaders?: Record<string, string>
        ): Promise<T> {
            const response = await fetch(url, {
                headers: getHeaders(appendHeaders || {}),
                method: "post",
                credentials: "same-origin",
                body: data ? JSON.stringify(data) : undefined
            });

            if (!response.ok) throw response;

            return parseResponse<T>(response);
        },

        async put<T>(
            url: string,
            data?: any,
            appendHeaders?: Record<string, string>
        ): Promise<T> {
            const response = await fetch(url, {
                headers: getHeaders(appendHeaders || {}),
                method: "put",
                credentials: "same-origin",
                body: data ? JSON.stringify(data) : undefined
            });

            if (!response.ok) throw response;

            return parseResponse<T>(response);
        },

        async delete<T>(url: string): Promise<T> {
            const response = await fetch(url, {
                headers: getHeaders({}),
                method: "delete",
            });

            if (!response.ok) throw response;

            return parseResponse<T>(response);
        }
    };



    return webApiClient;
}

async function parseResponse<T>(response: Response) {
    let result = {};
    try {
        result = await response.json();
    } catch (e) {
        console.log("Response could not be parsed");
    }
    return result as T;
}

function getHeaders(appendHeaders: Record<string, string>): Headers {
    const headers = new Headers({
        "Content-Type": "application/json",
        Accept: "application/json",
        ...appendHeaders
    });
    return headers;
}