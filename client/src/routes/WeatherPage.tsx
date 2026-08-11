import { Link, useParams } from "react-router-dom";
import { weatherKeys } from "../features/cities/weatherKeys";
import { getCurrentWeather, getWeatherForecast } from "../api/weatherApi";
import { useQuery } from "@tanstack/react-query";
import { ApiError } from "../types/api";

export function WeatherPage() {
  const { city } = useParams();

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

  function errorText(error: unknown) {
    if (error instanceof ApiError) return error.message;
    if (error) return "Something went wrong";
    return null;
  }

  return (
    <main>
      <p>
        <Link to="/">← Home</Link>
        {city && (
          <>
            {" · "}
            <Link to={`/cities/${encodeURIComponent(city)}`}>City</Link>
          </>
        )}
      </p>

      <h1>Weather: {city}</h1>

      {currentQuery.isLoading && <p>Loading…</p>}
      {errorText(currentQuery.error) && (
        <p role="alert">{errorText(currentQuery.error)}</p>
      )}

      {currentQuery.data && (
        <section>
          <h2>Now</h2>
          <p>
            {currentQuery.data.temperature}° — {currentQuery.data.summary}
          </p>
          <p>{currentQuery.data.country}</p>
        </section>
      )}

      <section>
        <h2>7-day forecast</h2>
        {forecastQuery.isLoading && <p>Loading forecast…</p>}
        {errorText(forecastQuery.error) && (
          <p role="alert">{errorText(forecastQuery.error)}</p>
        )}
        <ul>
          {forecastQuery.data?.map((day) => (
            <li key={day.forecastedAt}>
              {new Date(day.forecastedAt).toLocaleDateString()}:{" "}
              {day.temperature}° — {day.summary}
            </li>
          ))}
        </ul>
      </section>
    </main>
  );
}
