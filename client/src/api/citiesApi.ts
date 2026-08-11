import type { City } from '../types/api'
import { apiRequest } from './httpClient'

export function getCityByName(name: string, signal?: AbortSignal): Promise<City> {
  return apiRequest<City>(`/api/cities/${encodeURIComponent(name)}`, { signal })
}
