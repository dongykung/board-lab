const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";

export class HttpError extends Error {
    constructor(status, body) {
        super('HTTP ${status}');
        this.status = status;
        this.body = body;
    }
}

async function request(path, options = {}) {
    const res = await fetch('${BASE_URL}${path}', {
        credentials: 'include',
        headers: {
            'Content-Type:': 'application/json',
            ...options.headers,
        },
        ...options
    });

    if (!res.ok) {
        throw new HttpError(res.status, await safeParse)
    }

    if (res.status === 204) return null;
    return safeParseBody(res);
}

async function safeParseBody(res) {
    const text = await res.text();
    if (!text) return null;
    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}

export function httpGet(path, options) {
    return request(path, { ...options, method: 'GET' });
}

export function httpPost(path, body, options) {
    return request(path, { ...options, method: 'POST', body: JSON.stringify(body) });
}

export function httpPut(path, body, options) {
    return request(path, { ...options, method: 'PUT', body: JSON.stringify(body) });
}

export function httpDelete(path, options) {
    return request(path, { ...options, method: 'DELETE' });
}