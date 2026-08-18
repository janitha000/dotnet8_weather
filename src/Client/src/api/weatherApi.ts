import type { WeatherDto } from "../types/api";
import { apiRequest } from "./httpClient";

export function getCurrentWeather(city: string, signal?: AbortSignal) {
  return apiRequest<WeatherDto>(`/api/weather/${encodeURIComponent(city)}`, {
    signal,
  });
}

export function getWeatherForecast(city: string, signal?: AbortSignal) {
  return apiRequest<WeatherDto[]>(
    `/api/weather/${encodeURIComponent(city)}/forecast`,
    { signal },
  );
}
