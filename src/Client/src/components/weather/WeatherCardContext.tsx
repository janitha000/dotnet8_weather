import type { UseQueryResult } from "@tanstack/react-query";
import type { Unit } from "../../store/watchlistSlice";
import type { WeatherDto } from "../../types/api";
import { createContext, useContext } from "react";

export type WeatherCardContextValue = {
  city: string;
  unit: Unit;
  currentQuery: UseQueryResult<WeatherDto, Error>;
  forecastQuery: UseQueryResult<WeatherDto[], Error>;
  formatTemp: (celsius: number) => string;
};

export const WeatherCardContext = createContext<WeatherCardContextValue | null>(
  null,
);

export function useWeatherCard(): WeatherCardContextValue {
  const ctx = useContext(WeatherCardContext);
  if (!ctx) {
    throw new Error("WeatherCard parts must be used within <WeatherCard>");
  }
  return ctx;
}
