import { useQuery } from "@tanstack/react-query";
import { getCurrentWeather, getWeatherForecast } from "../api/weatherApi";
import { weatherKeys } from "../features/cities/weatherKeys";

export function useCityWeather(city: string | undefined) {
  const currentQuery = useQuery({
    queryKey: weatherKeys.current(city ?? ""),
    queryFn: ({ signal }) => getCurrentWeather(city!, signal),
    enabled: Boolean(city?.trim()),
    staleTime: 30_000,
  });

  const forecastQuery = useQuery({
    queryKey: weatherKeys.forecast(city ?? ""),
    queryFn: ({ signal }) => getWeatherForecast(city!, signal),
    enabled: Boolean(city?.trim()),
    staleTime: 60_000,
  });

  return { currentQuery, forecastQuery };
}
