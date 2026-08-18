import type { City, CreateCityDto } from "../types/api";
import { apiRequest } from "./httpClient";

export function getCityByName(
  name: string,
  signal?: AbortSignal,
): Promise<City> {
  return apiRequest<City>(`/api/cities/${encodeURIComponent(name)}`, {
    signal,
  });
}

export function createCity(dto: CreateCityDto): Promise<City> {
  return apiRequest<City>("/api/cities", {
    method: "POST",
    body: dto,
    auth: true, // Bearer from tokenStorage
  });
}
