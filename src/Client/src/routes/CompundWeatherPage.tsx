import { Link, useParams } from "react-router-dom";
import { PageHeader } from "../components/PageHeader";
import { WeatherCard } from "../components/weather/WeatherCard";

export function CompoundWeatherPage() {
  const { city } = useParams<{ city: string }>();

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

      {city && (
        <WeatherCard city={city}>
          <WeatherCard.Header />
          <WeatherCard.Now />
          <WeatherCard.Forecast />
        </WeatherCard>
      )}
    </main>
  );
}
