export interface City {
  id: number
  name: string
  country: string
  latitude: string
  longitude: string
  timeZone: string
}

export interface CreateCityDto {
  name: string
  country: string
  latitude: string
  longitude: string
  timeZone: string
}

export interface LoginDto {
  username: string
  password: string
}

export interface LoginResponse {
  accessToken: string
}

export interface WeatherDto {
  city: string
  country: string
  temperature: number
  summary: string
  retrievedAt: string
  forecastedAt: string
}

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  readonly status: number
  readonly problem?: ProblemDetails
  readonly bodyText?: string

  constructor(status: number, message: string, problem?: ProblemDetails, bodyText?: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
    this.bodyText = bodyText
  }
}
