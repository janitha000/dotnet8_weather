import type { LoginDto, LoginResponse } from "../types/api";
import { apiRequest } from "./httpClient";

export function login(dto: LoginDto): Promise<LoginResponse> {
  return apiRequest<LoginResponse>("/api/auth/login", {
    method: "POST",
    body: dto,
  });
}
