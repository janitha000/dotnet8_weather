import { ApiError } from "../types/api";

export function getErrorMessage(
  error: unknown,
  fallback = "Something went wrong",
): string {
  if (error instanceof ApiError) {
    if (error.status === 404) {
      return error.bodyText?.trim() || error.message || "Not found";
    }
    if (error.status === 409) {
      return error.problem?.detail ?? error.message ?? "Conflict";
    }
    if (error.status === 401) {
      return "Unauthorized — please sign in again";
    }
    if (error.status === 403) {
      return "You do not have permission to perform this action";
    }
    return error.message || fallback;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return fallback;
}
