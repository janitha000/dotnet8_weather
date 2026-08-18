import type { ReactNode } from "react";
import { useCityWeather } from "../../hooks/useCityWeather";
import { useAppSelector } from "../../store/hooks";
import { useWeatherCard, WeatherCardContext } from "./WeatherCardContext";
import { ErrorAlert } from "../ErrorAlert";
import { getErrorMessage } from "../../api/errorMapping";

function formatTemp(celsius: number, unit: "C" | "F") {
  if (unit === "C") return `${celsius}°C`;
  return `${Math.round((celsius * 9) / 5 + 32)}°F`;
}

type RootProps = {
  city: string;
  children: ReactNode;
};

function WeatherCardRoot({ city, children }: RootProps) {
  const { currentQuery, forecastQuery } = useCityWeather(city);
  const unit = useAppSelector((s) => s.watchlist.unit);
  const value = {
    city,
    unit,
    currentQuery,
    forecastQuery,
    formatTemp: (c: number) => formatTemp(c, unit),
  };
  return (
    <WeatherCardContext.Provider value={value}>
      <section className="weather-card">{children}</section>
    </WeatherCardContext.Provider>
  );
}

function Header() {
  const { city } = useWeatherCard();
  return <h2>{city}</h2>;
}

function Now() {
  const { currentQuery, formatTemp } = useWeatherCard();
  if (currentQuery.isLoading) return <p>Loading current…</p>;
  if (currentQuery.error) {
    return <ErrorAlert message={getErrorMessage(currentQuery.error)} />;
  }
  if (!currentQuery.data) return null;
  return (
    <div>
      <h3>Now</h3>
      <p>
        {formatTemp(currentQuery.data.temperature)} —{" "}
        {currentQuery.data.summary}
      </p>
      <p>{currentQuery.data.country}</p>
    </div>
  );
}

function Forecast() {
  const { forecastQuery, formatTemp } = useWeatherCard();
  if (forecastQuery.isLoading) return <p>Loading forecast…</p>;
  if (forecastQuery.error) {
    return <ErrorAlert message={getErrorMessage(forecastQuery.error)} />;
  }
  return (
    <div>
      <h3>7-day forecast</h3>
      <ul>
        {forecastQuery.data?.map((day) => (
          <li key={day.forecastedAt}>
            {new Date(day.forecastedAt).toLocaleDateString()}:{" "}
            {formatTemp(day.temperature)} — {day.summary}
          </li>
        ))}
      </ul>
    </div>
  );
}
export const WeatherCard = Object.assign(WeatherCardRoot, {
  Header,
  Now,
  Forecast,
});
