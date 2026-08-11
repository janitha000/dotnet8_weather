import { getAccessToken } from "../auth/tokenStorage";
import { ApiError, type ProblemDetails } from "../types/api";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5260";

export type RequestOptions = {
  method?: string;
  body?: unknown;
  signal?: AbortSignal;
  auth?: boolean;
};

async function parseError(response: Response): Promise<ApiError> {
  const contentType = response.headers.get("content-type") ?? "";
  const text = await response.text();

  if (contentType.includes("application/json") && text) {
    try {
      const problem = JSON.parse(text) as ProblemDetails;
      const message =
        problem.detail ??
        problem.title ??
        `Request failed with status ${response.status}`;
      return new ApiError(response.status, message, problem, text);
    } catch {
      // fall through to plain text
    }
  }

  return new ApiError(
    response.status,
    text || `Request failed with status ${response.status}`,
    undefined,
    text,
  );
}

export async function apiRequest<T>(
  path: string,
  options: RequestOptions = {},
): Promise<T> {
  const headers: Record<string, string> = {
    Accept: "application/json",
  };

  if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
  }

  if (options.auth) {
    const token = getAccessToken();
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: options.method ?? "GET",
    headers,
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
    signal: options.signal,
  });

  if (!response.ok) {
    throw await parseError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export function getApiBaseUrl(): string {
  return API_BASE_URL;
}
