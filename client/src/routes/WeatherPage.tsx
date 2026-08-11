import { Link, useParams } from "react-router-dom";
import { getErrorMessage } from "../api/errorMapping";
import { ErrorAlert } from "../components/ErrorAlert";
import { PageHeader } from "../components/PageHeader";
import { useCityWeather } from "../hooks/useCityWeather";

export function WeatherPage() {
  const { city } = useParams<{ city: string }>();
  const { currentQuery, forecastQuery } = useCityWeather(city);

  const currentError = currentQuery.error
    ? getErrorMessage(currentQuery.error)
    : null;
  const forecastError = forecastQuery.error
    ? getErrorMessage(forecastQuery.error)
    : null;

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

      <PageHeader title={`Weather: ${city ?? ""}`} />

      {currentQuery.isLoading && <p>Loading…</p>}
      {currentError && <ErrorAlert message={currentError} />}

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
        {forecastError && <ErrorAlert message={forecastError} />}
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
